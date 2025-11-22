using SettingsUI;
using SettingsUI.BLL;
using SettingsUI.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SettingsUI.UI
{
    public partial class UserControlAccount : BaseSettingControl
    {
        // === Controls ===
        private Button btnBack;
        private Label lblTitle;
        private TextBox txtUsername, txtPassword, txtStaffId;
        private ComboBox cmbRole;
        private Button btnEditUsername, btnEditPassword, btnSave;

        private Account currentAccount;
        private readonly AccountService accountService = new AccountService();
        private string oldUsername = "";

        public int CurrentUserId { get; set; } = -1;

        public UserControlAccount()
        {
            InitializeComponent();               // ← Bắt buộc gọi
            this.Load += UserControlAccount_Load; // ← Load dữ liệu khi hiện
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // ---------- Nút Back ----------
            btnBack = new Button
            {
                Text = "← Quay lại",
                Size = new Size(100, 35),
                Location = new Point(15, 15),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => RaiseBackClicked();
            this.Controls.Add(btnBack);

            // ---------- Tiêu đề ----------
            lblTitle = new Label
            {
                Text = "Cài đặt Tài khoản",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true,
                Location = new Point(300, 40)
            };
            this.Controls.Add(lblTitle);

            int labelX = 250;
            int inputX = 420;
            int y = 120;
            int step = 65;

            // ---------- Username ----------
            CreateLabel("Tên người dùng:", labelX, y);
            txtUsername = CreateTextBox(inputX, y - 3, true);
            btnEditUsername = CreateEditButton(inputX + 210, y - 3);
            btnEditUsername.Click += BtnEditUsername_Click;
            this.Controls.Add(btnEditUsername);

            // ---------- Password ----------
            y += step;
            CreateLabel("Mật khẩu:", labelX, y);
            txtPassword = CreateTextBox(inputX, y - 3, true);
            txtPassword.UseSystemPasswordChar = true;
            btnEditPassword = CreateEditButton(inputX + 210, y - 3);
            btnEditPassword.Click += BtnEditPassword_Click;
            this.Controls.Add(btnEditPassword);

            // ---------- Staff ID ----------
            y += step;
            CreateLabel("Staff ID:", labelX, y);
            txtStaffId = CreateTextBox(inputX, y - 3, true); // readonly

            // ---------- Role ----------
            y += step;
            CreateLabel("Role:", labelX, y);
            cmbRole = new ComboBox
            {
                Location = new Point(inputX, y - 3),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new[] { "Staff", "Manager" });
            this.Controls.Add(cmbRole);

            // ---------- Nút Save ----------
            y += step + 20;
            btnSave = new Button
            {
                Text = "Xác nhận thay đổi",
                Size = new Size(180, 45),
                Location = new Point(inputX, y),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #region Helper tạo control
        private void CreateLabel(string text, int x, int y)
        {
            Label lbl = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(lbl);
        }

        private TextBox CreateTextBox(int x, int y, bool readOnly)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(200, 25),
                ReadOnly = readOnly,
                Font = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(txt);
            return txt;
        }

        private Button CreateEditButton(int x, int y)
        {
            Button btn = new Button
            {
                Text = "✎",
                Size = new Size(50, 25),
                Location = new Point(x, y),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
        #endregion

        #region Events
        private void UserControlAccount_Load(object sender, EventArgs e)
        {
            if (CurrentUserId > 0)
                LoadUserData();
        }

        private void BtnEditUsername_Click(object sender, EventArgs e)
        {
            oldUsername = txtUsername.Text;
            txtUsername.ReadOnly = false;
            txtUsername.BackColor = Color.LightYellow;
            txtUsername.Focus();
        }

        private void BtnEditPassword_Click(object sender, EventArgs e)
        {
            var form = new PasswordChangeForm(currentAccount?.Password ?? "");
            if (form.ShowDialog() == DialogResult.OK && form.NewPassword != null)
            {
                if (accountService.ChangePassword(CurrentUserId, form.NewPassword))
                {
                    currentAccount.Password = form.NewPassword;
                    txtPassword.Text = new string('*', form.NewPassword.Length);
                    MessageBox.Show("Mật khẩu đã được cập nhật.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Lỗi cập nhật mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (currentAccount == null) return;

            string inputPass = Prompt.ShowDialog("Nhập mật khẩu hiện tại để xác nhận:", "Xác nhận thay đổi");
            if (inputPass == null) return;

            var result = accountService.UpdateAccountInfo(
                CurrentUserId,
                txtUsername.Text.Trim(),
                cmbRole.SelectedItem?.ToString() ?? "Staff",
                currentAccount.Password,
                inputPass);

            MessageBox.Show(result.Message,
                result.Success ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (result.Success)
            {
                txtUsername.ReadOnly = true;
                txtUsername.BackColor = Color.White;
                LoadUserData(); // reload dữ liệu mới
            }
            else if (inputPass == currentAccount.Password)
            {
                txtUsername.Text = oldUsername;
                txtUsername.ReadOnly = true;
                txtUsername.BackColor = Color.White;
            }
        }
        #endregion

        public void LoadUserData()
        {
            currentAccount = accountService.LoadAccount(CurrentUserId);
            if (currentAccount == null)
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtUsername.Text = currentAccount.Username;
            txtPassword.Text = new string('*', currentAccount.Password.Length);
            txtStaffId.Text = currentAccount.StaffId;
            cmbRole.SelectedItem = cmbRole.Items.Contains(currentAccount.Role) ? currentAccount.Role : "Staff";
            oldUsername = currentAccount.Username;
        }
    }

    // PasswordChangeForm và Prompt giữ nguyên như bạn đã có
    public class PasswordChangeForm : Form
    {
        private TextBox txtOld, txtNew1, txtNew2;
        private Button btnOk, btnCancel;
        private string currentPassword;
        public string NewPassword { get; private set; }
        public PasswordChangeForm(string currentPassword)
        {
            this.currentPassword = currentPassword;
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.Text = "Đổi mật khẩu";
            this.Size = new Size(350, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            Label lblOld = new Label() { Text = "Mật khẩu cũ:", Location = new Point(20, 20), AutoSize = true };
            txtOld = new TextBox() { Location = new Point(150, 20), Width = 150, UseSystemPasswordChar = true };
            Label lblNew1 = new Label() { Text = "Mật khẩu mới:", Location = new Point(20, 60), AutoSize = true };
            txtNew1 = new TextBox() { Location = new Point(150, 60), Width = 150, UseSystemPasswordChar = true };
            Label lblNew2 = new Label() { Text = "Nhập lại mật khẩu:", Location = new Point(20, 100), AutoSize = true };
            txtNew2 = new TextBox() { Location = new Point(150, 100), Width = 150, UseSystemPasswordChar = true };
            btnOk = new Button() { Text = "OK", Location = new Point(70, 150), Width = 80 };
            btnCancel = new Button() { Text = "Cancel", Location = new Point(180, 150), Width = 80 };
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.AddRange(new Control[] { lblOld, txtOld, lblNew1, txtNew1, lblNew2, txtNew2, btnOk, btnCancel });
        }
        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (txtOld.Text != currentPassword)
            {
                MessageBox.Show("Mật khẩu cũ không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtNew1.Text != txtNew2.Text)
            {
                MessageBox.Show("Mật khẩu mới không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNew1.Text))
            {
                MessageBox.Show("Mật khẩu mới không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            NewPassword = txtNew1.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
    // Prompt để nhập dữ liệu
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 350,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, AutoSize = true };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 280, UseSystemPasswordChar = (caption.ToLower().Contains("mật khẩu") || caption.ToLower().Contains("password")) };
            Button confirmation = new Button() { Text = "OK", Left = 70, Width = 80, Top = 80 };
            Button cancel = new Button() { Text = "Cancel", Left = 180, Width = 80, Top = 80 };
            confirmation.Click += (sender, e) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.DialogResult = DialogResult.Cancel; prompt.Close(); };
            prompt.Controls.AddRange(new Control[] { textLabel, inputBox, confirmation, cancel });
            prompt.AcceptButton = confirmation;
            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : null;
        }
    }
}




