using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SettingsUI.UI; // Đảm bảo namespace đúng

namespace SettingsUI.UI
{
    public partial class FormSettings : Form
    {
        // UserControls
        private BaseSettingControl ucAccount;
        private BaseSettingControl ucData;
        private BaseSettingControl ucGeneral;

        // Controls
        private Panel panelMain;
        private Button btnAccount;
        private Button btnData;
        private Button btnGeneral;
        private Button btnBack;

        public FormSettings()
        {
            InitializeComponent();
            this.Load += FormSettings_Load; // Quan trọng: Load giao diện khi form hiện
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            ShowDashboard(); // BẮT BUỘC gọi ở đây
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form chính
            this.Text = "Cài đặt hệ thống";
            this.Size = new Size(1200, 1000);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Panel chính chứa nội dung
            panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            this.Controls.Add(panelMain);

            // Tạo các nút menu
            btnAccount = CreateMenuButton("Tài khoản");
            btnData = CreateMenuButton("Dữ liệu");
            btnGeneral = CreateMenuButton("Chung");

            // Nút Back (ban đầu ẩn)
            btnBack = new Button
            {
                Text = "Quay lại",
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnBack.Click += (s, e) => ShowDashboard();

            // Sự kiện click các nút
            btnAccount.Click += (s, e) => ShowUserControl(GetAccountUC());
            btnData.Click += (s, e) => ShowUserControl(GetDataUC());
            btnGeneral.Click += (s, e) => ShowUserControl(GetGeneralUC());

            this.ResumeLayout(false);
        }

        private Button CreateMenuButton(string text)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(340, 95),
                BackColor = Color.FromArgb(230, 230, 230),        // XÁM NHẠT SIÊU ĐẸP, NHẸ MẮT
                ForeColor = Color.FromArgb(50, 50, 50),           // Chữ xám đậm, dễ đọc
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btn.FlatAppearance.BorderSize = 0;

            // Hiệu ứng hover: chuyển xám đậm nhẹ (rất tinh tế)
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(210, 210, 210);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(190, 190, 190);

            // Bo góc tròn mượt (20px)
            btn.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 20, 20));

            return btn;
        }

        private void ShowDashboard()
        {
            panelMain.Controls.Clear();

            // Thêm 3 nút vào giữa
            panelMain.Controls.Add(btnAccount);
            panelMain.Controls.Add(btnData);
            panelMain.Controls.Add(btnGeneral);

            int centerX = (panelMain.Width - 300) / 2;
            btnAccount.Location = new Point(centerX, 100);
            btnData.Location = new Point(centerX, 200);
            btnGeneral.Location = new Point(centerX, 300);

            btnBack.Visible = false;

            this.Text = "Cài đặt hệ thống";
        }

        private void ShowUserControl(BaseSettingControl uc)
        {
            panelMain.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);

            // Hiện nút Back
            btnBack.Location = new Point(15, 15);
            btnBack.Visible = true;
            panelMain.Controls.Add(btnBack);
            panelMain.Controls.SetChildIndex(btnBack, 0); // Đưa lên trên cùng

            this.Text = "Cài đặt hệ thống - " + uc.GetType().Name.Replace("UserControl", "");
        }

        // Lazy load UserControl (chỉ tạo khi cần)
        private BaseSettingControl GetAccountUC()
        {
            if (ucAccount == null)
            {
                ucAccount = new UserControlAccount();
                ucAccount.BackClicked += (s, e) => ShowDashboard();
            }
            return ucAccount;
        }

        private BaseSettingControl GetDataUC()
        {
            if (ucData == null)
            {
                ucData = new UserControlData();
                ucData.BackClicked += (s, e) => ShowDashboard();
            }
            return ucData;
        }

        private BaseSettingControl GetGeneralUC()
        {
            if (ucGeneral == null)
            {
                ucGeneral = new UserControlGeneral();
                ucGeneral.BackClicked += (s, e) => ShowDashboard();
            }
            return ucGeneral;
        }

        // Bo góc button
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);
    }
}