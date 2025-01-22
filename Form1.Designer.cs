namespace Tor_Manager_
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ChangeLocale = new System.Windows.Forms.Button();
            this.ReplaceCommand = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ListOfCommands = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.TorBox = new System.Windows.Forms.PictureBox();
            this.SetPort = new System.Windows.Forms.Button();
            this.CookieBox = new System.Windows.Forms.PictureBox();
            this.Start = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.SendCommand = new System.Windows.Forms.Button();
            this.Connect = new System.Windows.Forms.Button();
            this.Disconnect = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CookieBox)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ChangeLocale);
            this.splitContainer1.Panel1.Controls.Add(this.ReplaceCommand);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.ListOfCommands);
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.toolTip1.SetToolTip(this.splitContainer1, resources.GetString("splitContainer1.ToolTip"));
            // 
            // ChangeLocale
            // 
            resources.ApplyResources(this.ChangeLocale, "ChangeLocale");
            this.ChangeLocale.Name = "ChangeLocale";
            this.toolTip1.SetToolTip(this.ChangeLocale, resources.GetString("ChangeLocale.ToolTip"));
            this.ChangeLocale.UseVisualStyleBackColor = true;
            this.ChangeLocale.Click += new System.EventHandler(this.ChangeLocale_Click);
            // 
            // ReplaceCommand
            // 
            resources.ApplyResources(this.ReplaceCommand, "ReplaceCommand");
            this.ReplaceCommand.Name = "ReplaceCommand";
            this.toolTip1.SetToolTip(this.ReplaceCommand, resources.GetString("ReplaceCommand.ToolTip"));
            this.ReplaceCommand.UseVisualStyleBackColor = true;
            this.ReplaceCommand.CheckedChanged += new System.EventHandler(this.ReplaceCommand_CheckedChanged);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.toolTip1.SetToolTip(this.textBox1, resources.GetString("textBox1.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // ListOfCommands
            // 
            this.ListOfCommands.FormattingEnabled = true;
            this.ListOfCommands.Items.AddRange(new object[] {
            resources.GetString("ListOfCommands.Items"),
            resources.GetString("ListOfCommands.Items1"),
            resources.GetString("ListOfCommands.Items2"),
            resources.GetString("ListOfCommands.Items3"),
            resources.GetString("ListOfCommands.Items4"),
            resources.GetString("ListOfCommands.Items5"),
            resources.GetString("ListOfCommands.Items6"),
            resources.GetString("ListOfCommands.Items7"),
            resources.GetString("ListOfCommands.Items8"),
            resources.GetString("ListOfCommands.Items9"),
            resources.GetString("ListOfCommands.Items10"),
            resources.GetString("ListOfCommands.Items11"),
            resources.GetString("ListOfCommands.Items12"),
            resources.GetString("ListOfCommands.Items13"),
            resources.GetString("ListOfCommands.Items14"),
            resources.GetString("ListOfCommands.Items15"),
            resources.GetString("ListOfCommands.Items16")});
            resources.ApplyResources(this.ListOfCommands, "ListOfCommands");
            this.ListOfCommands.Name = "ListOfCommands";
            this.toolTip1.SetToolTip(this.ListOfCommands, resources.GetString("ListOfCommands.ToolTip"));
            this.ListOfCommands.Click += new System.EventHandler(this.listBox1_Click);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.Start, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Stop, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SendCommand, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.Connect, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Disconnect, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.TorBox, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.SetPort, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.CookieBox, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // TorBox
            // 
            this.TorBox.BackgroundImage = global::Tor_Manager_.Properties.Resources.VisualElements_70;
            resources.ApplyResources(this.TorBox, "TorBox");
            this.TorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TorBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TorBox.Name = "TorBox";
            this.TorBox.TabStop = false;
            this.toolTip1.SetToolTip(this.TorBox, resources.GetString("TorBox.ToolTip"));
            this.TorBox.Click += new System.EventHandler(this.TorBox_Click);
            // 
            // SetPort
            // 
            resources.ApplyResources(this.SetPort, "SetPort");
            this.SetPort.Name = "SetPort";
            this.toolTip1.SetToolTip(this.SetPort, resources.GetString("SetPort.ToolTip"));
            this.SetPort.UseVisualStyleBackColor = true;
            this.SetPort.Click += new System.EventHandler(this.SetPort_Click);
            // 
            // CookieBox
            // 
            this.CookieBox.BackgroundImage = global::Tor_Manager_.Properties.Resources.Cookie_1;
            resources.ApplyResources(this.CookieBox, "CookieBox");
            this.CookieBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CookieBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CookieBox.Name = "CookieBox";
            this.CookieBox.TabStop = false;
            this.toolTip1.SetToolTip(this.CookieBox, resources.GetString("CookieBox.ToolTip"));
            this.CookieBox.Click += new System.EventHandler(this.CookieBox_Click);
            // 
            // Start
            // 
            resources.ApplyResources(this.Start, "Start");
            this.Start.Name = "Start";
            this.toolTip1.SetToolTip(this.Start, resources.GetString("Start.ToolTip"));
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // Stop
            // 
            resources.ApplyResources(this.Stop, "Stop");
            this.Stop.Name = "Stop";
            this.toolTip1.SetToolTip(this.Stop, resources.GetString("Stop.ToolTip"));
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // SendCommand
            // 
            resources.ApplyResources(this.SendCommand, "SendCommand");
            this.SendCommand.Name = "SendCommand";
            this.toolTip1.SetToolTip(this.SendCommand, resources.GetString("SendCommand.ToolTip"));
            this.SendCommand.UseVisualStyleBackColor = true;
            this.SendCommand.Click += new System.EventHandler(this.SendCommand_Click);
            // 
            // Connect
            // 
            resources.ApplyResources(this.Connect, "Connect");
            this.Connect.Name = "Connect";
            this.toolTip1.SetToolTip(this.Connect, resources.GetString("Connect.ToolTip"));
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // Disconnect
            // 
            resources.ApplyResources(this.Disconnect, "Disconnect");
            this.Disconnect.Name = "Disconnect";
            this.toolTip1.SetToolTip(this.Disconnect, resources.GetString("Disconnect.ToolTip"));
            this.Disconnect.UseVisualStyleBackColor = true;
            this.Disconnect.Click += new System.EventHandler(this.Disconnect_Click);
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.Name = "richTextBox1";
            this.toolTip1.SetToolTip(this.richTextBox1, resources.GetString("richTextBox1.ToolTip"));
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitContainer1);
            this.HelpButton = true;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CookieBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.Button Disconnect;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ListBox ListOfCommands;
        private System.Windows.Forms.CheckBox ReplaceCommand;
        private System.Windows.Forms.PictureBox TorBox;
        private System.Windows.Forms.Button SetPort;
        private System.Windows.Forms.PictureBox CookieBox;
        private System.Windows.Forms.Button SendCommand;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button ChangeLocale;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

