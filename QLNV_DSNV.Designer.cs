namespace NCKH
{
    partial class QLNV_DSNV
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
            this.titleFormLabel = new System.Windows.Forms.Label();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.userRoleLabel = new System.Windows.Forms.Label();
            this.userAvatarIcon = new System.Windows.Forms.PictureBox();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.iconSearch = new System.Windows.Forms.PictureBox();
            this.dataGridPanel = new System.Windows.Forms.Panel();
            this.dataGridViewNhanVien = new System.Windows.Forms.DataGridView();
            this.iconHome = new System.Windows.Forms.PictureBox();
            this.iconCar = new System.Windows.Forms.PictureBox();
            this.iconStats = new System.Windows.Forms.PictureBox();
            this.iconEmployee = new System.Windows.Forms.PictureBox();
            this.iconHistory = new System.Windows.Forms.PictureBox();
            this.iconSettings = new System.Windows.Forms.PictureBox();
            this.iconLogout = new System.Windows.Forms.PictureBox();

            ((System.ComponentModel.ISupportInitialize)(this.userAvatarIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNhanVien)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconHome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconCar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconStats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconEmployee)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconLogout)).BeginInit();

            this.SuspendLayout();

            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.mainContentPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.sidebarPanel);
            this.Name = "QLNV_DSNV";
            this.Text = "Danh Sách Nhân Viên";

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
            this.logoLabel.Text = "LOGO";

            int iconY = 100;
            this.iconHome.Location = new System.Drawing.Point(15, iconY);
            this.iconCar.Location = new System.Drawing.Point(15, iconY + 50);
            this.iconStats.Location = new System.Drawing.Point(15, iconY + 100);
            this.iconEmployee.Location = new System.Drawing.Point(15, iconY + 150);
            this.iconHistory.Location = new System.Drawing.Point(15, iconY + 200);
            this.iconSettings.Location = new System.Drawing.Point(15, iconY + 250);
            this.iconLogout.Location = new System.Drawing.Point(15, 630);

            System.Windows.Forms.PictureBox[] sidebarIcons = {
                iconHome, iconCar, iconStats, iconEmployee, iconHistory, iconSettings, iconLogout
            };
            foreach (var icon in sidebarIcons)
            {
                icon.Size = new System.Drawing.Size(50, 40);
                icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            }
            // this.iconHome.Image = global::NCKH.Properties.Resources.home_icon;
            // this.iconEmployee.Image = global::NCKH.Properties.Resources.employee_icon;

            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(80, 0);
            this.headerPanel.Size = new System.Drawing.Size(1120, 70);
            this.headerPanel.Controls.Add(this.titleFormLabel);
            this.headerPanel.Controls.Add(this.userNameLabel);
            this.headerPanel.Controls.Add(this.userRoleLabel);
            this.headerPanel.Controls.Add(this.userAvatarIcon);

            this.titleFormLabel.AutoSize = true;
            this.titleFormLabel.Location = new System.Drawing.Point(20, 20);
            this.titleFormLabel.Text = "Danh Sách Nhân Viên";

            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(950, 18);
            this.userNameLabel.Text = "Tên nhân viên";

            this.userRoleLabel.AutoSize = true;
            this.userRoleLabel.Location = new System.Drawing.Point(950, 38);
            this.userRoleLabel.Text = "Chức vụ";

            this.userAvatarIcon.Location = new System.Drawing.Point(1050, 15);
            this.userAvatarIcon.Size = new System.Drawing.Size(45, 45);
            this.userAvatarIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // this.userAvatarIcon.Image = global::NCKH.Properties.Resources.default_avatar;

            this.mainContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContentPanel.Location = new System.Drawing.Point(80, 70);
            this.mainContentPanel.Size = new System.Drawing.Size(1120, 630);
            this.mainContentPanel.Controls.Add(this.dataGridPanel);

            this.dataGridPanel.Location = new System.Drawing.Point(20, 20);
            this.dataGridPanel.Size = new System.Drawing.Size(1060, 580);
            this.dataGridPanel.Controls.Add(this.txtSearch);
            this.dataGridPanel.Controls.Add(this.iconSearch);
            this.dataGridPanel.Controls.Add(this.dataGridViewNhanVien);

            this.txtSearch.Location = new System.Drawing.Point(750, 30);
            this.txtSearch.Size = new System.Drawing.Size(280, 30);
            this.txtSearch.Text = "Tìm nhân viên...";
            this.txtSearch.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);

            this.iconSearch.Location = new System.Drawing.Point(760, 35);
            this.iconSearch.Size = new System.Drawing.Size(20, 20);
            this.iconSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            // this.iconSearch.Image = global::NCKH.Properties.Resources.search_icon;

            this.dataGridViewNhanVien.Location = new System.Drawing.Point(30, 90);
            this.dataGridViewNhanVien.Size = new System.Drawing.Size(1000, 450);
            this.dataGridViewNhanVien.AutoGenerateColumns = false;
            this.dataGridViewNhanVien.ReadOnly = true;

            this.ApplyStyles();

            ((System.ComponentModel.ISupportInitialize)(this.userAvatarIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNhanVien)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconHome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconCar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconStats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconEmployee)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconLogout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel sidebarPanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel mainContentPanel;
        private System.Windows.Forms.Label logoLabel;
        private System.Windows.Forms.Label titleFormLabel;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label userRoleLabel;
        private System.Windows.Forms.PictureBox userAvatarIcon;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.PictureBox iconSearch;
        private System.Windows.Forms.Panel dataGridPanel;
        private System.Windows.Forms.DataGridView dataGridViewNhanVien;
        private System.Windows.Forms.PictureBox iconHome;
        private System.Windows.Forms.PictureBox iconCar;
        private System.Windows.Forms.PictureBox iconStats;
        private System.Windows.Forms.PictureBox iconEmployee;
        private System.Windows.Forms.PictureBox iconHistory;
        private System.Windows.Forms.PictureBox iconSettings;
        private System.Windows.Forms.PictureBox iconLogout;
    }
}