using System;
using System.Drawing;
using System.Windows.Forms;
using FTP.Core.Server;
using FTP.Core.Enum;

namespace FTP.Server
{
    public partial class FtpServerForm : Form
    {
        private ServerConfiguration _config;
        private SessionManager _sessionManager;
        private FtpServer _ftpServer;

        public FtpServerForm()
        {
            InitializeComponent();
        }

        private void FtpServerForm_Load(object sender, EventArgs e)
        {
            // Khởi tạo các đối tượng
            _config = ServerConfiguration.CreateDefault();
            _sessionManager = new SessionManager();
            _ftpServer = new FtpServer(_config, _sessionManager);

            // Subscribe vào events của FtpServer
            _ftpServer.LogMessage += OnLogMessage;
            _ftpServer.StatusChanged += OnStatusChanged;

            // Subscribe vào events của SessionManager
            _sessionManager.SessionAdded += OnSessionAdded;
            _sessionManager.SessionRemoved += OnSessionRemoved;

            // Thiết lập giá trị mặc định cho controls
            txtPort.Text = _config.Port.ToString();
            txtRootFolder.Text = _config.RootFolder;
            lblStatus.Text = "Stopped";
            lblStatus.ForeColor = Color.Red;

            // Cấu hình ListView cho hiển thị clients
            SetupClientListView();
        }

        /// Cấu hình ListView để hiển thị danh sách clients.
        private void SetupClientListView()
        {
            lvClients.View = View.Details;
            lvClients.FullRowSelect = true;
            lvClients.GridLines = true;

            // Thêm các cột theo thiết kế
            lvClients.Columns.Clear();
            lvClients.Columns.Add("Client ID", 100);
            lvClients.Columns.Add("IP Address", 120);
            lvClients.Columns.Add("User Name", 100);
            lvClients.Columns.Add("Connected Since", 150);
            lvClients.Columns.Add("Status", 100);
        }
        /// Xử lý khi có log message mới từ server.
        private void OnLogMessage(string message)
        {
            // Kiểm tra cross-thread call
            if (lsbLog.InvokeRequired)
            {
                lsbLog.Invoke(new Action(() => OnLogMessage(message)));
                return;
            }

            // Thêm message vào ListBox với timestamp
            string logEntry = $"[{DateTime.Now:HH:mm}] {message}";
            lsbLog.Items.Add(logEntry);

            // Auto-scroll xuống dưới cùng
            lsbLog.TopIndex = lsbLog.Items.Count - 1;

            // Giới hạn số dòng log (tránh tràn bộ nhớ)
            if (lsbLog.Items.Count > 1000)
            {
                lsbLog.Items.RemoveAt(0);
            }
        }
        /// Xử lý khi trạng thái server thay đổi.
        private void OnStatusChanged(ServerStatus status)
        {
            // Kiểm tra cross-thread call
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => OnStatusChanged(status)));
                return;
            }

            // Cập nhật label status
            lblStatus.Text = status.ToString();

            // Thay đổi màu sắc theo trạng thái
            switch (status)
            {
                case ServerStatus.Running:
                    lblStatus.ForeColor = Color.Green;
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    txtPort.Enabled = false;
                    txtRootFolder.Enabled = false;
                    btnBrowseRoot.Enabled = false;
                    break;

                case ServerStatus.Stopped:
                    lblStatus.ForeColor = Color.Red;
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    txtPort.Enabled = true;
                    txtRootFolder.Enabled = true;
                    btnBrowseRoot.Enabled = true;
                    break;

                case ServerStatus.Starting:
                    lblStatus.ForeColor = Color.Orange;
                    btnStart.Enabled = false;
                    btnStop.Enabled = false;
                    break;

                case ServerStatus.Stopping:
                    lblStatus.ForeColor = Color.Orange;
                    btnStart.Enabled = false;
                    btnStop.Enabled = false;
                    break;
            }
        }
        /// Xử lý khi có session mới được thêm (client kết nối).
        private void OnSessionAdded(ClientSession session)
        {
            // Kiểm tra cross-thread call
            if (lvClients.InvokeRequired)
            {
                lvClients.Invoke(new Action(() => OnSessionAdded(session)));
                return;
            }

            // Tạo ListViewItem mới với dữ liệu từ session
            ListViewItem item = new ListViewItem(session.SessionId);
            item.SubItems.Add(session.ClientIPAddress);
            item.SubItems.Add(session.Username ?? "(not logged in)");
            item.SubItems.Add(session.ConnectedTime.ToString("HH:mm:ss"));
            item.SubItems.Add(session.Status.ToString());

            // Gắn Tag để có thể update sau này
            item.Tag = session.SessionId;

            // Thêm vào ListView
            lvClients.Items.Add(item);

            // Cập nhật status bar (nếu có)
            UpdateStatusBar();
        }
        /// Xử lý khi có session bị xóa (client ngắt kết nối).
        private void OnSessionRemoved(string sessionId)
        {
            // Kiểm tra cross-thread call
            if (lvClients.InvokeRequired)
            {
                lvClients.Invoke(new Action(() => OnSessionRemoved(sessionId)));
                return;
            }

            // Tìm và xóa item tương ứng trong ListView
            foreach (ListViewItem item in lvClients.Items)
            {
                if (item.Tag != null && item.Tag.ToString() == sessionId)
                {
                    lvClients.Items.Remove(item);
                    break;
                }
            }

            // Cập nhật status bar (nếu có)
            UpdateStatusBar();
        }
        /// Cập nhật status bar với số lượng kết nối hiện tại.
        private void UpdateStatusBar()
        {
            if (statusbarMain.InvokeRequired)
            {
                statusbarMain.Invoke(new Action(UpdateStatusBar));
                return;
            }

            // Cập nhật số lượng active connections
            int activeConnections = _sessionManager.ActiveSessionCount;

            tslActiveConnections.Text = $"Active Connections: {activeConnections}";
        }

        /// Nút Start Server.
        private async void btnStart_Click(object sender, EventArgs e)
        {
            // Validate input
            if (!int.TryParse(txtPort.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Invalid port number. Please enter a value between 1 and 65535.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRootFolder.Text))
            {
                MessageBox.Show("Please select a root folder.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cập nhật cấu hình
            _config.Port = port;
            _config.RootFolder = txtRootFolder.Text;

            // Start server (async)
            await _ftpServer.StartAsync();
        }
        /// Nút Stop Server.
        private void btnStop_Click(object sender, EventArgs e)
        {
            // Xác nhận trước khi stop
            var result = MessageBox.Show("Are you sure you want to stop the server?\nAll active connections will be closed.",
                "Confirm Stop", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _ftpServer.Stop();

                // Xóa tất cả items trong ListView
                lvClients.Items.Clear();
            }
        }
        /// Nút Browse Root Folder.
        private void btnBrowseRoot_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select FTP Root Folder";
                dialog.SelectedPath = txtRootFolder.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtRootFolder.Text = dialog.SelectedPath;
                }
            }
        }

        /*/// Nút Refresh Client List (chức năng nâng cao).
        private void btnRefreshClient_Click(object sender, EventArgs e)
        {
            // Xóa và load lại danh sách
            lvClients.Items.Clear();

            var sessions = _sessionManager.GetAllSessions();
            foreach (var session in sessions)
            {
                OnSessionAdded(session);
            }
        }*/

        /*/// Nút Disconnect Selected Client (chức năng nâng cao).
        private void btnDisconnectClient_Click(object sender, EventArgs e)
        {
            if (lvClients.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a client to disconnect.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = lvClients.SelectedItems[0];
            string sessionId = selectedItem.Tag.ToString();

            var result = MessageBox.Show($"Are you sure you want to disconnect client {sessionId}?",
                "Confirm Disconnect", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool success = _sessionManager.DisconnectSession(sessionId);

                if (success)
                {
                    MessageBox.Show("Client disconnected successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to disconnect client.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }*/


        /// <summary>
        /// Xử lý khi form đóng.
        /// </summary>
        private void FtpServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Nếu server đang chạy, hỏi xác nhận
            if (_ftpServer.IsRunning)
            {
                var result = MessageBox.Show("Server is still running. Do you want to stop it and exit?",
                    "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _ftpServer.Stop();
                }
                else
                {
                    e.Cancel = true; // Hủy việc đóng form
                }
            }
        }
    }
}
