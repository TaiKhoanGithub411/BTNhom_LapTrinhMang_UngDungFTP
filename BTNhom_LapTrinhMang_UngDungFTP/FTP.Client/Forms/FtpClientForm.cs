using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTP.Client
{
    public partial class FtpClientForm : Form
    {
        private TcpClient _controlClient;      // socket control
        private NetworkStream _networkStream;  // stream gửi/nhận
        private StreamReader _reader;
        private StreamWriter _writer;
        private bool _isConnected = false;

        public FtpClientForm()
        {
            InitializeComponent();
            txtPass.UseSystemPasswordChar = true;
            LoadLocalDrives();
        }
        #region Connect
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_isConnected)
            {
                MessageBox.Show("Connecting... ");
                return;
            }

            string host = txtHost.Text.Trim();
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();

            if (!int.TryParse(txtPort.Text.Trim(), out int port))
            {
                MessageBox.Show("Port không hợp lệ");
                return;
            }

            try
            {
                _controlClient = new TcpClient();
                _controlClient.Connect(host, port);

                _networkStream = _controlClient.GetStream();
                _reader = new StreamReader(_networkStream, Encoding.ASCII);
                _writer = new StreamWriter(_networkStream, Encoding.ASCII)
                {
                    AutoFlush = true
                };

                // Nhận welcome (220)
                string welcome = _reader.ReadLine();

                // USER
                _writer.WriteLine("USER " + user);
                _reader.ReadLine();

                // PASS
                _writer.WriteLine("PASS " + pass);
                string response = _reader.ReadLine();

                if (!response.StartsWith("230"))
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu ");
                    CleanupConnection();
                    return;
                }

                _isConnected = true;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                lblTrasferStatus.Text = "Connected to " + host + ":" + port;
                LoadServerFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kết nối thất bại: " + ex.Message);
                CleanupConnection();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (!_isConnected) return;

            try
            {
                _writer.WriteLine("QUIT");
            }
            catch { }

            CleanupConnection();
            lblTrasferStatus.Text = "Disconnected.";

        }
        private void CleanupConnection()
        {
            _isConnected = false;

            try { _reader?.Close(); } catch { }
            try { _writer?.Close(); } catch { }
            try { _networkStream?.Close(); } catch { }
            try { _controlClient?.Close(); } catch { }

            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }
        #endregion

        #region Local file
        // Load các ổ đĩa
        private void LoadLocalDrives()
        {
            tvLocalFiles.Nodes.Clear();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) continue;

                TreeNode node = new TreeNode(drive.Name);
                node.Tag = drive.RootDirectory.FullName;
                node.Nodes.Add("..."); // node giả để expand
                tvLocalFiles.Nodes.Add(node);
            }

            tvLocalFiles.BeforeExpand += tvLocalFiles_BeforeExpand;
        }

        #endregion

        private void tvLocalFiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            node.Nodes.Clear();

            string path = node.Tag.ToString();

            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    TreeNode subDir = new TreeNode(Path.GetFileName(dir));
                    subDir.Tag = dir;
                    subDir.Nodes.Add("...");
                    node.Nodes.Add(subDir);
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    TreeNode fileNode = new TreeNode(Path.GetFileName(file));
                    fileNode.Tag = file;
                    node.Nodes.Add(fileNode);
                }
            }
            catch { }
        }

        private void btnDeleteLocal_Click(object sender, EventArgs e)
        {
            if (tvLocalFiles.SelectedNode == null) return;

            string path = tvLocalFiles.SelectedNode.Tag.ToString();

            if (Directory.Exists(path))
            {
                MessageBox.Show("Chỉ cho xoá file, không xoá thư mục ");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn chắc chắn muốn xoá file này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                File.Delete(path);
                tvLocalFiles.SelectedNode.Remove();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xoá file: " + ex.Message);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                MessageBox.Show("Chưa connect server ");
                return;
            }

            if (tvLocalFiles.SelectedNode == null) return;

            // Lấy đường dẫn file cục bộ cần upload
            string localPath = tvLocalFiles.SelectedNode.Tag.ToString();
            string fileName = Path.GetFileName(localPath); // Tên file trên server

            if (!File.Exists(localPath))
            {
                MessageBox.Show("Chỉ upload FILE, không phải thư mục.");
                return;
            }

            // MessageBox.Show("Sẵn sàng upload file:\n" + localPath); // Có thể bỏ dòng này

            // Bắt đầu quá trình upload
            try
            {
                // 1. Yêu cầu PASV (Passive Mode)
                _writer.WriteLine("PASV");

                // Đọc cho đến khi gặp dòng 227 (hoặc null)
                string pasvResponse = ReadExpectedResponse("227");

                if (!pasvResponse.StartsWith("227"))
                {
                    throw new Exception("Không nhận được phản hồi 227 cho lệnh PASV.");
                }

                // Phân tích phản hồi PASV để lấy IP và Port
                int start = pasvResponse.IndexOf('(');
                int end = pasvResponse.IndexOf(')');
                if (start == -1 || end == -1)
                {
                    throw new Exception("Phản hồi PASV không hợp lệ: " + pasvResponse);
                }
                string[] parts = pasvResponse.Substring(start + 1, end - start - 1).Split(',');

                string ip = $"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}";
                int port = int.Parse(parts[4]) * 256 + int.Parse(parts[5]);

                // Tạo Data Connection
                TcpClient dataClient = new TcpClient();
                dataClient.Connect(ip, port);

                // Cập nhật trạng thái
                lblTrasferStatus.Text = $"Uploading {fileName} to {ip}:{port}...";

                // 2. Gửi lệnh STOR (Store) trên Control Connection
                _writer.WriteLine("STOR " + fileName);

                // Đọc phản hồi 150 (File status okay, about to open data connection)
                string storResponse = _reader.ReadLine();
                if (!storResponse.StartsWith("150"))
                {
                    dataClient.Close();
                    throw new Exception("Không thể bắt đầu upload: " + storResponse);
                }

                // 3. Gửi dữ liệu file trên Data Connection
                using (NetworkStream dataStream = dataClient.GetStream())
                using (FileStream fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(dataStream); // Copy toàn bộ nội dung file sang Data Stream
                }

                dataClient.Close(); // Đóng Data Connection

                // 4. Đọc phản hồi 226 (Transfer complete) trên Control Connection
                string completeResponse = _reader.ReadLine();
                if (completeResponse.StartsWith("226"))
                {
                    MessageBox.Show($"Upload file **{fileName}** thành công!");
                    lblTrasferStatus.Text = $"Upload complete: {fileName}";
                    LoadServerFiles(); // Tải lại danh sách file trên server để thấy file vừa upload
                }
                else
                {
                    throw new Exception("Upload thất bại sau khi truyền dữ liệu: " + completeResponse);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi upload file: " + ex.Message);
                lblTrasferStatus.Text = "Upload failed.";
                CleanupConnection(); // Có thể ngắt kết nối nếu có lỗi nghiêm trọng
            }
        }


        #region Server file

        private void LoadServerFiles()
        {
            if (!_isConnected) return;

            try
            {
                lvServerFiles.Items.Clear();

                // 1. Yêu cầu PASV
                _writer.WriteLine("PASV");
                string pasvResponse = _reader.ReadLine();

                int start = pasvResponse.IndexOf('(');
                int end = pasvResponse.IndexOf(')');
                string[] parts = pasvResponse.Substring(start + 1, end - start - 1).Split(',');

                string ip = $"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}";
                int port = int.Parse(parts[4]) * 256 + int.Parse(parts[5]);

                TcpClient dataClient = new TcpClient();
                dataClient.Connect(ip, port);

                // 2. Gửi LIST
                _writer.WriteLine("LIST");

                using (NetworkStream dataStream = dataClient.GetStream())
                using (StreamReader dataReader = new StreamReader(dataStream))
                {
                    string line;
                    while ((line = dataReader.ReadLine()) != null)
                    {
                        ListViewItem item = new ListViewItem(line);
                        lvServerFiles.Items.Add(item);
                    }
                }

                dataClient.Close();
                _reader.ReadLine(); // 226 Transfer complete
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load file server: " + ex.Message);
            }
        }
        // Thêm phương thức này vào cuối file FtpClientForm.cs, trước dấu đóng } của class
        private string ReadExpectedResponse(string expectedCode)
        {
            string line;
            // Đọc tất cả các dòng cho đến khi tìm thấy dòng bắt đầu bằng mã trạng thái mong muốn
            while ((line = _reader.ReadLine()) != null)
            {
                // Phản hồi FTP nhiều dòng có thể sử dụng dấu '-' sau mã code (ví dụ: 227-)
                // hoặc kết thúc bằng dấu cách (ví dụ: 227 )
                if (line.StartsWith(expectedCode + " ") || line.StartsWith(expectedCode + "-"))
                {
                    return line;
                }
            }
            return ""; // Trả về chuỗi rỗng nếu không tìm thấy
        }
        #endregion

    }
}
