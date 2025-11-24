using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FTP.Core.Server;
using FTP.Core.Authentication;
using System.IO;
using System.Net;


namespace FTP.Server.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly ServerConfiguration _config;
        private readonly UserManager _userManager;

        // nhớ user đang edit (null = đang thêm mới)
        private string _editingUserName = null;
        public SettingsForm()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;

        }
        #region MainButton
        private void SaveAllSettings()
        {
            // 1. Nếu đang nhập user mà chưa bấm Save User thì lưu luôn
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                btnSaveUser_Click(null, null);
            }

            // 2. Tab Permission – lưu permission của user đang chọn
            SavePermissionForSelectedUser();

            // 3. Tab Advanced – lưu cấu hình server
            if (_config != null)
            {
                _config.MaxConnections = (int)numMaxConnections.Value;
                _config.MaxConnectionsPerUser = (int)numMaxConnsPerUser.Value;
                _config.LoginTimeout = (int)numLoginTimeOut.Value;

                // IP filter
                _config.AllowAllConnections = rdAllowAll.Checked;

                _config.BannedIPs.Clear();
                foreach (var item in lstBannedIPs.Items)
                {
                    _config.BannedIPs.Add(item.ToString());
                }
            }

            MessageBox.Show("Settings have been saved successfully!",
                            "Saved",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveAllSettings();
            MessageBox.Show("Settings applied successfully.");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveAllSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region User Management
        public SettingsForm(ServerConfiguration config) : this()
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userManager = _config.UserManager
                           ?? throw new InvalidOperationException("UserManager is null in config.");

            InitUserManagementUI();
            InitPermissionTab();
        }
        private void InitUserManagementUI()
        {
            SetupUserListView();
            LoadUsersToListView();
            ResetUserDetail();
        }

        private void SetupUserListView()
        {
            lvUsers.Columns.Clear();
        }

        private void LoadUsersToListView()
        {
            lvUsers.Items.Clear();

            var users = _userManager.GetAllUsers();   // lấy list<User> từ JSON 
            foreach (var u in users)
            {
                var item = new ListViewItem(u.UserName);
                item.SubItems.Add(u.HomeDirectory);
                item.Tag = u;   // lưu cả object để lát lấy lại
                lvUsers.Items.Add(item);
            }
        }

        private void ResetUserDetail()
        {
            _editingUserName = null;
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtHomeDir.Text = "";
            lbPass.Text = "";

        }

        private void lvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvUsers.SelectedItems.Count == 0)
                return;

            var item = lvUsers.SelectedItems[0];
            var user = item.Tag as User;
            if (user == null) return;

            _editingUserName = user.UserName;
            txtUsername.Text = user.UserName;
            txtPassword.Text = ""; // KHÔNG hiện lại pass (vì đang lưu hash) 
            txtHomeDir.Text = user.HomeDirectory;
            lbPass.Text = "To keep the current password,\nleave this field empty.";
        }
        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (lvUsers.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }

            var item = lvUsers.SelectedItems[0];
            var user = item.Tag as User;
            if (user == null) return;

            // Không cho xóa admin mặc định
            if (string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Cannot delete default admin user.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete user '{user.UserName}'?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            bool ok = _userManager.RemoveUser(user.UserName);  // xóa trong list + users.json 
            if (!ok)
            {
                MessageBox.Show("Failed to delete user.");
                return;
            }

            LoadUsersToListView();
            ResetUserDetail();

        }

        private void btnBrowseHome_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select Home Directory";

                if (!string.IsNullOrWhiteSpace(txtHomeDir.Text))
                    dlg.SelectedPath = txtHomeDir.Text;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtHomeDir.Text = dlg.SelectedPath;
                }
            }
        }

        private void btnSaveUser_Click(object sender, EventArgs e)
        {
            var userName = txtUsername.Text.Trim();
            var password = txtPassword.Text;
            var homeDir = txtHomeDir.Text.Trim();

            // 1. Validate
            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Username is required.");
                return;
            }

            if (_editingUserName == null && string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Password is required for new user.");
                return;
            }

            if (string.IsNullOrEmpty(homeDir))
            {
                MessageBox.Show("Home directory is required.");
                return;
            }

            if (!Directory.Exists(homeDir))
            {
                var confirm = MessageBox.Show(
                    "Home directory does not exist. Create it?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    Directory.CreateDirectory(homeDir);
                }
                else
                {
                    return;
                }
            }

            // 2. Nếu đang thêm mới
            if (_editingUserName == null)
            {
                var newUser = new User
                {
                    UserName = userName,
                    PasswordHash = PasswordHelper.HashPassword(password), // hash trước khi lưu 
                    HomeDirectory = homeDir,
                    Permissions = UserPermissions.FullAccess, // tạm thời cho full, sau này sửa bằng tab Permission
                    QuotaBytes = -1
                };

                bool ok = _userManager.AddUser(newUser);   // thêm + SaveUsers()
                if (!ok)
                {
                    MessageBox.Show("Username already exists.");
                    return;
                }
            }
            // 3. Nếu đang sửa user cũ
            else
            {
                var existing = _userManager.GetUser(_editingUserName);
                if (existing == null)
                {
                    MessageBox.Show("User not found.");
                    return;
                }

                // Nếu đổi username, check trùng
                if (!string.Equals(_editingUserName, userName, StringComparison.OrdinalIgnoreCase))
                {
                    var other = _userManager.GetUser(userName);
                    if (other != null)
                    {
                        MessageBox.Show("Another user with this username already exists.");
                        return;
                    }

                    existing.UserName = userName;
                }

                existing.HomeDirectory = homeDir;

                if (!string.IsNullOrEmpty(password))
                {
                    existing.PasswordHash = PasswordHelper.HashPassword(password);
                }

                _userManager.UpdateUser(existing);   // lưu thay đổi  
            }

            // 4. Refresh UI
            LoadUsersToListView();
            ResetUserDetail();
        }

        private void btnCancelUser_Click(object sender, EventArgs e)
        {
            ResetUserDetail();
        }
        #endregion

        #region Permision
        private void InitPermissionTab()
        {
            if (_userManager == null) return;


            // Lấy list user từ UserManager
            var users = _userManager.GetAllUsers();

            cbbUserPermission.DataSource = null; // reset để binding lại cho sạch
            cbbUserPermission.DataSource = users;
            cbbUserPermission.DisplayMember = "UserName";
            cbbUserPermission.ValueMember = "UserName";

            // Nếu có user thì chọn user đầu tiên và load quyền
            if (users.Count > 0)
            {
                cbbUserPermission.SelectedIndex = 0;
                LoadPermissionForSelectedUser();
            }
            else
            {
                // Không có user nào thì clear checkbox
                LoadPermissionForSelectedUser();
            }
        }
        private void LoadPermissionForSelectedUser()
        {
            // Không chọn user nào thì clear hết
            if (!(cbbUserPermission.SelectedItem is User user))
            {
                chkRead.Checked = false;
                chkWrite.Checked = false;
                chkDelete.Checked = false;
                chkCreateDir.Checked = false;
                chkDeleteDir.Checked = false;
                return;
            }

            var perms = user.Permissions;

            // File permissions
            chkRead.Checked = perms.HasFlag(UserPermissions.Read);
            chkWrite.Checked = perms.HasFlag(UserPermissions.Write);


            chkDelete.Checked = perms.HasFlag(UserPermissions.Delete);

            // Directory permissions
            chkCreateDir.Checked = perms.HasFlag(UserPermissions.CreateDirectory);
            chkDeleteDir.Checked = perms.HasFlag(UserPermissions.DeleteDirectory);

        }

        private UserPermissions BuildPermissionsFromUI()
        {
            UserPermissions perms = UserPermissions.None;

            // Read  content đi chung
            if (chkRead.Checked )
                perms |= UserPermissions.Read;

            // Write  đi chung
            if (chkWrite.Checked )
                perms |= UserPermissions.Write;

            if (chkDelete.Checked)
                perms |= UserPermissions.Delete;

            if (chkCreateDir.Checked)
                perms |= UserPermissions.CreateDirectory;

            if (chkDeleteDir.Checked)
                perms |= UserPermissions.DeleteDirectory;

            return perms;
        }
        private void SavePermissionForSelectedUser()
        {
            if (!(cbbUserPermission.SelectedItem is User user))
                return;

            var newPerms = BuildPermissionsFromUI();

            user.Permissions = newPerms;

            // Gọi UserManager để update + SaveUsers()
            _userManager.UpdateUser(user);
        }
        private void cbbUserPermission_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPermissionForSelectedUser();
        }


        #endregion

        
    }
}
