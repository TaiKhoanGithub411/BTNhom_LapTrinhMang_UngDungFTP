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
            txtPort.Text = "21";
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


            try
            {
                _writer.WriteLine("CWD /"); // Gửi lệnh CWD để chuyển đến thư mục gốc
                string cwdResponse = _reader.ReadLine();

                // Kiểm tra phản hồi CWD (thường là 250)
                if (!cwdResponse.StartsWith("250"))
                {
                    // Nếu không thể chuyển về thư mục gốc, báo lỗi và dừng upload
                    MessageBox.Show($"Lỗi: Không thể chuyển về thư mục gốc: {cwdResponse}", "Lỗi CWD");
                    return;
                }

                lblTrasferStatus.Text = "Chuyển về thư mục gốc thành công. Bắt đầu upload...";
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

                // *** DÒNG CODE ĐÃ SỬA ***
                string pasvResponse = ReadExpectedResponse("227");

                if (!pasvResponse.StartsWith("227"))
                {
                    throw new Exception("Không nhận được phản hồi 227 cho lệnh PASV khi tải file.");
                }
                // *** KẾT THÚC DÒNG CODE ĐÃ SỬA ***

                int start = pasvResponse.IndexOf('(');
                int end = pasvResponse.IndexOf(')');
                // Thêm kiểm tra lỗi (start == -1 || end == -1) để chắc chắn hơn
                if (start == -1 || end == -1)
                {
                    throw new Exception("Phản hồi PASV không hợp lệ.");
                }

                string[] parts = pasvResponse.Substring(start + 1, end - start - 1).Split(',');

                string ip = $"{parts[0]}.{parts[1]}.{parts[2]}.{parts[3]}";
                int port = int.Parse(parts[4]) * 256 + int.Parse(parts[5]);

                TcpClient dataClient = new TcpClient();
                dataClient.Connect(ip, port);

                // 2. Gửi LIST
                _writer.WriteLine("LIST");

                // Cần đọc phản hồi 150 trước khi đọc Data Stream
                string listResponse = _reader.ReadLine();
                if (!listResponse.StartsWith("150"))
                {
                    dataClient.Close();
                    throw new Exception("Không thể bắt đầu LIST: " + listResponse);
                }


                using (NetworkStream dataStream = dataClient.GetStream())
                using (StreamReader dataReader = new StreamReader(dataStream))
                {
                    string line;
                    while ((line = dataReader.ReadLine()) != null)
                    {
                        // giữ nguyên dòng LIST gốc để kiểm tra type later
                        string rawLine = line;

                        // tách từng phần (nghĩa là giống format UNIX ls -l)
                        string[] part = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (part.Length < 8) continue;

                        string fileName = part[part.Length - 1];
                        string fileSize = part[4];
                        string fileModified = $"{part[5]} {part[6]} {part[7]}";

                        ListViewItem item = new ListViewItem(fileName);
                        item.SubItems.Add(fileSize);
                        item.SubItems.Add(fileModified);

                        // LƯU DÒNG GỐC vào Tag để sau này kiểm tra có phải folder không
                        item.Tag = rawLine;

                        lvServerFiles.Items.Add(item);
                    }

                }

                dataClient.Close();

                ReadExpectedResponse("226"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load file server: " + ex.Message);
            }
        }


        private string ReadExpectedResponse(string expectedCode)
        {
            string line;
            while ((line = _reader.ReadLine()) != null)
            {
                if (line.StartsWith(expectedCode + " ") || line.StartsWith(expectedCode + "-"))
                {
                    return line;
                }
            }
            return ""; 
        }

        private bool IsDirectory(string listLine)
        {
            return listLine.StartsWith("d") || listLine.Contains("<DIR>");
        }

        private string GetNameFromListLine(string listLine)
        {
            string[] parts = listLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts[parts.Length - 1];
        }

        private bool DownloadFile(string remoteFile, string localFile)
        {
            try
            {
                // TYPE I
                _writer.WriteLine("TYPE I");
                string typeResp = _reader.ReadLine();
                if (!typeResp.StartsWith("200")) return false;

                // PASV
                _writer.WriteLine("PASV");
                string pasvResp = _reader.ReadLine();
                if (!pasvResp.StartsWith("227")) return false;

                int start = pasvResp.IndexOf('(');
                int end = pasvResp.IndexOf(')');
                string[] ipParts = pasvResp.Substring(start + 1, end - start - 1).Split(',');

                string ip = $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.{ipParts[3]}";
                int port = int.Parse(ipParts[4]) * 256 + int.Parse(ipParts[5]);

                TcpClient dataClient = new TcpClient();
                dataClient.Connect(ip, port);

                // RETR
                _writer.WriteLine("RETR " + remoteFile);
                string retrResp = _reader.ReadLine();
                if (!retrResp.StartsWith("150"))
                {
                    dataClient.Close();
                    return false;
                }

                // copy stream
                using (NetworkStream dataStream = dataClient.GetStream())
                using (FileStream fs = new FileStream(localFile, FileMode.Create))
                {
                    dataStream.CopyTo(fs);
                }

                dataClient.Close();

                // Finish
                _reader.ReadLine(); // 226

                return true;
            }
            catch
            {
                return false;
            }
        }

        //Lấy file con trong folder
        private List<string> ListDirectory()
        {
            List<string> lines = new List<string>();

            _writer.WriteLine("PASV");
            string pasvResp = _reader.ReadLine();

            int start = pasvResp.IndexOf('(');
            int end = pasvResp.IndexOf(')');
            string[] ipParts = pasvResp.Substring(start + 1, end - start - 1).Split(',');

            string ip = $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.{ipParts[3]}";
            int port = int.Parse(ipParts[4]) * 256 + int.Parse(ipParts[5]);

            TcpClient dataClient = new TcpClient();
            dataClient.Connect(ip, port);

            _writer.WriteLine("LIST");
            _reader.ReadLine(); // 150

            using (StreamReader dr = new StreamReader(dataClient.GetStream()))
            {
                while (!dr.EndOfStream)
                    lines.Add(dr.ReadLine());
            }

            dataClient.Close();
            _reader.ReadLine(); // 226

            return lines;
        }

        //Tải folder
        private void DownloadFolder(string remoteFolder, string localFolder)
        {
            // tạo thư mục local
            Directory.CreateDirectory(localFolder);

            // vào thư mục trên server
            _writer.WriteLine("CWD " + remoteFolder);
            _reader.ReadLine(); // 250

            // lấy danh sách bên trong
            var items = ListDirectory();

            foreach (string line in items)
            {
                string name = GetNameFromListLine(line);

                if (IsDirectory(line))
                {
                    DownloadFolder(name, Path.Combine(localFolder, name)); // đệ quy
                }
                else
                {
                    DownloadFile(name, Path.Combine(localFolder, name));
                }
            }

            // quay lại thư mục trước đó
            _writer.WriteLine("CDUP");
            _reader.ReadLine();
        }

        //Hảm tải tự động nếu là file thì tải file, còn nếu là thư mục thì tải toàn bộ
        private void DownloadPath(string listLine, string saveTo)
        {
            string name = GetNameFromListLine(listLine);

            if (IsDirectory(listLine))
            {
                DownloadFolder(name, saveTo);
                MessageBox.Show("Download thư mục hoàn tất!");
            }
            else
            {
                DownloadFile(name, saveTo);
                MessageBox.Show("Download file hoàn tất!");
            }
        }

        private void btnDowloadServer_Click(object sender, EventArgs e)
        {
            string listLine = lvServerFiles.SelectedItems[0].Text;
            string name = GetNameFromListLine(listLine);

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Chọn nơi lưu file";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            string selectedPath = dlg.SelectedPath;

           /* if (selectedPath.Length < 2 || selectedPath[1] != ':' ||
             !char.ToUpper(selectedPath[0]).Equals('C'))
            {
                MessageBox.Show("Bạn phải chọn thư mục trong ổ C!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // dừng tải
            }*/

            string savePath = Path.Combine(selectedPath, name);
            DownloadPath(listLine, savePath);
        }

        // Hàm hiện hộp thoại xác nhận
        private bool ConfirmDelete(string name, bool isDirectory)
        {
            string type = isDirectory ? "thư mục" : "file";
            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {type} '{name}'?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return confirm == DialogResult.Yes;
        }

        // Hàm gửi lệnh xóa lên server FTP
        private void DeleteServerItem(string name, bool isDirectory)
        {
            try
            {
                string command = isDirectory ? $"RMD {name}" : $"DELE {name}";
                _writer.WriteLine(command);

                string response = _reader.ReadLine();

                if (response.StartsWith("250"))
                {
                    MessageBox.Show($"{(isDirectory ? "Thư mục" : "File")} '{name}' đã được xóa thành công.");
                    LoadServerFiles(); // tải lại danh sách để cập nhật
                }
                else
                {
                    MessageBox.Show($"Xóa thất bại: {response}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }
        

        private void btnDeleteServer_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                MessageBox.Show("Chưa kết nối tới server.");
                return;
            }

            if (lvServerFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Bạn chưa chọn file hoặc thư mục để xóa.");
                return;
            }

            
            string listLine = lvServerFiles.SelectedItems[0].Tag?.ToString() ?? lvServerFiles.SelectedItems[0].Text;
            string name = lvServerFiles.SelectedItems[0].Text;
            bool isDir = IsDirectory(listLine);

            if (!ConfirmDelete(name, isDir))
                return;

            DeleteServerItem(name, isDir);

        }
        #endregion

        #region Create
        private void CreateServerDirectory(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                MessageBox.Show("Tên thư mục không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Gửi lệnh MKD
                _writer.WriteLine($"MKD {folderName}");
                lblTrasferStatus.Text = $"Creating directory: {folderName}";

                // Đọc phản hồi từ server
                string response = _reader.ReadLine();

                if (response.StartsWith("257"))
                {
                    MessageBox.Show($"Thư mục '{folderName}' đã được tạo thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadServerFiles(); // Tải lại danh sách để hiển thị thư mục mới
                    lblTrasferStatus.Text = $"Directory created: {folderName}";
                }
                else
                {
                    MessageBox.Show($"Tạo thư mục thất bại: {response}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblTrasferStatus.Text = $"Failed to create directory: {folderName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo thư mục: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblTrasferStatus.Text = "Connection error during directory creation.";
            }
        }




        #endregion

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                MessageBox.Show("Chưa kết nối tới server.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mở hộp thoại để người dùng nhập tên thư mục
            string newFolderName = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập tên thư mục mới:", 
                "Tạo thư mục trên Server", 
                "New Folder");
            
            if (!string.IsNullOrWhiteSpace(newFolderName))
            {
                CreateServerDirectory(newFolderName);
            }
        }
    }
}
