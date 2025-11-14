
namespace FTP.Server
{
    partial class FtpServerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSetting = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnBrowseRoot = new System.Windows.Forms.Button();
            this.txtRootFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.statusbarMain = new System.Windows.Forms.StatusStrip();
            this.tslActiveConnections = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lsbLog = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDisconectClient = new System.Windows.Forms.Button();
            this.btnRefeshClient = new System.Windows.Forms.Button();
            this.lblClientActivity = new System.Windows.Forms.Label();
            this.lblCLientConnectedTime = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblCLientIP = new System.Windows.Forms.Label();
            this.lblCLientID = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lvClients = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.statusbarMain.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnSetting);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.btnBrowseRoot);
            this.groupBox1.Controls.Add(this.txtRootFolder);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(716, 106);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server Control";
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnStop.Location = new System.Drawing.Point(383, 65);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(64, 25);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnSetting.Location = new System.Drawing.Point(474, 65);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(83, 25);
            this.btnSetting.TabIndex = 8;
            this.btnSetting.Text = "Setting";
            this.btnSetting.UseVisualStyleBackColor = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(583, 31);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(15, 16);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "_";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(536, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Status";
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnStart.Location = new System.Drawing.Point(294, 65);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(64, 25);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnBrowseRoot
            // 
            this.btnBrowseRoot.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnBrowseRoot.Location = new System.Drawing.Point(154, 65);
            this.btnBrowseRoot.Name = "btnBrowseRoot";
            this.btnBrowseRoot.Size = new System.Drawing.Size(113, 25);
            this.btnBrowseRoot.TabIndex = 4;
            this.btnBrowseRoot.Text = "Brownse Root";
            this.btnBrowseRoot.UseVisualStyleBackColor = false;
            this.btnBrowseRoot.Click += new System.EventHandler(this.btnBrowseRoot_Click);
            // 
            // txtRootFolder
            // 
            this.txtRootFolder.Location = new System.Drawing.Point(373, 28);
            this.txtRootFolder.Name = "txtRootFolder";
            this.txtRootFolder.Size = new System.Drawing.Size(138, 22);
            this.txtRootFolder.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(292, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Root Folder";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(127, 27);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(135, 22);
            this.txtPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 106);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(716, 299);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.statusbarMain);
            this.tabPage1.Controls.Add(this.lsbLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(708, 270);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabLog";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // statusbarMain
            // 
            this.statusbarMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslActiveConnections,
            this.toolStripStatusLabel2});
            this.statusbarMain.Location = new System.Drawing.Point(3, 245);
            this.statusbarMain.Name = "statusbarMain";
            this.statusbarMain.Size = new System.Drawing.Size(702, 22);
            this.statusbarMain.TabIndex = 1;
            this.statusbarMain.Text = "statusStrip1";
            // 
            // tslActiveConnections
            // 
            this.tslActiveConnections.Name = "tslActiveConnections";
            this.tslActiveConnections.Size = new System.Drawing.Size(113, 17);
            this.tslActiveConnections.Text = "Active Connections:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(122, 17);
            this.toolStripStatusLabel2.Text = "Total Files Transferred:";
            // 
            // lsbLog
            // 
            this.lsbLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.lsbLog.FormattingEnabled = true;
            this.lsbLog.ItemHeight = 16;
            this.lsbLog.Location = new System.Drawing.Point(3, 3);
            this.lsbLog.Name = "lsbLog";
            this.lsbLog.Size = new System.Drawing.Size(702, 228);
            this.lsbLog.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.lvClients);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(708, 270);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabClient";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDisconectClient);
            this.groupBox2.Controls.Add(this.btnRefeshClient);
            this.groupBox2.Controls.Add(this.lblClientActivity);
            this.groupBox2.Controls.Add(this.lblCLientConnectedTime);
            this.groupBox2.Controls.Add(this.lblUser);
            this.groupBox2.Controls.Add(this.lblCLientIP);
            this.groupBox2.Controls.Add(this.lblCLientID);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(3, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(702, 100);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Client Info";
            // 
            // btnDisconectClient
            // 
            this.btnDisconectClient.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnDisconectClient.Location = new System.Drawing.Point(500, 66);
            this.btnDisconectClient.Name = "btnDisconectClient";
            this.btnDisconectClient.Size = new System.Drawing.Size(175, 25);
            this.btnDisconectClient.TabIndex = 14;
            this.btnDisconectClient.Text = "Disconnect Selected Client";
            this.btnDisconectClient.UseVisualStyleBackColor = false;
            // 
            // btnRefeshClient
            // 
            this.btnRefeshClient.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnRefeshClient.Location = new System.Drawing.Point(500, 27);
            this.btnRefeshClient.Name = "btnRefeshClient";
            this.btnRefeshClient.Size = new System.Drawing.Size(113, 25);
            this.btnRefeshClient.TabIndex = 9;
            this.btnRefeshClient.Text = "Refesh List";
            this.btnRefeshClient.UseVisualStyleBackColor = false;
            // 
            // lblClientActivity
            // 
            this.lblClientActivity.AutoSize = true;
            this.lblClientActivity.Location = new System.Drawing.Point(357, 61);
            this.lblClientActivity.Name = "lblClientActivity";
            this.lblClientActivity.Size = new System.Drawing.Size(15, 16);
            this.lblClientActivity.TabIndex = 13;
            this.lblClientActivity.Text = "_";
            // 
            // lblCLientConnectedTime
            // 
            this.lblCLientConnectedTime.AutoSize = true;
            this.lblCLientConnectedTime.Location = new System.Drawing.Point(357, 36);
            this.lblCLientConnectedTime.Name = "lblCLientConnectedTime";
            this.lblCLientConnectedTime.Size = new System.Drawing.Size(15, 16);
            this.lblCLientConnectedTime.TabIndex = 12;
            this.lblCLientConnectedTime.Text = "_";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(110, 70);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(15, 16);
            this.lblUser.TabIndex = 11;
            this.lblUser.Text = "_";
            // 
            // lblCLientIP
            // 
            this.lblCLientIP.AutoSize = true;
            this.lblCLientIP.Location = new System.Drawing.Point(110, 47);
            this.lblCLientIP.Name = "lblCLientIP";
            this.lblCLientIP.Size = new System.Drawing.Size(15, 16);
            this.lblCLientIP.TabIndex = 10;
            this.lblCLientIP.Text = "_";
            // 
            // lblCLientID
            // 
            this.lblCLientID.AutoSize = true;
            this.lblCLientID.Location = new System.Drawing.Point(110, 22);
            this.lblCLientID.Name = "lblCLientID";
            this.lblCLientID.Size = new System.Drawing.Size(15, 16);
            this.lblCLientID.TabIndex = 9;
            this.lblCLientID.Text = "_";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(260, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 16);
            this.label8.TabIndex = 4;
            this.label8.Text = "Last Activity :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(260, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 16);
            this.label7.TabIndex = 3;
            this.label7.Text = "Connected :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "User :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "IP :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Client ID :";
            // 
            // lvClients
            // 
            this.lvClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvClients.FullRowSelect = true;
            this.lvClients.GridLines = true;
            this.lvClients.HideSelection = false;
            this.lvClients.Location = new System.Drawing.Point(3, 3);
            this.lvClients.Name = "lvClients";
            this.lvClients.Size = new System.Drawing.Size(702, 264);
            this.lvClients.TabIndex = 0;
            this.lvClients.UseCompatibleStateImageBehavior = false;
            this.lvClients.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Client ID";
            this.columnHeader1.Width = 81;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "IP Address";
            this.columnHeader2.Width = 95;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "User Name";
            this.columnHeader3.Width = 85;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Connected Since";
            this.columnHeader4.Width = 128;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Status";
            this.columnHeader5.Width = 97;
            // 
            // FtpServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(716, 405);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FtpServerForm";
            this.Text = "FTP Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FtpServerForm_FormClosing);
            this.Load += new System.EventHandler(this.FtpServerForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.statusbarMain.ResumeLayout(false);
            this.statusbarMain.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnBrowseRoot;
        private System.Windows.Forms.TextBox txtRootFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.StatusStrip statusbarMain;
        private System.Windows.Forms.ToolStripStatusLabel tslActiveConnections;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ListBox lsbLog;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDisconectClient;
        private System.Windows.Forms.Button btnRefeshClient;
        private System.Windows.Forms.Label lblClientActivity;
        private System.Windows.Forms.Label lblCLientConnectedTime;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblCLientIP;
        private System.Windows.Forms.Label lblCLientID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView lvClients;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button btnStop;
    }
}

