using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace NCKH
{
    public partial class QLNV_Them : Form
    {
        private string connectionString = "Host=localhost;Port=5432;Database=QLXE;Username=postgres;Password=MQ123@";

        public QLNV_Them()
        {
            InitializeComponent();
            ApplyStyles();
            this.Load += QLNV_Them_Load;
        }

        private void QLNV_Them_Load(object sender, EventArgs e)
        {
            txtMaNhanVien.Text = GenerateNewStaffId();
        }

        private string GenerateNewStaffId()
        {
            string newId = "NV00001";
            string sql = "SELECT MAX(ID) FROM STAFF";

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            string lastId = result.ToString();
                            if (lastId.StartsWith("NV") && lastId.Length == 7)
                            {
                                int currentNum = int.Parse(lastId.Substring(2));
                                int nextNum = currentNum + 1;
                                newId = "NV" + nextNum.ToString("D5");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo ID nhân viên: {ex.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return newId;
            }
            return newId;
        }

        private void InsertStaffAndAccount(string staffId, string hoTen, string chucVu, string matKhau)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlStaff = "INSERT INTO STAFF (ID, NAME, START_TIME) VALUES (@id, @name, NOW())";
                        using (var cmdStaff = new NpgsqlCommand(sqlStaff, conn, transaction))
                        {
                            cmdStaff.Parameters.AddWithValue("id", staffId);
                            cmdStaff.Parameters.AddWithValue("name", hoTen);
                            cmdStaff.ExecuteNonQuery();
                        }

                        if (!string.IsNullOrEmpty(matKhau) && (chucVu == "Manager" || chucVu == "Staff"))
                        {
                            string sqlAccount = "INSERT INTO USER_ACCOUNT (USERNAME, PASS, STAFF_ID, ROLE) VALUES (@username, @pass, @staffid, @role)";
                            using (var cmdAccount = new NpgsqlCommand(sqlAccount, conn, transaction))
                            {
                                string username = hoTen.Replace(" ", "").ToLower() + staffId.Substring(5);

                                cmdAccount.Parameters.AddWithValue("username", username);
                                cmdAccount.Parameters.AddWithValue("pass", matKhau);
                                cmdAccount.Parameters.AddWithValue("staffid", staffId);
                                cmdAccount.Parameters.AddWithValue("role", chucVu);
                                cmdAccount.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show($"Đã thêm nhân viên {hoTen} (Mã: {staffId}) thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Lỗi khi thêm nhân viên: {ex.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnXacNhan_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNhanVien.Text;
            string hoTen = txtHoTen.Text;
            string chucVu = txtChucVu.Text;
            string matKhau = txtMatKhau.Text;
            string xacNhan = txtXacNhanMK.Text;

            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(matKhau) || string.IsNullOrWhiteSpace(xacNhan))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các trường thông tin bắt buộc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (matKhau != xacNhan)
            {
                MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            InsertStaffAndAccount(maNV, hoTen, chucVu, matKhau);
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

            foreach (Control control in this.sidebarPanel.Controls)
            {
                if (control is Label menuLabel)
                {
                    menuLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
                    menuLabel.ForeColor = Color.Black;
                    menuLabel.Cursor = Cursors.Hand;
                    menuLabel.Click += SidebarMenu_Click;

                    if (menuLabel.Text.Equals("Nhân viên"))
                    {
                        menuLabel.BackColor = Color.FromArgb(0, 192, 255);
                    }
                    else if (menuLabel.Text.Equals("Đăng xuất"))
                    {
                        menuLabel.BackColor = Color.Transparent;
                        menuLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                        menuLabel.TextAlign = ContentAlignment.MiddleCenter;
                    }
                    else
                    {
                        menuLabel.BackColor = Color.Transparent;
                    }
                }
            }

            this.lblDienThongTin.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblDienThongTin.ForeColor = Color.Black;

            Label[] dataLabels = { lblHoTen, lblChucVu, lblMatKhau, lblXacNhanMK };
            foreach (var lbl in dataLabels)
            {
                lbl.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
                lbl.ForeColor = Color.Black;
            }

            Control[] inputControls = { txtHoTen, txtChucVu, txtMatKhau, txtXacNhanMK, txtMaNhanVien };
            foreach (var ctrl in inputControls)
            {
                ctrl.Font = new Font("Segoe UI", 12F);
                ctrl.BackColor = Color.White;
                ctrl.ForeColor = Color.Black;
                if (ctrl is TextBox txt) txt.BorderStyle = BorderStyle.FixedSingle;
            }

            this.txtMaNhanVien.ReadOnly = true;

            this.btnXacNhan.BackColor = Color.FromArgb(0, 192, 255);
            this.btnXacNhan.ForeColor = Color.White;
            this.btnXacNhan.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.btnXacNhan.FlatStyle = FlatStyle.Flat;
            this.btnXacNhan.FlatAppearance.BorderSize = 0;
            this.btnXacNhan.Cursor = Cursors.Hand;
            this.btnXacNhan.Click += BtnXacNhan_Click;
        }

        private void SidebarMenu_Click(object sender, EventArgs e)
        {
            Label clickedLabel = sender as Label;

            if (clickedLabel.Text.Equals("Đăng xuất"))
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show($"Chuyển đến chức năng: {clickedLabel.Text}", "Menu Sidebar");
            }
        }
    }
}