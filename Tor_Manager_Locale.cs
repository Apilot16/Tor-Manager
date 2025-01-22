using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace Tor_Manager_
{
    internal class Tor_Manager_Locale
    {
        private Process _torProcess;
        private TcpClient _client;
        private NetworkStream _stream;
        public bool _isAuthenticated = false;
        private bool _isConnected = false;
        public string TorPath { get; set; }
        public int ControlPort { get; set; }
        public string ControlPassword { get; set; }
        public string AuthCookiePath { get; set; }
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public event Func<object, string, Task> OnInfo;
        public event Func<object, string, Task> OnError;
        

        //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        public Tor_Manager_Locale(string torPath = null, int controlPort = 9151, string controlPassword = null, string authCookiePath = null)
        {
            TorPath = torPath;
            ControlPort = controlPort;
            ControlPassword = controlPassword;
            AuthCookiePath = authCookiePath;
        }

        private Task NotifyInfoAsync(string message)
        {
            return OnInfo?.Invoke(this, message) ?? Task.CompletedTask;
        }

        private Task NotifyErrorAsync(string message)
        {
            return OnError?.Invoke(this, message) ?? Task.CompletedTask;
        }

        private async Task LogExceptionAsync(Exception exception, string context)
        {
            string message = string.Format(Locale.GetString("ErrorGlobal"), context, exception.GetType().Name, exception.Message, exception.StackTrace);
            await NotifyErrorAsync(message);
        }

        private async Task<T> ExecuteSafelyAsync<T>(Func<Task<T>> action, string errorMessage)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                await LogExceptionAsync(ex, errorMessage);
                throw; // Повторно бросаем исключение для дальнейшей обработки, если необходимо
            }
        }

        private async Task ExecuteSafelyAsync(Func<Task> action, string errorMessage)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                await LogExceptionAsync(ex, errorMessage);
                throw;
            }
        }

        private string GetTorHashedPassword(string password)
        {
            Process process = new Process();
            process.StartInfo.FileName = TorPath;
            process.StartInfo.Arguments = $"--hash-password {password}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string hashedPassword = lines[lines.Length - 1];

            return hashedPassword;
        }

        public async Task StartTor()
        {
            await ExecuteSafelyAsync(async () =>
            {
                if (_torProcess != null && !_torProcess.HasExited)
                {
                    await NotifyInfoAsync(Locale.GetString("TorRunning"));
                    return;
                }
                if (!File.Exists(TorPath)) { throw new FileNotFoundException(string.Format(Locale.GetString("NotFoundFileTor"), TorPath)); }

                string authenticationMethod = string.Empty;
                if (!string.IsNullOrEmpty(ControlPassword))
                {
                    authenticationMethod = $"--HashedControlPassword {GetTorHashedPassword(ControlPassword)}";
                }
                else if (string.IsNullOrEmpty(AuthCookiePath)) authenticationMethod = "DataDirectory %TEMP% -CookieAuthentication 1";

                _torProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = TorPath,
                        Arguments = $"--ControlPort {ControlPort} {authenticationMethod}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    },
                    EnableRaisingEvents = true
                };

                _torProcess.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        Task.Run(() => NotifyInfoAsync(args.Data));
                };
                _torProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        Task.Run(() => NotifyErrorAsync(args.Data));
                };

                await NotifyErrorAsync($"--ControlPort {ControlPort} {authenticationMethod}");
                _torProcess.Exited += (sender, e) => { Task.Run(() => NotifyInfoAsync(Locale.GetString("TorExited"))); };
                _torProcess.Start();
                _torProcess.BeginOutputReadLine();
                _torProcess.BeginErrorReadLine();
            }, Locale.GetString("ErrorLaunchTor"));
        }

        public async Task StopTor()
        {

            await ExecuteSafelyAsync(async () =>
            {
                if (_torProcess != null && !_torProcess.HasExited)
                {
                    _torProcess.Kill();
                    _torProcess.WaitForExit();
                    await NotifyInfoAsync(Locale.GetString("TorKilled"));
                }
                _torProcess = null;
            }, Locale.GetString("ErrorStopingTor"));
        }

        public async Task ConnectAsync()
        {
            await _semaphore.WaitAsync(); // Начало критической секции
            try
            {
                await ExecuteSafelyAsync(async () =>
                {
                    _client = new TcpClient("127.0.0.1", ControlPort);
                    _stream = _client.GetStream();

                    if (!string.IsNullOrEmpty(ControlPassword))
                    {
                        await AuthenticateWithParameterAsync($"\"{ControlPassword}\"", Locale.GetString("ByPass"));
                    }
                    else if (!string.IsNullOrEmpty(AuthCookiePath) && File.Exists(AuthCookiePath))
                    {
                        byte[] cookieBytes = File.ReadAllBytes(AuthCookiePath);
                        string cookieHex = BitConverter.ToString(cookieBytes).Replace("-", string.Empty);
                        await AuthenticateWithParameterAsync(cookieHex, Locale.GetString("ByCookie"));
                    }
                    else
                    {
                        await SendAsync("GETCONF\n");
                        string response = await ReadResponseAsync();
                        if (response.Contains("250 OK"))
                        {
                            _isAuthenticated = true;
                            await NotifyInfoAsync(Locale.GetString("SuccessfullAuthenticationNoPass"));
                        }
                        else
                        {
                            throw new Exception(Locale.GetString("ErrorAuthenticationNoPass") + response);
                        }
                    }
                    _isConnected = true; // Установить флаг после успешного подключения
                }, Locale.GetString("ErrorConnecting"));
            }
            finally
            {
                _semaphore.Release(); // Конец критической секции
            }
        }

        public async void Disconnect()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
                _isAuthenticated = false;
                _isConnected = false; // Установить флаг после отключения
                await NotifyInfoAsync(Locale.GetString("ClosedConnectionTor"));
            }
            catch (Exception e)
            {
                await NotifyErrorAsync(string.Format(Locale.GetString("ErrorDisconnect"), e.Message));
            }
        }

        private Task AuthenticateWithParameterAsync(string parameter, string method)
        {
            return ExecuteSafelyAsync(async () =>
            {
                await SendAsync($"AUTHENTICATE {parameter}\n");
                string response = await ReadResponseAsync();

                if (response.Contains("250 OK"))
                {
                    _isAuthenticated = true;
                    await NotifyInfoAsync(string.Format(Locale.GetString("SuccessfullAuthentication"), method));
                }
                else
                {
                    throw new Exception(string.Format(Locale.GetString("ErrorAuthenticationMetodResponse"), method, response));
                }
            }, string.Format(Locale.GetString("ErrorAuthenticationMetod"), method));
        }

        public async Task<string> SendCommandAsync(string command)
        {
            if (!_isConnected)
            {
                await ConnectAsync();
                _isConnected = true;
            }
            return await ExecuteSafelyAsync(async () =>
            {
                await SendAsync(command + "\n");
                return await ReadResponseAsync();
            }, string.Format(Locale.GetString("ErrorLaunchCommand"), command));
        }

        private async Task SendAsync(string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }


        private async Task<string> ReadResponseAsync()
        {
            var buffer = new byte[4096];
            string response = string.Empty;

            while (true)
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break; // Закрытие соединения
                response += Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if (response.Contains("\r\n")) // Проверка конца строки
                {
                        break;
                }
            }

            await NotifyInfoAsync(string.Format(Locale.GetString("ReadedBytes"), response.Length, response.Trim()));
            return response;
        }


        public async Task<bool> IsTorRunningAsync()
        {
            return await Task.Run(() =>
            {
                // Проверка на наличие процесса Tor
                var processes = Process.GetProcessesByName("tor");
                return processes.Length > 0;
            });
        }


        public async Task GatherTorProcessInfoAsync()
        {
            var outInfo = new StringBuilder();
            var controlPorts = new HashSet<int>
            {
                ControlPort //Добавление текущего ControlPort для проверки.
            };
            var torDataDirectories = new HashSet<string>();
            

            try
            {
                // Найти путь к исполняемому файлу Tor
                var processes = Process.GetProcessesByName("tor");
                if (processes.Length == 0)
                {
                    await NotifyErrorAsync(Locale.GetString("NotFoundTorProcess"));
                    return;
                }

                string torExecutablePath = processes[0].MainModule.FileName;
                string torDataDirectory = Path.GetDirectoryName(torExecutablePath);
                //torDataDirectories.Add(torDataDirectory);

                // Получить аргументы запуска процесса Tor
                string query = $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {processes[0].Id}";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                using (ManagementObjectCollection objects = searcher.Get())
                {
                    foreach (ManagementBaseObject obj in objects)
                    {
                        string commandLine = obj["CommandLine"]?.ToString();
                        var regex = new Regex(@"(DataDirectory|[+]__ControlPort|controlport)(?:\s+(""[^""]*""|\S*))?", RegexOptions.IgnoreCase);
                        var matches = regex.Matches(commandLine);

                        foreach (Match match in matches)
                        {
                            string key = match.Groups[1].Value;
                            string value = match.Groups[2].Success ? match.Groups[2].Value.Trim('"') : null;

                            if (key.Contains("ControlPort") && value != null)
                            {
                                if (value.Contains(":"))
                                {
                                    if (int.TryParse(value.Split(':')[1], out int port))
                                    {
                                        controlPorts.Add(port);
                                        outInfo.AppendLine(string.Format(Locale.GetString("FoundControlPortInArguments"), port));
                                    }
                                }
                                else
                                {
                                    if (int.TryParse(value, out int port))
                                    {
                                        controlPorts.Add(port);
                                        outInfo.AppendLine(string.Format(Locale.GetString("FoundControlPortInArguments"), port));
                                    }
                                }
                            }
                            if (key.Equals("DataDirectory", StringComparison.OrdinalIgnoreCase) && value != null)
                            {
                                torDataDirectories.Add(value);
                                outInfo.AppendLine(string.Format(Locale.GetString("FoundDataDirectoryInArguments"), value));
                            }
                        }
                        break;
                    }
                }

                // Получение слушающих портов
                var listeningPorts = new List<int>();
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "netstat",
                        Arguments = "-ano",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();

                var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains($" {processes[0].Id}") && line.Contains("LISTENING"))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 4 && parts[1].Contains(':'))
                        {
                            string portPart = parts[1].Split(':')[1];
                            if (int.TryParse(portPart, out int port))
                            {
                                listeningPorts.Add(port);
                                //controlPorts.Add(port);
                                outInfo.AppendLine(string.Format(Locale.GetString("FoundListenPort"), port));
                            }
                        }
                    }
                }

                // Поиск файла control_auth_cookie
               string authCookiePath = Path.Combine(torDataDirectory, "..", "Data", "Tor", "control_auth_cookie");
                torDataDirectories.Add(Path.GetFullPath(Path.Combine(torDataDirectory, "..", "Data", "Tor")));
                if (File.Exists(authCookiePath))
                {
                    AuthCookiePath = authCookiePath;
                    outInfo.AppendLine(string.Format(Locale.GetString("Foundcontrol_auth_cookie"), authCookiePath));
                }
                else
                {
                    foreach (var directory in torDataDirectories)
                    {
                        var tempPath = Path.GetFullPath(Path.Combine(directory, "control_auth_cookie"));
                        if (File.Exists(tempPath))
                        {
                            AuthCookiePath = tempPath;
                            break;
                        }
                    }
                    outInfo.AppendLine(Locale.GetString("NotFoundControl_auth_cookie"));

                }

                // Определение ControlPort из файла конфигурации
                string torrcPath = Path.Combine(torDataDirectory, "..", "Data", "Tor", "torrc");
                if (File.Exists(torrcPath))
                {
                    try
                    {
                        var torrcLines = File.ReadAllLines(torrcPath);
                        foreach (var line in torrcLines)
                        {
                            var cleanedLine = line.Trim();
                            if (cleanedLine.StartsWith("ControlPort ", StringComparison.OrdinalIgnoreCase))
                            {
                                var portValue = cleanedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                                if (int.TryParse(portValue, out int torrcPort))
                                {
                                    controlPorts.Add(torrcPort);
                                    outInfo.AppendLine(string.Format(Locale.GetString("ControlPortFromTorrc"), torrcPort));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        await NotifyErrorAsync(string.Format(Locale.GetString("ErrorAnalysetorrc"),e.Message));
                    }
                }
                else
                {
                    outInfo.AppendLine(Locale.GetString("NotFoundtorrc"));
                }

                // Сравнение controlPorts с listeningPorts
                var confirmedControlPort = controlPorts.Intersect(listeningPorts).FirstOrDefault();
                if (confirmedControlPort != 0)
                {
                    ControlPort = confirmedControlPort;
                    outInfo.AppendLine(string.Format(Locale.GetString("ConfirmedControlPort"), ControlPort));
                }
                else
                {
                    outInfo.AppendLine(Locale.GetString("NotConfirmedControlPort"));
                }

                if (torDataDirectories.Count > 1)
                {
                    foreach (var directory in torDataDirectories) { outInfo.AppendLine($"{directory}"); }
                    outInfo.AppendLine(Locale.GetString("DifferentsInDataDirectory"));
                }
                else
                {
                    outInfo.AppendLine(string.Format(Locale.GetString("ConfirmedDataDirectory"),torDataDirectories.FirstOrDefault()));
                }
                await NotifyInfoAsync(outInfo.ToString());
            }
            catch (Exception e)
            {
                await NotifyErrorAsync(string.Format(Locale.GetString("ErrorGrabbingTorInfo"),  e.Message,e.StackTrace));
            }
        }


    }
}
