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
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu 😢");
                    CleanupConnection();
                    return;
                }

                _isConnected = true;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                lblTrasferStatus.Text = "Connected to " + host + ":" + port;
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
       
        #endregion

       
    }
}
