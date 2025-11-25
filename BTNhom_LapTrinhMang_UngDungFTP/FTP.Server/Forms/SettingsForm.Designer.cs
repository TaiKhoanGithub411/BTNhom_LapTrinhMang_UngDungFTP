
namespace FTP.Server.Forms
{
    partial class SettingsForm
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
            this.groupBoxUser = new System.Windows.Forms.GroupBox();
            this.lvUsers = new System.Windows.Forms.ListView();
            this.clName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clHomeDir = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDeleteUser = new System.Windows.Forms.Button();
            this.tabUsers = new System.Windows.Forms.TabPage();
            this.groupBoxUserDetail = new System.Windows.Forms.GroupBox();
            this.lbPass = new System.Windows.Forms.Label();
            this.btnCancelUser = new System.Windows.Forms.Button();
            this.txtHomeDir = new System.Windows.Forms.TextBox();
            this.btnSaveUser = new System.Windows.Forms.Button();
            this.btnBrowseHome = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.tabSetting = new System.Windows.Forms.TabControl();
            this.tabPermission = new System.Windows.Forms.TabPage();
            this.groupBoxDirectoryPermission = new System.Windows.Forms.GroupBox();
            this.chkDeleteDir = new System.Windows.Forms.CheckBox();
            this.chkCreateDir = new System.Windows.Forms.CheckBox();
            this.groupBoxFilePermission = new System.Windows.Forms.GroupBox();
            this.chkDelete = new System.Windows.Forms.CheckBox();
            this.chkWrite = new System.Windows.Forms.CheckBox();
            this.chkRead = new System.Windows.Forms.CheckBox();
            this.cbbUserPermission = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.groupBoxIPFilltering = new System.Windows.Forms.GroupBox();
            this.btnRemoveIP = new System.Windows.Forms.Button();
            this.btnAddIP = new System.Windows.Forms.Button();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lstBannedIPs = new System.Windows.Forms.ListBox();
            this.rdDenySelected = new System.Windows.Forms.RadioButton();
            this.rdAllowAll = new System.Windows.Forms.RadioButton();
            this.groupBoxConnectLimit = new System.Windows.Forms.GroupBox();
            this.numLoginTimeOut = new System.Windows.Forms.NumericUpDown();
            this.numMaxConnsPerUser = new System.Windows.Forms.NumericUpDown();
            this.numMaxConnections = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBoxUser.SuspendLayout();
            this.tabUsers.SuspendLayout();
            this.groupBoxUserDetail.SuspendLayout();
            this.tabSetting.SuspendLayout();
            this.tabPermission.SuspendLayout();
            this.groupBoxDirectoryPermission.SuspendLayout();
            this.groupBoxFilePermission.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.groupBoxIPFilltering.SuspendLayout();
            this.groupBoxConnectLimit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLoginTimeOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConnsPerUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConnections)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxUser
            // 
            this.groupBoxUser.Controls.Add(this.lvUsers);
            this.groupBoxUser.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxUser.Location = new System.Drawing.Point(3, 6);
            this.groupBoxUser.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBoxUser.Name = "groupBoxUser";
            this.groupBoxUser.Padding = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBoxUser.Size = new System.Drawing.Size(736, 228);
            this.groupBoxUser.TabIndex = 0;
            this.groupBoxUser.TabStop = false;
            this.groupBoxUser.Text = "User Management";
            // 
            // lvUsers
            // 
            this.lvUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clName,
            this.clHomeDir});
            this.lvUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.lvUsers.GridLines = true;
            this.lvUsers.HideSelection = false;
            this.lvUsers.Location = new System.Drawing.Point(3, 26);
            this.lvUsers.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.lvUsers.Name = "lvUsers";
            this.lvUsers.Size = new System.Drawing.Size(730, 180);
            this.lvUsers.TabIndex = 0;
            this.lvUsers.UseCompatibleStateImageBehavior = false;
            this.lvUsers.View = System.Windows.Forms.View.Details;
            this.lvUsers.SelectedIndexChanged += new System.EventHandler(this.lvUsers_SelectedIndexChanged);
            // 
            // clName
            // 
            this.clName.Text = "Name";
            this.clName.Width = 153;
            // 
            // clHomeDir
            // 
            this.clHomeDir.Text = "Home directory";
            this.clHomeDir.Width = 347;
            // 
            // btnDeleteUser
            // 
            this.btnDeleteUser.Location = new System.Drawing.Point(343, 206);
            this.btnDeleteUser.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnDeleteUser.Name = "btnDeleteUser";
            this.btnDeleteUser.Size = new System.Drawing.Size(120, 47);
            this.btnDeleteUser.TabIndex = 10;
            this.btnDeleteUser.Text = "Delete User";
            this.btnDeleteUser.UseVisualStyleBackColor = true;
            this.btnDeleteUser.Click += new System.EventHandler(this.btnDeleteUser_Click);
            // 
            // tabUsers
            // 
            this.tabUsers.Controls.Add(this.groupBoxUserDetail);
            this.tabUsers.Controls.Add(this.groupBoxUser);
            this.tabUsers.Location = new System.Drawing.Point(4, 31);
            this.tabUsers.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tabUsers.Name = "tabUsers";
            this.tabUsers.Padding = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tabUsers.Size = new System.Drawing.Size(742, 527);
            this.tabUsers.TabIndex = 0;
            this.tabUsers.Text = "Users";
            this.tabUsers.UseVisualStyleBackColor = true;
            // 
            // groupBoxUserDetail
            // 
            this.groupBoxUserDetail.Controls.Add(this.lbPass);
            this.groupBoxUserDetail.Controls.Add(this.btnDeleteUser);
            this.groupBoxUserDetail.Controls.Add(this.btnCancelUser);
            this.groupBoxUserDetail.Controls.Add(this.txtHomeDir);
            this.groupBoxUserDetail.Controls.Add(this.btnSaveUser);
            this.groupBoxUserDetail.Controls.Add(this.btnBrowseHome);
            this.groupBoxUserDetail.Controls.Add(this.txtPassword);
            this.groupBoxUserDetail.Controls.Add(this.label3);
            this.groupBoxUserDetail.Controls.Add(this.label2);
            this.groupBoxUserDetail.Controls.Add(this.label1);
            this.groupBoxUserDetail.Controls.Add(this.txtUsername);
            this.groupBoxUserDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxUserDetail.Location = new System.Drawing.Point(3, 243);
            this.groupBoxUserDetail.Name = "groupBoxUserDetail";
            this.groupBoxUserDetail.Size = new System.Drawing.Size(736, 278);
            this.groupBoxUserDetail.TabIndex = 1;
            this.groupBoxUserDetail.TabStop = false;
            this.groupBoxUserDetail.Text = "User Detail";
            // 
            // lbPass
            // 
            this.lbPass.AutoSize = true;
            this.lbPass.Location = new System.Drawing.Point(526, 95);
            this.lbPass.Name = "lbPass";
            this.lbPass.Size = new System.Drawing.Size(14, 22);
            this.lbPass.TabIndex = 14;
            this.lbPass.Text = ".";
            // 
            // btnCancelUser
            // 
            this.btnCancelUser.Location = new System.Drawing.Point(514, 206);
            this.btnCancelUser.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnCancelUser.Name = "btnCancelUser";
            this.btnCancelUser.Size = new System.Drawing.Size(120, 47);
            this.btnCancelUser.TabIndex = 13;
            this.btnCancelUser.Text = "Reset";
            this.btnCancelUser.UseVisualStyleBackColor = true;
            this.btnCancelUser.Click += new System.EventHandler(this.btnCancelUser_Click);
            // 
            // txtHomeDir
            // 
            this.txtHomeDir.Location = new System.Drawing.Point(165, 140);
            this.txtHomeDir.Name = "txtHomeDir";
            this.txtHomeDir.Size = new System.Drawing.Size(425, 27);
            this.txtHomeDir.TabIndex = 5;
            // 
            // btnSaveUser
            // 
            this.btnSaveUser.Location = new System.Drawing.Point(188, 206);
            this.btnSaveUser.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnSaveUser.Name = "btnSaveUser";
            this.btnSaveUser.Size = new System.Drawing.Size(117, 47);
            this.btnSaveUser.TabIndex = 12;
            this.btnSaveUser.Text = "Save User";
            this.btnSaveUser.UseVisualStyleBackColor = true;
            this.btnSaveUser.Click += new System.EventHandler(this.btnSaveUser_Click);
            // 
            // btnBrowseHome
            // 
            this.btnBrowseHome.Location = new System.Drawing.Point(24, 206);
            this.btnBrowseHome.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnBrowseHome.Name = "btnBrowseHome";
            this.btnBrowseHome.Size = new System.Drawing.Size(117, 47);
            this.btnBrowseHome.TabIndex = 11;
            this.btnBrowseHome.Text = "Browse Home";
            this.btnBrowseHome.UseVisualStyleBackColor = true;
            this.btnBrowseHome.Click += new System.EventHandler(this.btnBrowseHome_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(165, 103);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(425, 27);
            this.txtPassword.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 22);
            this.label3.TabIndex = 3;
            this.label3.Text = "Home Dir";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 22);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "UserName";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(165, 65);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(425, 27);
            this.txtUsername.TabIndex = 0;
            // 
            // tabSetting
            // 
            this.tabSetting.Controls.Add(this.tabUsers);
            this.tabSetting.Controls.Add(this.tabPermission);
            this.tabSetting.Controls.Add(this.tabAdvanced);
            this.tabSetting.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabSetting.Location = new System.Drawing.Point(0, 0);
            this.tabSetting.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tabSetting.Name = "tabSetting";
            this.tabSetting.SelectedIndex = 0;
            this.tabSetting.Size = new System.Drawing.Size(750, 562);
            this.tabSetting.TabIndex = 4;
            // 
            // tabPermission
            // 
            this.tabPermission.Controls.Add(this.groupBoxDirectoryPermission);
            this.tabPermission.Controls.Add(this.groupBoxFilePermission);
            this.tabPermission.Controls.Add(this.cbbUserPermission);
            this.tabPermission.Controls.Add(this.label4);
            this.tabPermission.Location = new System.Drawing.Point(4, 31);
            this.tabPermission.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tabPermission.Name = "tabPermission";
            this.tabPermission.Padding = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tabPermission.Size = new System.Drawing.Size(742, 527);
            this.tabPermission.TabIndex = 1;
            this.tabPermission.Text = "Permission";
            this.tabPermission.UseVisualStyleBackColor = true;
            // 
            // groupBoxDirectoryPermission
            // 
            this.groupBoxDirectoryPermission.Controls.Add(this.chkDeleteDir);
            this.groupBoxDirectoryPermission.Controls.Add(this.chkCreateDir);
            this.groupBoxDirectoryPermission.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxDirectoryPermission.Location = new System.Drawing.Point(3, 306);
            this.groupBoxDirectoryPermission.Name = "groupBoxDirectoryPermission";
            this.groupBoxDirectoryPermission.Size = new System.Drawing.Size(736, 215);
            this.groupBoxDirectoryPermission.TabIndex = 3;
            this.groupBoxDirectoryPermission.TabStop = false;
            this.groupBoxDirectoryPermission.Text = "Directory Permisson";
            // 
            // chkDeleteDir
            // 
            this.chkDeleteDir.AutoSize = true;
            this.chkDeleteDir.Location = new System.Drawing.Point(57, 130);
            this.chkDeleteDir.Name = "chkDeleteDir";
            this.chkDeleteDir.Size = new System.Drawing.Size(132, 26);
            this.chkDeleteDir.TabIndex = 13;
            this.chkDeleteDir.Text = "Delete Directory";
            this.chkDeleteDir.UseVisualStyleBackColor = true;
            // 
            // chkCreateDir
            // 
            this.chkCreateDir.AutoSize = true;
            this.chkCreateDir.Location = new System.Drawing.Point(57, 69);
            this.chkCreateDir.Name = "chkCreateDir";
            this.chkCreateDir.Size = new System.Drawing.Size(134, 26);
            this.chkCreateDir.TabIndex = 12;
            this.chkCreateDir.Text = "Create Directory";
            this.chkCreateDir.UseVisualStyleBackColor = true;
            // 
            // groupBoxFilePermission
            // 
            this.groupBoxFilePermission.Controls.Add(this.chkDelete);
            this.groupBoxFilePermission.Controls.Add(this.chkWrite);
            this.groupBoxFilePermission.Controls.Add(this.chkRead);
            this.groupBoxFilePermission.Location = new System.Drawing.Point(3, 128);
            this.groupBoxFilePermission.Name = "groupBoxFilePermission";
            this.groupBoxFilePermission.Size = new System.Drawing.Size(871, 172);
            this.groupBoxFilePermission.TabIndex = 2;
            this.groupBoxFilePermission.TabStop = false;
            this.groupBoxFilePermission.Text = "File Permisson";
            // 
            // chkDelete
            // 
            this.chkDelete.AutoSize = true;
            this.chkDelete.Location = new System.Drawing.Point(456, 78);
            this.chkDelete.Name = "chkDelete";
            this.chkDelete.Size = new System.Drawing.Size(72, 26);
            this.chkDelete.TabIndex = 11;
            this.chkDelete.Text = "Delete";
            this.chkDelete.UseVisualStyleBackColor = true;
            // 
            // chkWrite
            // 
            this.chkWrite.AutoSize = true;
            this.chkWrite.Location = new System.Drawing.Point(249, 78);
            this.chkWrite.Name = "chkWrite";
            this.chkWrite.Size = new System.Drawing.Size(64, 26);
            this.chkWrite.TabIndex = 9;
            this.chkWrite.Text = "Write";
            this.chkWrite.UseVisualStyleBackColor = true;
            // 
            // chkRead
            // 
            this.chkRead.AutoSize = true;
            this.chkRead.Location = new System.Drawing.Point(57, 78);
            this.chkRead.Name = "chkRead";
            this.chkRead.Size = new System.Drawing.Size(66, 26);
            this.chkRead.TabIndex = 8;
            this.chkRead.Text = "Read";
            this.chkRead.UseVisualStyleBackColor = true;
            // 
            // cbbUserPermission
            // 
            this.cbbUserPermission.FormattingEnabled = true;
            this.cbbUserPermission.Location = new System.Drawing.Point(143, 35);
            this.cbbUserPermission.Name = "cbbUserPermission";
            this.cbbUserPermission.Size = new System.Drawing.Size(198, 30);
            this.cbbUserPermission.TabIndex = 1;
            this.cbbUserPermission.SelectedIndexChanged += new System.EventHandler(this.cbbUserPermission_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 22);
            this.label4.TabIndex = 0;
            this.label4.Text = "Select User";
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.Controls.Add(this.groupBoxIPFilltering);
            this.tabAdvanced.Controls.Add(this.groupBoxConnectLimit);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 31);
            this.tabAdvanced.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Size = new System.Drawing.Size(742, 527);
            this.tabAdvanced.TabIndex = 2;
            this.tabAdvanced.Text = "Advanced";
            this.tabAdvanced.UseVisualStyleBackColor = true;
            // 
            // groupBoxIPFilltering
            // 
            this.groupBoxIPFilltering.Controls.Add(this.btnRemoveIP);
            this.groupBoxIPFilltering.Controls.Add(this.btnAddIP);
            this.groupBoxIPFilltering.Controls.Add(this.txtIP);
            this.groupBoxIPFilltering.Controls.Add(this.label8);
            this.groupBoxIPFilltering.Controls.Add(this.lstBannedIPs);
            this.groupBoxIPFilltering.Controls.Add(this.rdDenySelected);
            this.groupBoxIPFilltering.Controls.Add(this.rdAllowAll);
            this.groupBoxIPFilltering.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxIPFilltering.Location = new System.Drawing.Point(0, 210);
            this.groupBoxIPFilltering.Name = "groupBoxIPFilltering";
            this.groupBoxIPFilltering.Size = new System.Drawing.Size(742, 317);
            this.groupBoxIPFilltering.TabIndex = 1;
            this.groupBoxIPFilltering.TabStop = false;
            this.groupBoxIPFilltering.Text = "IP Filltering";
            // 
            // btnRemoveIP
            // 
            this.btnRemoveIP.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRemoveIP.Location = new System.Drawing.Point(520, 253);
            this.btnRemoveIP.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnRemoveIP.Name = "btnRemoveIP";
            this.btnRemoveIP.Size = new System.Drawing.Size(81, 37);
            this.btnRemoveIP.TabIndex = 9;
            this.btnRemoveIP.Text = "Remove IP";
            this.btnRemoveIP.UseVisualStyleBackColor = true;
            this.btnRemoveIP.Click += new System.EventHandler(this.btnRemoveIP_Click);
            // 
            // btnAddIP
            // 
            this.btnAddIP.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAddIP.Location = new System.Drawing.Point(403, 253);
            this.btnAddIP.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnAddIP.Name = "btnAddIP";
            this.btnAddIP.Size = new System.Drawing.Size(81, 37);
            this.btnAddIP.TabIndex = 8;
            this.btnAddIP.Text = "Add IP";
            this.btnAddIP.UseVisualStyleBackColor = true;
            this.btnAddIP.Click += new System.EventHandler(this.btnAddIP_Click);
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(118, 257);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(230, 27);
            this.txtIP.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(72, 260);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(22, 22);
            this.label8.TabIndex = 6;
            this.label8.Text = "IP";
            // 
            // lstBannedIPs
            // 
            this.lstBannedIPs.FormattingEnabled = true;
            this.lstBannedIPs.ItemHeight = 22;
            this.lstBannedIPs.Location = new System.Drawing.Point(62, 102);
            this.lstBannedIPs.Name = "lstBannedIPs";
            this.lstBannedIPs.Size = new System.Drawing.Size(539, 114);
            this.lstBannedIPs.TabIndex = 2;
            // 
            // rdDenySelected
            // 
            this.rdDenySelected.AutoSize = true;
            this.rdDenySelected.Location = new System.Drawing.Point(62, 70);
            this.rdDenySelected.Name = "rdDenySelected";
            this.rdDenySelected.Size = new System.Drawing.Size(169, 26);
            this.rdDenySelected.TabIndex = 1;
            this.rdDenySelected.TabStop = true;
            this.rdDenySelected.Text = "Deny connection from";
            this.rdDenySelected.UseVisualStyleBackColor = true;
            // 
            // rdAllowAll
            // 
            this.rdAllowAll.AutoSize = true;
            this.rdAllowAll.Location = new System.Drawing.Point(62, 38);
            this.rdAllowAll.Name = "rdAllowAll";
            this.rdAllowAll.Size = new System.Drawing.Size(160, 26);
            this.rdAllowAll.TabIndex = 0;
            this.rdAllowAll.TabStop = true;
            this.rdAllowAll.Text = "Allow all connections";
            this.rdAllowAll.UseVisualStyleBackColor = true;
            // 
            // groupBoxConnectLimit
            // 
            this.groupBoxConnectLimit.Controls.Add(this.numLoginTimeOut);
            this.groupBoxConnectLimit.Controls.Add(this.numMaxConnsPerUser);
            this.groupBoxConnectLimit.Controls.Add(this.numMaxConnections);
            this.groupBoxConnectLimit.Controls.Add(this.label7);
            this.groupBoxConnectLimit.Controls.Add(this.label6);
            this.groupBoxConnectLimit.Controls.Add(this.label5);
            this.groupBoxConnectLimit.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxConnectLimit.Location = new System.Drawing.Point(0, 0);
            this.groupBoxConnectLimit.Name = "groupBoxConnectLimit";
            this.groupBoxConnectLimit.Size = new System.Drawing.Size(742, 204);
            this.groupBoxConnectLimit.TabIndex = 0;
            this.groupBoxConnectLimit.TabStop = false;
            this.groupBoxConnectLimit.Text = "Connection Limits";
            // 
            // numLoginTimeOut
            // 
            this.numLoginTimeOut.Location = new System.Drawing.Point(247, 137);
            this.numLoginTimeOut.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numLoginTimeOut.Name = "numLoginTimeOut";
            this.numLoginTimeOut.Size = new System.Drawing.Size(380, 27);
            this.numLoginTimeOut.TabIndex = 5;
            // 
            // numMaxConnsPerUser
            // 
            this.numMaxConnsPerUser.Location = new System.Drawing.Point(247, 88);
            this.numMaxConnsPerUser.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numMaxConnsPerUser.Name = "numMaxConnsPerUser";
            this.numMaxConnsPerUser.Size = new System.Drawing.Size(380, 27);
            this.numMaxConnsPerUser.TabIndex = 4;
            // 
            // numMaxConnections
            // 
            this.numMaxConnections.Location = new System.Drawing.Point(247, 42);
            this.numMaxConnections.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numMaxConnections.Name = "numMaxConnections";
            this.numMaxConnections.Size = new System.Drawing.Size(380, 27);
            this.numMaxConnections.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(129, 22);
            this.label7.TabIndex = 2;
            this.label7.Text = "Login Timout (sec)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 22);
            this.label6.TabIndex = 1;
            this.label6.Text = "Max Conns per User";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 22);
            this.label5.TabIndex = 0;
            this.label5.Text = "Max Connections";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.Location = new System.Drawing.Point(450, 574);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 47);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.Location = new System.Drawing.Point(312, 574);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(95, 47);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnApply.Location = new System.Drawing.Point(172, 574);
            this.btnApply.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(95, 47);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 636);
            this.Controls.Add(this.tabSetting);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnApply);
            this.Font = new System.Drawing.Font("Arial Narrow", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.groupBoxUser.ResumeLayout(false);
            this.tabUsers.ResumeLayout(false);
            this.groupBoxUserDetail.ResumeLayout(false);
            this.groupBoxUserDetail.PerformLayout();
            this.tabSetting.ResumeLayout(false);
            this.tabPermission.ResumeLayout(false);
            this.tabPermission.PerformLayout();
            this.groupBoxDirectoryPermission.ResumeLayout(false);
            this.groupBoxDirectoryPermission.PerformLayout();
            this.groupBoxFilePermission.ResumeLayout(false);
            this.groupBoxFilePermission.PerformLayout();
            this.tabAdvanced.ResumeLayout(false);
            this.groupBoxIPFilltering.ResumeLayout(false);
            this.groupBoxIPFilltering.PerformLayout();
            this.groupBoxConnectLimit.ResumeLayout(false);
            this.groupBoxConnectLimit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLoginTimeOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConnsPerUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConnections)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxUser;
        private System.Windows.Forms.ListView lvUsers;
        private System.Windows.Forms.TabPage tabUsers;
        private System.Windows.Forms.TabControl tabSetting;
        private System.Windows.Forms.TabPage tabPermission;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnDeleteUser;
        private System.Windows.Forms.GroupBox groupBoxUserDetail;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtHomeDir;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancelUser;
        private System.Windows.Forms.Button btnSaveUser;
        private System.Windows.Forms.Button btnBrowseHome;
        private System.Windows.Forms.ComboBox cbbUserPermission;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBoxDirectoryPermission;
        private System.Windows.Forms.GroupBox groupBoxFilePermission;
        private System.Windows.Forms.CheckBox chkDeleteDir;
        private System.Windows.Forms.CheckBox chkCreateDir;
        private System.Windows.Forms.CheckBox chkDelete;
        private System.Windows.Forms.CheckBox chkWrite;
        private System.Windows.Forms.CheckBox chkRead;
        private System.Windows.Forms.GroupBox groupBoxIPFilltering;
        private System.Windows.Forms.GroupBox groupBoxConnectLimit;
        private System.Windows.Forms.NumericUpDown numMaxConnections;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numLoginTimeOut;
        private System.Windows.Forms.NumericUpDown numMaxConnsPerUser;
        private System.Windows.Forms.RadioButton rdDenySelected;
        private System.Windows.Forms.RadioButton rdAllowAll;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListBox lstBannedIPs;
        private System.Windows.Forms.Button btnRemoveIP;
        private System.Windows.Forms.Button btnAddIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lbPass;
        private System.Windows.Forms.ColumnHeader clName;
        private System.Windows.Forms.ColumnHeader clHomeDir;
    }
}