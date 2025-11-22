namespace NCKH
{
    partial class QLNV_Sua
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.sidebarPanel = new System.Windows.Forms.Panel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.mainContentPanel = new System.Windows.Forms.Panel();
            this.logoLabel = new System.Windows.Forms.Label();
            this.iconHome = new System.Windows.Forms.PictureBox();
            this.iconCar = new System.Windows.Forms.PictureBox();
            this.iconStats = new System.Windows.Forms.PictureBox();
            this.iconEmployee = new System.Windows.Forms.PictureBox();
            this.iconHistory = new System.Windows.Forms.PictureBox();
            this.iconSettings = new System.Windows.Forms.PictureBox();
            this.iconLogout = new System.Windows.Forms.PictureBox();
            this.titleFormLabel = new System.Windows.Forms.Label();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.userRoleLabel = new System.Windows.Forms.Label();
            this.userAvatarIcon = new System.Windows.Forms.PictureBox();
            this.employeeImage = new System.Windows.Forms.PictureBox();
            this.lblHoTen = new System.Windows.Forms.Label();
            this.txtHoTen = new System.Windows.Forms.TextBox();
            this.lblNgaySinh = new System.Windows.Forms.Label();
            this.dtpNgaySinh = new System.Windows.Forms.DateTimePicker();
            this.lblChucVu = new System.Windows.Forms.Label();
            this.cboChucVu = new System.Windows.Forms.ComboBox();
            this.lblNgayBatDauLamViec = new System.Windows.Forms.Label();
            this.dtpNgayBatDauLamViec = new System.Windows.Forms.DateTimePicker();
            this.lblThongTinLienHe = new System.Windows.Forms.Label();
            this.txtThongTinLienHe = new System.Windows.Forms.TextBox();
            this.lblDiaChi = new System.Windows.Forms.Label();
            this.txtDiaChi = new System.Windows.Forms.TextBox();
            this.btnXacNhan = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.employeeImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconHome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconCar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconStats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconEmployee)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconLogout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userAvatarIcon)).BeginInit();

            this.SuspendLayout();

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.mainContentPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.sidebarPanel);
            this.Name = "QLNV_Sua";
            this.Text = "Sửa thông tin nhân viên";

            this.sidebarPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidebarPanel.Location = new System.Drawing.Point(0, 0);
            this.sidebarPanel.Size = new System.Drawing.Size(80, 700);
            this.sidebarPanel.Controls.Add(this.logoLabel);
            this.sidebarPanel.Controls.Add(this.iconHome);
            this.sidebarPanel.Controls.Add(this.iconCar);
            this.sidebarPanel.Controls.Add(this.iconStats);
            this.sidebarPanel.Controls.Add(this.iconEmployee);
            this.sidebarPanel.Controls.Add(this.iconHistory);
            this.sidebarPanel.Controls.Add(this.iconSettings);
            this.sidebarPanel.Controls.Add(this.iconLogout);

            this.logoLabel.AutoSize = true;
            this.logoLabel.Location = new System.Drawing.Point(10, 20);
            this.logoLabel.Name = "logoLabel";
            this.logoLabel.Text = "LOGO";

            int iconY = 100;
            this.iconHome.Location = new System.Drawing.Point(15, iconY);
            this.iconHome.Size = new System.Drawing.Size(50, 40);
            this.iconHome.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconHome.Name = "iconHome";
            // this.iconHome.Image = global::NCKH.Properties.Resources.home_icon;

            this.iconCar.Location = new System.Drawing.Point(15, iconY + 50);
            this.iconCar.Size = new System.Drawing.Size(50, 40);
            this.iconCar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconCar.Name = "iconCar";
            // this.iconCar.Image = global::NCKH.Properties.Resources.car_icon;

            this.iconStats.Location = new System.Drawing.Point(15, iconY + 100);
            this.iconStats.Size = new System.Drawing.Size(50, 40);
            this.iconStats.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconStats.Name = "iconStats";
            // this.iconStats.Image = global::NCKH.Properties.Resources.stats_icon;

            this.iconEmployee.Location = new System.Drawing.Point(15, iconY + 150);
            this.iconEmployee.Size = new System.Drawing.Size(50, 40);
            this.iconEmployee.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconEmployee.Name = "iconEmployee";
            // this.iconEmployee.Image = global::NCKH.Properties.Resources.employee_icon;

            this.iconHistory.Location = new System.Drawing.Point(15, iconY + 200);
            this.iconHistory.Size = new System.Drawing.Size(50, 40);
            this.iconHistory.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconHistory.Name = "iconHistory";
            // this.iconHistory.Image = global::NCKH.Properties.Resources.history_icon;

            this.iconSettings.Location = new System.Drawing.Point(15, iconY + 250);
            this.iconSettings.Size = new System.Drawing.Size(50, 40);
            this.iconSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconSettings.Name = "iconSettings";
            // this.iconSettings.Image = global::NCKH.Properties.Resources.settings_icon;

            this.iconLogout.Location = new System.Drawing.Point(15, 630);
            this.iconLogout.Size = new System.Drawing.Size(50, 40);
            this.iconLogout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconLogout.Name = "iconLogout";
            // this.iconLogout.Image = global::NCKH.Properties.Resources.logout_icon;

            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(80, 0);
            this.headerPanel.Size = new System.Drawing.Size(1120, 70);
            this.headerPanel.Controls.Add(this.titleFormLabel);
            this.headerPanel.Controls.Add(this.userNameLabel);
            this.headerPanel.Controls.Add(this.userRoleLabel);
            this.headerPanel.Controls.Add(this.userAvatarIcon);

            this.titleFormLabel.AutoSize = true;
            this.titleFormLabel.Location = new System.Drawing.Point(20, 20);
            this.titleFormLabel.Name = "titleFormLabel";
            this.titleFormLabel.Text = "Sửa thông tin nhân viên";

            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(950, 18);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Text = "Tên nhân viên";

            this.userRoleLabel.AutoSize = true;
            this.userRoleLabel.Location = new System.Drawing.Point(950, 38);
            this.userRoleLabel.Name = "userRoleLabel";
            this.userRoleLabel.Text = "Chức vụ";

            this.userAvatarIcon.Location = new System.Drawing.Point(1050, 15);
            this.userAvatarIcon.Size = new System.Drawing.Size(45, 45);
            this.userAvatarIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.userAvatarIcon.Name = "userAvatarIcon";
            // this.userAvatarIcon.Image = global::NCKH.Properties.Resources.default_avatar;

            this.mainContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContentPanel.Location = new System.Drawing.Point(80, 70);
            this.mainContentPanel.Size = new System.Drawing.Size(1120, 630);
            this.mainContentPanel.Controls.Add(this.employeeImage);
            this.mainContentPanel.Controls.Add(this.lblHoTen);
            this.mainContentPanel.Controls.Add(this.txtHoTen);
            this.mainContentPanel.Controls.Add(this.lblNgaySinh);
            this.mainContentPanel.Controls.Add(this.dtpNgaySinh);
            this.mainContentPanel.Controls.Add(this.lblChucVu);
            this.mainContentPanel.Controls.Add(this.cboChucVu);
            this.mainContentPanel.Controls.Add(this.lblNgayBatDauLamViec);
            this.mainContentPanel.Controls.Add(this.dtpNgayBatDauLamViec);
            this.mainContentPanel.Controls.Add(this.lblThongTinLienHe);
            this.mainContentPanel.Controls.Add(this.txtThongTinLienHe);
            this.mainContentPanel.Controls.Add(this.lblDiaChi);
            this.mainContentPanel.Controls.Add(this.txtDiaChi);
            this.mainContentPanel.Controls.Add(this.btnXacNhan);

            this.employeeImage.Location = new System.Drawing.Point(50, 50);
            this.employeeImage.Size = new System.Drawing.Size(150, 180);
            this.employeeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.employeeImage.Name = "employeeImage";
            // this.employeeImage.Image = global::NCKH.Properties.Resources.BartSimpson_Image;

            int labelX = 250;
            int controlX = 450;
            int startY = 50;
            int spacing = 40;

            this.lblHoTen.AutoSize = true;
            this.lblHoTen.Location = new System.Drawing.Point(labelX, startY);
            this.lblHoTen.Name = "lblHoTen";
            this.lblHoTen.Text = "Họ và tên:";
            this.txtHoTen.Location = new System.Drawing.Point(controlX, startY - 3);
            this.txtHoTen.Size = new System.Drawing.Size(300, 20);
            this.txtHoTen.Name = "txtHoTen";

            this.lblNgaySinh.AutoSize = true;
            this.lblNgaySinh.Location = new System.Drawing.Point(labelX, startY + spacing);
            this.lblNgaySinh.Name = "lblNgaySinh";
            this.lblNgaySinh.Text = "Ngày sinh:";
            this.dtpNgaySinh.Location = new System.Drawing.Point(controlX, startY + spacing - 3);
            this.dtpNgaySinh.Size = new System.Drawing.Size(150, 20);
            this.dtpNgaySinh.Name = "dtpNgaySinh";
            this.dtpNgaySinh.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            this.lblChucVu.AutoSize = true;
            this.lblChucVu.Location = new System.Drawing.Point(labelX, startY + spacing * 2);
            this.lblChucVu.Name = "lblChucVu";
            this.lblChucVu.Text = "Chức vụ:";
            this.cboChucVu.Location = new System.Drawing.Point(controlX, startY + spacing * 2 - 3);
            this.cboChucVu.Size = new System.Drawing.Size(150, 21);
            this.cboChucVu.Name = "cboChucVu";
            this.cboChucVu.Items.AddRange(new object[] { "Nhân Viên", "Quản lý", "Giám đốc" });

            this.lblNgayBatDauLamViec.AutoSize = true;
            this.lblNgayBatDauLamViec.Location = new System.Drawing.Point(labelX, startY + spacing * 4);
            this.lblNgayBatDauLamViec.Name = "lblNgayBatDauLamViec";
            this.lblNgayBatDauLamViec.Text = "Ngày bắt đầu làm việc:";
            this.dtpNgayBatDauLamViec.Location = new System.Drawing.Point(controlX, startY + spacing * 4 - 3);
            this.dtpNgayBatDauLamViec.Size = new System.Drawing.Size(150, 20);
            this.dtpNgayBatDauLamViec.Name = "dtpNgayBatDauLamViec";
            this.dtpNgayBatDauLamViec.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            this.lblThongTinLienHe.AutoSize = true;
            this.lblThongTinLienHe.Location = new System.Drawing.Point(labelX, startY + spacing * 5);
            this.lblThongTinLienHe.Name = "lblThongTinLienHe";
            this.lblThongTinLienHe.Text = "Thông tin liên hệ:";
            this.txtThongTinLienHe.Location = new System.Drawing.Point(controlX, startY + spacing * 5 - 3);
            this.txtThongTinLienHe.Size = new System.Drawing.Size(200, 20);
            this.txtThongTinLienHe.Name = "txtThongTinLienHe";

            this.lblDiaChi.AutoSize = true;
            this.lblDiaChi.Location = new System.Drawing.Point(labelX, startY + spacing * 6);
            this.lblDiaChi.Name = "lblDiaChi";
            this.lblDiaChi.Text = "Địa chỉ:";
            this.txtDiaChi.Location = new System.Drawing.Point(controlX, startY + spacing * 6 - 3);
            this.txtDiaChi.Size = new System.Drawing.Size(400, 20);
            this.txtDiaChi.Name = "txtDiaChi";

            this.btnXacNhan.Location = new System.Drawing.Point(500, 500);
            this.btnXacNhan.Size = new System.Drawing.Size(150, 50);
            this.btnXacNhan.Name = "btnXacNhan";
            this.btnXacNhan.Text = "Xác nhận";

            this.ApplyStyles();

            ((System.ComponentModel.ISupportInitialize)(this.employeeImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconHome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconCar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconStats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconEmployee)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconLogout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userAvatarIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel sidebarPanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel mainContentPanel;
        private System.Windows.Forms.Label logoLabel;
        private System.Windows.Forms.PictureBox iconHome;
        private System.Windows.Forms.PictureBox iconCar;
        private System.Windows.Forms.PictureBox iconStats;
        private System.Windows.Forms.PictureBox iconEmployee;
        private System.Windows.Forms.PictureBox iconHistory;
        private System.Windows.Forms.PictureBox iconSettings;
        private System.Windows.Forms.PictureBox iconLogout;
        private System.Windows.Forms.Label titleFormLabel;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label userRoleLabel;
        private System.Windows.Forms.PictureBox userAvatarIcon;
        private System.Windows.Forms.PictureBox employeeImage;
        private System.Windows.Forms.Label lblHoTen;
        private System.Windows.Forms.TextBox txtHoTen;
        private System.Windows.Forms.Label lblNgaySinh;
        private System.Windows.Forms.DateTimePicker dtpNgaySinh;
        private System.Windows.Forms.Label lblChucVu;
        private System.Windows.Forms.ComboBox cboChucVu;
        private System.Windows.Forms.Label lblNgayBatDauLamViec;
        private System.Windows.Forms.DateTimePicker dtpNgayBatDauLamViec;
        private System.Windows.Forms.Label lblThongTinLienHe;
        private System.Windows.Forms.TextBox txtThongTinLienHe;
        private System.Windows.Forms.Label lblDiaChi;
        private System.Windows.Forms.TextBox txtDiaChi;
        private System.Windows.Forms.Button btnXacNhan;
    }
}