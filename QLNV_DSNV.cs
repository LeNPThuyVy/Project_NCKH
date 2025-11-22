using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NCKH
{
    public class NhanVien
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string ChucVu { get; set; }
    }

    public partial class QLNV_DSNV : Form
    {
        public QLNV_DSNV()
        {
            InitializeComponent();
            ApplyStyles();
            SetupDataGridView();
            LoadDataNhanVien();
        }

        private void ApplyStyles()
        {
            // --- FORM & PANEL STYLES ---
            this.BackColor = Color.White;
            this.sidebarPanel.BackColor = Color.FromArgb(245, 245, 245); // Xám rất nhạt
            this.headerPanel.BackColor = Color.White;
            this.mainContentPanel.BackColor = Color.White;

            // Header/Logo
            this.logoLabel.ForeColor = Color.Black;
            this.logoLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.titleFormLabel.ForeColor = Color.Black;
            this.titleFormLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);

            this.userNameLabel.ForeColor = Color.Black;
            this.userRoleLabel.ForeColor = Color.DarkGray;
            this.userAvatarIcon.BackColor = Color.LightGray;

            // Sidebar Icons (Chữ đen, không có nền xanh)
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

            // --- MAIN CONTENT STYLES ---
            this.dataGridPanel.BackColor = Color.White;
            this.dataGridPanel.BorderStyle = BorderStyle.FixedSingle; // Viền xám

            // Search Box
            this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
            this.txtSearch.Font = new Font("Segoe UI", 12F);
            this.txtSearch.ForeColor = Color.DarkGray;
            this.txtSearch.BackColor = Color.White;
            this.iconSearch.BackColor = Color.Transparent;

            // DataGridView Styles
            this.dataGridViewNhanVien.BorderStyle = BorderStyle.None;
            this.dataGridViewNhanVien.BackgroundColor = Color.White;
            this.dataGridViewNhanVien.RowHeadersVisible = false;
            this.dataGridViewNhanVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewNhanVien.EnableHeadersVisualStyles = false;

            this.dataGridViewNhanVien.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dataGridViewNhanVien.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            this.dataGridViewNhanVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            this.dataGridViewNhanVien.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            this.dataGridViewNhanVien.DefaultCellStyle.Font = new Font("Segoe UI", 11F);
            this.dataGridViewNhanVien.DefaultCellStyle.BackColor = Color.White;
            this.dataGridViewNhanVien.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            this.dataGridViewNhanVien.CellBorderStyle = DataGridViewCellBorderStyle.None;
            this.dataGridViewNhanVien.CellDoubleClick += DataGridViewNhanVien_CellDoubleClick;
        }

        private void SetupDataGridView()
        {
            DataGridViewTextBoxColumn colMaNV = new DataGridViewTextBoxColumn();
            colMaNV.DataPropertyName = "MaNhanVien";
            colMaNV.HeaderText = "Mã nhân viên";
            colMaNV.Width = 200;

            DataGridViewTextBoxColumn colTenNV = new DataGridViewTextBoxColumn();
            colTenNV.DataPropertyName = "TenNhanVien";
            colTenNV.HeaderText = "Tên nhân viên";
            colTenNV.Width = 400;

            DataGridViewTextBoxColumn colChucVu = new DataGridViewTextBoxColumn();
            colChucVu.DataPropertyName = "ChucVu";
            colChucVu.HeaderText = "Chức vụ";
            colChucVu.Width = 400;

            this.dataGridViewNhanVien.Columns.Add(colMaNV);
            this.dataGridViewNhanVien.Columns.Add(colTenNV);
            this.dataGridViewNhanVien.Columns.Add(colChucVu);
        }

        private void LoadDataNhanVien()
        {
            List<NhanVien> danhSach = new List<NhanVien>
            {
                new NhanVien { MaNhanVien = "NV001", TenNhanVien = "Lê Ngọc Phương Thúy", ChucVu = "Quản lý" },
                new NhanVien { MaNhanVien = "NV002", TenNhanVien = "Thái Duy Vũ", ChucVu = "Bảo vệ" },
                new NhanVien { MaNhanVien = "NV003", TenNhanVien = "Nguyễn Minh Quí", ChucVu = "Nhân viên" },
                new NhanVien { MaNhanVien = "NV004", TenNhanVien = "Trần Bảo Anh", ChucVu = "Nhân viên" }
            };

            this.dataGridViewNhanVien.DataSource = danhSach;
        }

        private void DataGridViewNhanVien_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                NhanVien selectedNV = this.dataGridViewNhanVien.Rows[e.RowIndex].DataBoundItem as NhanVien;

                if (selectedNV != null && int.TryParse(selectedNV.MaNhanVien.Replace("NV", ""), out int maSo))
                {
                    // Mở Form Sửa thông qua ID
                    // MessageBox.Show($"Mở Form Sửa cho Mã NV: {selectedNV.MaNhanVien}");
                }
            }
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
            if (clickedIcon != null) clickedIcon.BackColor = Color.LightGray; // Highlight bằng màu xám nhạt
        }
    }
}