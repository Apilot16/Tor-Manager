using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tor_Manager_.Properties;


namespace Tor_Manager_
{
    public partial class Form1 : Form
    {
        private readonly Tor_Manager_Locale tor_Manager;
        private int currentScaleFactorpercent = 100;// Начальный масштаб
        private readonly float baseFontSize = 8.25f;
        // Сохраним базовый размер формы и контролов
        private SizeF baseScale = new SizeF(1f, 1f);
        public Form1()
        {
            // Устанавливаем AutoScaleMode на основе DPI
            this.AutoScaleMode = AutoScaleMode.Dpi;
            // Определяем текущий DPI
            using (Graphics graphics = this.CreateGraphics())
            {
                float dpiX = graphics.DpiX; // Горизонтальный DPI
                float dpiY = graphics.DpiY; // Вертикальный DPI

                // Устанавливаем AutoScaleDimensions на основе текущего DPI
                this.AutoScaleDimensions = new SizeF(dpiX, dpiY);
            }

            // Включаем предварительный просмотр клавиш формы
            this.KeyPreview = true;
            this.MouseWheel += Form1_MouseWheel;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("RU");
            InitializeComponent();
            tor_Manager = new Tor_Manager_Locale(Settings.Default.Tor_path, Settings.Default.Control_port, Settings.Default.Control_password, Settings.Default.Cookie_path);
            tor_Manager.OnError += async (sender, message) => { richTextBox1.Text += message + "\n"; };
            tor_Manager.OnInfo += async (sender, message) => { richTextBox1.Text += message + "\n"; };
            UpdateUI();
        }

        private async void Start_Click(object sender, EventArgs e)
        {
            try
            {
                tor_Manager.TorPath = Settings.Default.Tor_path;
                tor_Manager.AuthCookiePath = !string.IsNullOrEmpty(Settings.Default.Cookie_path) ? Settings.Default.Cookie_path : string.Empty;
                tor_Manager.ControlPort = Settings.Default.Control_port;
                tor_Manager.ControlPassword = !string.IsNullOrEmpty(textBox1.Text) ? (textBox1.Text) : null;
                if (!await tor_Manager.IsTorRunningAsync())
                {
                    await tor_Manager.StartTor();
                    await Task.Delay(3000); // Ждем запуска Tor
                    if (await tor_Manager.IsTorRunningAsync()) { UpdateStatus(Locale.GetString("TorStarted")); } else { UpdateStatus(Locale.GetString("TorNotStarted")); }
                }
                else
                {
                    UpdateStatus(Locale.GetString("TorRunning"));
                }
                await tor_Manager.GatherTorProcessInfoAsync();
                await tor_Manager.ConnectAsync();
                if (tor_Manager._isAuthenticated) { UpdateStatus(Locale.GetString("TorStartedAndConnected")); }
            }
            catch (Exception ex)
            {
                UpdateStatus(Locale.GetString("Error") + ex.Message);
            }
        }

        private async void Stop_Click(object sender, EventArgs e)
        {
            try
            {
                await tor_Manager.StopTor();
                tor_Manager.Disconnect();
                UpdateStatus(Locale.GetString("TorKilled"));
            }
            catch (Exception ex)
            {
                UpdateStatus(Locale.GetString("Error") + ex.Message);
            }
        }

        private async void UpdateUI()
        {
            if (Settings.Default.Replace_Command_text) { ReplaceCommand.Checked = true; } else { ReplaceCommand.Checked = false; }
            Start.BackColor = SystemColors.Control;
            if (Settings.Default.Cookie_path != null)
            {
                if (Settings.Default.Cookie_path.Contains("control_auth_cookie"))
                {
                    CookieBox.BackColor = Color.GreenYellow;
                }
                else
                {
                    CookieBox.BackColor = SystemColors.Control;
                }
            }
            if (Settings.Default.Tor_path != null)
            {
                if (Settings.Default.Tor_path.Contains("tor.exe"))
                {
                    TorBox.BackColor = Color.GreenYellow; Start.Enabled = true;
                }
                else
                {
                    TorBox.BackColor = SystemColors.Control; Start.Enabled = false;
                }
            }
            if (await tor_Manager.IsTorRunningAsync()) { Start.Enabled = false; Start.BackColor = SystemColors.Highlight; }
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => label1.Text = message));
            }
            else
            {
                label1.Text = message;
            }
        }

        private async void Connect_Click(object sender, EventArgs e)
        {
            try
            {
                if (await tor_Manager.IsTorRunningAsync())
                {
                    if (MessageBox.Show(Locale.GetString("FoundRunningTor"), Locale.GetString("Connect"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        tor_Manager.ControlPassword = null;
                        await tor_Manager.GatherTorProcessInfoAsync();
                    }
                    else
                    {
                        tor_Manager.ControlPort = Settings.Default.Control_port;
                        tor_Manager.AuthCookiePath = File.Exists(Settings.Default.Cookie_path) ? Settings.Default.Cookie_path : null;
                        tor_Manager.ControlPassword = !string.IsNullOrEmpty(textBox1.Text) ? textBox1.Text : null;
                    }
                    await tor_Manager.ConnectAsync();
                }
                else
                {
                    UpdateStatus(Locale.GetString("TorNotRunning"));
                }
            }
            catch (Exception ex)
            {
                UpdateStatus(Locale.GetString("Error") + ex.Message);
            }
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                tor_Manager.Disconnect();
                UpdateStatus(Locale.GetString("TorDisconnected"));
            }
            catch (Exception ex)
            {
                UpdateStatus(Locale.GetString("Error") + ex.Message);
            }
        }

        private async void SendCommand_Click(object sender, EventArgs e)
        {
            try
            {
                string response = await tor_Manager.SendCommandAsync(textBox1.Text);
                UpdateStatus(Locale.GetString("Answer") + response);
            }
            catch (Exception ex)
            {
                UpdateStatus(Locale.GetString("Error") + ex.Message);
            }
        }

        private void CookieBox_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = Locale.GetString("CookieTitle");
            openFileDialog1.Filter = Locale.GetString("CookieFilter");
            openFileDialog1.FilterIndex = 1; // Индекс фильтра по умолчанию (1 - первый)
            CookieBox.BackColor = SystemColors.Control;
            Settings.Default.Cookie_path = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Contains("control_auth_cookie"))
                {
                    Settings.Default.Cookie_path = openFileDialog1.FileName;
                    CookieBox.BackColor = System.Drawing.Color.GreenYellow;


                    UpdateStatus(Settings.Default.Cookie_path);



                }
                else
                {
                    CookieBox.BackColor = System.Drawing.Color.PaleVioletRed;
                    if (MessageBox.Show(Locale.GetString("FileWrong"), Locale.GetString("ErrorChoise"), MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.Retry)
                    {
                        CookieBox_Click(sender, e);
                    }
                }
            }
            else
            {
                // Если пользователь отменил диалог, выходим из метода
                return;
            }
        }

        private void TorBox_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = Locale.GetString("TorBoxTitle");
            openFileDialog1.Filter = Locale.GetString("TorBoxFilter");
            openFileDialog1.FilterIndex = 1; // Индекс фильтра по умолчанию (1 - первый)
            TorBox.BackColor = SystemColors.Control;
            Start.Enabled = false;
            Settings.Default.Tor_path = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Contains("tor.exe"))
                {
                    Settings.Default.Tor_path = openFileDialog1.FileName;
                    TorBox.BackColor = System.Drawing.Color.GreenYellow;
                    Start.Enabled = true;

                    UpdateStatus(Settings.Default.Tor_path);


                }
                else
                {
                    Start.Enabled = false;
                    TorBox.BackColor = System.Drawing.Color.PaleVioletRed;
                    if (MessageBox.Show(Locale.GetString("FileWrong"), Locale.GetString("ErrorChoise"), MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.Retry)
                    {
                        TorBox_Click(sender, e);
                    }
                }
            }
            else
            {
                // Если пользователь отменил диалог, выходим из метода
                return;
            }
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                await tor_Manager.StopTor();
                tor_Manager.Disconnect();
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(string.Format(Locale.GetString("ErrorCloseCL"), ex.Message), Locale.GetString("Error2"), MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.No) { e.Cancel = true; }
                UpdateStatus(string.Format(Locale.GetString("ErrorClose"), ex.Message));
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                // Проверка нажатия клавиш для '+' и '-' как на цифровой клавиатуре, так и на основной
                if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus)
                {
                    AdjustScale(10);  //AdjustScale(1.1f); // Увеличиваем масштаб при прокрутке вверх
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
                {
                    AdjustScale(-10); //AdjustScale(0.9f); // Уменьшаем масштаб при прокрутке вниз
                    e.Handled = true;
                }
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {

            if (ModifierKeys.HasFlag(Keys.Control))
            {
                if (e.Delta > 0)
                {
                    AdjustScale(10);   //  AdjustScale(1.1f); // // Увеличиваем масштаб при прокрутке вверх
                }
                else if (e.Delta < 0)
                {
                    AdjustScale(-10);  //   AdjustScale(0.9f);// // Уменьшаем масштаб при прокрутке вниз
                }
            }
        }

        private void AdjustScale(int scaleFactor)
        {
            // Обновляем текущий процент масштабирования
            currentScaleFactorpercent += scaleFactor;
            // Вычисляем новый масштаб на основе базового масштаба (100% = 1.0f)
            float newScaleFactor = currentScaleFactorpercent / 100f;

            // Устанавливаем новый размер формы на основе базового размера
            this.Width = (int)(baseScale.Width * newScaleFactor);
            this.Height = (int)(baseScale.Height * newScaleFactor);

            // Обновляем размер шрифта для всех контролов
            AdjustFontSize(this, currentScaleFactorpercent);
        }

        private void AdjustFontSize(Control control, float scaleFactorPercent)
        {
            //UpdateStatus("current: " + this.SendCommand.Font.Size.ToString());//+$" {currentScaleFactorpercent} |/ {(this.SendCommand.Font.Size*currentScaleFactorpercent)/100}");

            // Вычисляем новый размер шрифта на основе базового размера и текущего масштаба
            float newFontSize = baseFontSize * scaleFactorPercent / 100;

            // Устанавливаем новый размер шрифта
            control.Font = new Font(control.Font.FontFamily, newFontSize, control.Font.Style);

            // Рекурсивно масштабируем шрифты дочерних контролов
            foreach (Control childControl in control.Controls)
            {
                AdjustFontSize(childControl, scaleFactorPercent);
            }
        }


        private void SetPort_Click(object sender, EventArgs e)
        {
            Settings.Default.Control_port = int.TryParse(textBox1.Text, out int port) ? port : Settings.Default.Control_port;
            textBox1.Text = null;
            UpdateStatus(string.Format(Locale.GetString("SetPort"), Settings.Default.Control_port));
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (Settings.Default.Replace_Command_text)
            {
                textBox1.Text = ListOfCommands.SelectedItem.ToString();
            }
            else
            {
                textBox1.Text += ListOfCommands.SelectedItem.ToString();
            }
        }

        private void ReplaceCommand_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Replace_Command_text = ReplaceCommand.Checked;
        }

        private void ChangeLocale_Click(object sender, EventArgs e)
        {
            var Ru_culture = new CultureInfo("ru");
            var En_culture = new CultureInfo("en");
            if (Thread.CurrentThread.CurrentUICulture == CultureInfo.DefaultThreadCurrentCulture || Thread.CurrentThread.CurrentUICulture.Name == Ru_culture.Name)
            {
                Thread.CurrentThread.CurrentUICulture = En_culture;

            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = Ru_culture;
            }
            ApplyResources();
            UpdateUI();
        }

        private void ApplyResources(Control control, ComponentResourceManager resources, ToolTip toolTip)
        {
            resources.ApplyResources(control, control.Name);

            // Устанавливаем локализованный текст для ToolTip
            if (toolTip != null)
            {
                string toolTipText = resources.GetString(control.Name + ".ToolTip");
                if (!string.IsNullOrEmpty(toolTipText))
                {
                    toolTip.SetToolTip(control, toolTipText);
                }
            }

            // Рекурсивно обновляем дочерние элементы
            foreach (Control childControl in control.Controls)
            {
                ApplyResources(childControl, resources, toolTip);
            }

            // Если у элемента есть дочерние элементы, такие как ToolStrip, нужно обработать их отдельно
            if (control is ToolStrip toolStrip)
            {
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    ApplyResources(item, resources);
                }
            }

            if (control is MenuStrip menuStrip)
            {
                foreach (ToolStripItem menuItem in menuStrip.Items)
                {
                    ApplyResources(menuItem, resources);
                }
            }
        }

        private void ApplyResources()
        {
            var resources = new ComponentResourceManager(typeof(Form1));
            var toolTip = this.toolTip1; // Предполагается, что у вас есть объект ToolTip

            resources.ApplyResources(this, "$this"); // Применить ресурсы к самой форме

            // Применить ресурсы ко всем элементам управления
            ApplyResources(this, resources, toolTip);
        }

        private void ApplyResources(ToolStripItem item, ComponentResourceManager resources)
        {
            resources.ApplyResources(item, item.Name);

            if (item is ToolStripMenuItem menuItem)
            {
                foreach (ToolStripItem dropDownItem in menuItem.DropDownItems)
                {
                    ApplyResources(dropDownItem, resources);
                }
            }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                AboutBox1 aboutBox = new AboutBox1();
                aboutBox.ShowDialog();
                return true; // Указывает, что нажатие обработано

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Сохраняем базовый размер формы при загрузке
            baseScale = new SizeF(this.Width, this.Height);
        }
    }
}
