using System;
using System.Drawing;
using System.Windows.Forms;

namespace SettingsUI.UI
{
    public partial class UserControlGeneral : BaseSettingControl
    {
        private Button btnBack;
        private Label lblTitle;

        // GroupBox
        private GroupBox grpLanguage, grpTheme, grpNotifications, grpPrivacy, grpSystem;

        // Controls
        private ComboBox cmbLanguage, cmbTimezone, cmbTheme;
        private CheckBox chkNotifyMessages, chkNotifyUpdates, chkNotifySounds;
        private CheckBox chkShareData, chkUseCamera, chkUseLocation;
        private CheckBox chkAutoUpdate, chkBackup;
        private Button btnResetDefaults;

        public UserControlGeneral()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // Nút Back
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

            // Tiêu đề
            lblTitle = new Label
            {
                Text = "Cài đặt chung",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true,
                Location = new Point(180, 25)
            };
            this.Controls.Add(lblTitle);

            int startX = 80;
            int startY = 100;
            int groupWidth = 720;
            int groupHeight = 140;
            int gap = 160;

            // 1. Ngôn ngữ & Múi giờ
            grpLanguage = CreateGroupBox("Ngôn ngữ & Khu vực", startX, startY);
            AddLabelAndCombo(grpLanguage, "Ngôn ngữ:", 25, 35, out cmbLanguage, new[] { "Tiếng Việt", "English" });
            AddLabelAndCombo(grpLanguage, "Múi giờ:", 25, 80, out cmbTimezone, new[] { "GMT+7 (Indochina Time)", "GMT+0 (London)", "GMT+8 (Beijing)", "GMT-5 (New York)" });
            cmbLanguage.SelectedIndex = 0;
            cmbTimezone.SelectedIndex = 0;

            // 2. Giao diện
            grpTheme = CreateGroupBox("Giao diện", startX + groupWidth + 50, startY);
            AddLabelAndCombo(grpTheme, "Chủ đề:", 25, 50, out cmbTheme, new[] { "Sáng", "Tối", "Tự động theo hệ thống" });
            cmbTheme.SelectedIndex = 0;

            // 3. Thông báo
            grpNotifications = CreateGroupBox("Thông báo", startX, startY + gap);
            chkNotifyMessages = AddCheckBox(grpNotifications, "Hiển thị thông báo tin nhắn", 25, 35, true);
            chkNotifyUpdates = AddCheckBox(grpNotifications, "Thông báo cập nhật phần mềm", 25, 65, true);
            chkNotifySounds = AddCheckBox(grpNotifications, "Phát âm thanh khi có thông báo", 25, 95, true);

            // 4. Quyền riêng tư
            grpPrivacy = CreateGroupBox("Quyền riêng tư & Bảo mật", startX + groupWidth + 50, startY + gap);
            chkShareData = AddCheckBox(grpPrivacy, "Chia sẻ dữ liệu ẩn danh để cải thiện sản phẩm", 25, 35, false);
            chkUseCamera = AddCheckBox(grpPrivacy, "Cho phép ứng dụng sử dụng camera", 25, 65, false);
            chkUseLocation = AddCheckBox(grpPrivacy, "Cho phép ứng dụng truy cập vị trí", 25, 95, false);

            // 5. Hệ thống
            grpSystem = CreateGroupBox("Hệ thống & Nâng cao", startX, startY + gap * 2);
            chkAutoUpdate = AddCheckBox(grpSystem, "Tự động kiểm tra và cập nhật", 25, 35, true);
            chkBackup = AddCheckBox(grpSystem, "Tự động sao lưu dữ liệu hàng tuần", 25, 70, true);

            btnResetDefaults = new Button
            {
                Text = "Khôi phục cài đặt mặc định",
                Size = new Size(220, 42),
                Location = new Point(25, 110),
                BackColor = Color.FromArgb(52, 58, 64),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnResetDefaults.FlatAppearance.BorderSize = 0;
            btnResetDefaults.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 77, 85);
            btnResetDefaults.Click += (s, e) => ResetToDefaults();
            grpSystem.Controls.Add(btnResetDefaults);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #region Helper Methods

        private GroupBox CreateGroupBox(string title, int x, int y)
        {
            var gb = new GroupBox
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                Size = new Size(720, 170),
                Location = new Point(x, y)
            };
            this.Controls.Add(gb);
            return gb;
        }

        private void AddLabelAndCombo(GroupBox parent, string labelText, int labelY, int comboY, out ComboBox combo, string[] items)
        {
            var lbl = new Label { Text = labelText, Location = new Point(25, labelY), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            combo = new ComboBox
            {
                Location = new Point(180, comboY),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            combo.Items.AddRange(items);
            parent.Controls.Add(lbl);
            parent.Controls.Add(combo);
        }

        private CheckBox AddCheckBox(GroupBox parent, string text, int x, int y, bool @checked)
        {
            var chk = new CheckBox
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Checked = @checked,
                Font = new Font("Segoe UI", 10F)
            };
            parent.Controls.Add(chk);
            return chk;
        }

        private void ResetToDefaults()
        {
            if (MessageBox.Show("Bạn có chắc muốn khôi phục tất cả cài đặt về mặc định?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cmbLanguage.SelectedIndex = 0;
                cmbTimezone.SelectedIndex = 0;
                cmbTheme.SelectedIndex = 0;

                chkNotifyMessages.Checked = true;
                chkNotifyUpdates.Checked = true;
                chkNotifySounds.Checked = true;

                chkShareData.Checked = false;
                chkUseCamera.Checked = false;
                chkUseLocation.Checked = false;

                chkAutoUpdate.Checked = true;
                chkBackup.Checked = true;

                MessageBox.Show("Đã khôi phục cài đặt mặc định thành công!", "Hoàn tất",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion
    }
}