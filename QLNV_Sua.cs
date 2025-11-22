using System;
using System.Drawing;
using System.Windows.Forms;

namespace NCKH
{
    public partial class QLNV_Sua : Form
    {
        private int _employeeId;

        public QLNV_Sua(int employeeId = -1)
        {
            InitializeComponent();
            _employeeId = employeeId;

            if (_employeeId != -1)
            {
                LoadEmployeeData(_employeeId);
            }
        }

        private void ApplyStyles()
        {
            this.BackColor = Color.White;
            this.sidebarPanel.BackColor = Color.FromArgb(245, 245, 245);
            this.headerPanel.BackColor = Color.White;
            this.mainContentPanel.BackColor = Color.White;

            this.logoLabel.ForeColor = Color.Black;
            this.logoLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);

            this.titleFormLabel.ForeColor = Color.Black;
            this.titleFormLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.titleFormLabel.BackColor = Color.Transparent;

            this.userNameLabel.ForeColor = Color.Black;
            this.userRoleLabel.ForeColor = Color.DarkGray;
            this.userAvatarIcon.BackColor = Color.LightGray;

            PictureBox[] sidebarIcons = {
                iconHome, iconCar, iconStats, iconEmployee,
                iconHistory, iconSettings, iconLogout
            };
            foreach (var icon in sidebarIcons)
            {
                icon.BackColor = Color.Transparent;
                icon.Cursor = Cursors.Hand;
                icon.Click += SidebarIcon_Click;
            }

            this.employeeImage.BackColor = Color.LightGray;

            Label[] infoLabels = {
                lblHoTen, lblNgaySinh, lblChucVu, lblNgayBatDauLamViec,
                lblThongTinLienHe, lblDiaChi
            };
            foreach (var lbl in infoLabels)
            {
                lbl.ForeColor = Color.Black;
                lbl.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            }

            Control[] inputControls = {
                txtHoTen, dtpNgaySinh, cboChucVu, dtpNgayBatDauLamViec,
                txtThongTinLienHe, txtDiaChi
            };
            foreach (var ctrl in inputControls)
            {
                ctrl.Font = new Font("Segoe UI", 12F);
                ctrl.BackColor = Color.White;
                ctrl.ForeColor = Color.Black;
                if (ctrl is TextBox txt) txt.BorderStyle = BorderStyle.FixedSingle;
                if (ctrl is ComboBox cb)
                {
                    cb.FlatStyle = FlatStyle.Flat;
                }
            }

            this.btnXacNhan.BackColor = Color.LightGray;
            this.btnXacNhan.ForeColor = Color.Black;
            this.btnXacNhan.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.btnXacNhan.FlatStyle = FlatStyle.Flat;
            this.btnXacNhan.FlatAppearance.BorderSize = 0;
            this.btnXacNhan.Cursor = Cursors.Hand;
            this.btnXacNhan.Click += btnXacNhan_Click;
        }

        private void LoadEmployeeData(int employeeId)
        {
            this.txtHoTen.Text = "Nguyễn Minh Quí";
            this.dtpNgaySinh.Value = new DateTime(2005, 7, 1);
            this.cboChucVu.SelectedItem = "Nhân Viên";
            this.dtpNgayBatDauLamViec.Value = new DateTime(2020, 1, 1);
            this.txtThongTinLienHe.Text = "0333333333";
            this.txtDiaChi.Text = "140 Lê Trọng Tấn, Tây Thạnh, Tân Phú, TP. HCM";
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Dữ liệu đã được cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SidebarIcon_Click(object sender, EventArgs e)
        {
            PictureBox clickedIcon = sender as PictureBox;
            foreach (Control ctrl in sidebarPanel.Controls)
            {
                if (ctrl is PictureBox pb && pb.Name.StartsWith("icon"))
                {
                    pb.BackColor = Color.Transparent;
                }
            }
            if (clickedIcon != null) clickedIcon.BackColor = Color.LightGray;
        }
    }
}