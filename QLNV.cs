using System;
using System.Drawing;
using System.Windows.Forms;

namespace NCKH
{
    public partial class QLNV : Form
    {
        public QLNV()
        {
            InitializeComponent();
        }

        private void ApplyStyles(Label logo, Button logoutButton)
        {
            this.BackColor = Color.White;
            this.sidebar.BackColor = Color.FromArgb(240, 240, 240);
            this.header.BackColor = Color.FromArgb(220, 220, 220);
            this.mainPanel.BackColor = Color.White;

            logo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);

            this.titleLabel.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            this.titleLabel.BackColor = Color.LightGray;
            this.titleLabel.ForeColor = Color.Black;
            this.titleLabel.Padding = new Padding(10, 5, 10, 5);

            foreach (Control control in this.sidebar.Controls)
            {
                if (control is Button menuButton)
                {
                    menuButton.Font = new Font("Segoe UI", 12F);
                    menuButton.FlatStyle = FlatStyle.Flat;
                    menuButton.FlatAppearance.BorderSize = 0;

                    if (menuButton.Text.Equals("Đăng xuất"))
                    {
                        menuButton.BackColor = Color.Transparent;
                        menuButton.ForeColor = Color.Black;
                        menuButton.Click += LogoutButton_Click;
                    }
                    else
                    {
                        menuButton.BackColor = Color.LightGray;
                        menuButton.ForeColor = Color.Black;
                        menuButton.Click += MainMenuButton_Click;
                    }
                }
            }

            Button[] mainButtons = { this.btnDanhSach, this.btnSuaNV, this.btnThemNV, this.btnXoaNV };
            foreach (var btn in mainButtons)
            {
                btn.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
                btn.BackColor = Color.LightGray;
                btn.ForeColor = Color.Black;
                btn.UseVisualStyleBackColor = false;
                btn.Click += ControlButton_Click;
            }
        }

        private void ControlButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            this.titleLabel.Text = clickedButton.Text.Replace("\n", " ");

            if (clickedButton == btnDanhSach)
            {
                OpenChildForm(new QLNV_DSNV());
            }
            else if (clickedButton == btnSuaNV)
            {
                // Mở Form Sửa với ID mẫu
                OpenChildForm(new QLNV_Sua(1));
            }
            else if (clickedButton == btnThemNV)
            {
                // Mở Form Thêm Mới
                OpenChildForm(new QLNV_Them());
            }
            else if (clickedButton == btnXoaNV)
            {
                // Mở Form Xóa
                OpenChildForm(new QLNV_Xoa());
            }
            else
            {
                MessageBox.Show("Chức năng " + clickedButton.Text.Replace("\n", " ") + " đang được phát triển.", "Thông báo");
            }
        }

        private void OpenChildForm(Form childForm)
        {
            try
            {
                childForm.StartPosition = FormStartPosition.CenterScreen;

                if (childForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Thao tác thành công. Đã làm mới dữ liệu!", "Hoàn tất");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi mở Form con: " + ex.Message, "Lỗi hệ thống");
            }
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            MessageBox.Show("Chuyển đến chức năng: " + clickedButton.Text, "Menu Sidebar");
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}