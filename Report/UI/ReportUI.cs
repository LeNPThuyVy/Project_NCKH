using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ReportUI.BLL;

namespace ReportUI.UI
{
    public class ReportUI : Form
    {
        // ================== Controls ==================
        private ComboBox cmbFilterType, cmbMonth, cmbQuarter;
        private DateTimePicker dtpDay;
        private Chart chartColumn, chartPie;
        private Label lblTotalTitle, lblTotalValue, lblMoto, lblCar;
        private Button btnViewReport;

        // ================== Service ==================
        private ReportService service = new ReportService();

        public ReportUI()
        {
            BuildUI();
        }

        // ================== Build UI ==================
        private void BuildUI()
        {
            this.Text = "Báo cáo thống kê";
            this.Size = new Size(1100, 700);

            // ===== Filter Type =====
            cmbFilterType = new ComboBox();
            cmbFilterType.Items.AddRange(new string[] { "Ngày", "Tháng", "Quý" });
            cmbFilterType.SelectedIndex = 0;
            cmbFilterType.Location = new Point(20, 20);
            cmbFilterType.SelectedIndexChanged += CmbFilterType_SelectedIndexChanged;
            this.Controls.Add(cmbFilterType);

            // ===== Pick Day =====
            dtpDay = new DateTimePicker()
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(150, 20)
            };
            this.Controls.Add(dtpDay);

            // ===== Month Combo =====
            cmbMonth = new ComboBox()
            {
                Location = new Point(150, 20),
                Visible = false
            };
            cmbMonth.Items.AddRange(Enumerable.Range(1, 12).Select(x => "Tháng " + x).ToArray());
            cmbMonth.SelectedIndex = 0;
            this.Controls.Add(cmbMonth);

            // ===== Quarter Combo =====
            cmbQuarter = new ComboBox()
            {
                Location = new Point(150, 20),
                Visible = false
            };
            cmbQuarter.Items.AddRange(new string[] { "Quý 1", "Quý 2", "Quý 3", "Quý 4" });
            cmbQuarter.SelectedIndex = 0;
            this.Controls.Add(cmbQuarter);

            // ===== Tổng số phương tiện =====
            lblTotalTitle = new Label()
            {
                Text = "Tổng số phương tiện:",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            this.Controls.Add(lblTotalTitle);

            lblTotalValue = new Label()
            {
                Text = "0",
                Location = new Point(220, 60),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            this.Controls.Add(lblTotalValue);

            // ===== Số lượng xe máy - ô tô =====
            lblMoto = new Label()
            {
                Text = "Xe máy: 0",
                Location = new Point(20, 90),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };
            this.Controls.Add(lblMoto);

            lblCar = new Label()
            {
                Text = "Ô tô: 0",
                Location = new Point(20, 120),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            this.Controls.Add(lblCar);

            // ===== Nút Xem Báo Cáo =====
            btnViewReport = new Button()
            {
                Text = "Xem báo cáo",
                Location = new Point(20, 150),
                Size = new Size(150, 30)
            };
            btnViewReport.Click += BtnViewReport_Click;
            this.Controls.Add(btnViewReport);

            // ===== Biểu đồ cột =====
            chartColumn = new Chart();
            chartColumn.Location = new Point(20, 200);
            chartColumn.Size = new Size(500, 450);

            ChartArea columnArea = new ChartArea("ColumnArea");
            chartColumn.ChartAreas.Add(columnArea);

            Series columnSeries = new Series("Số lượng xe theo thời gian");
            columnSeries.ChartType = SeriesChartType.Column;
            columnSeries.XValueType = ChartValueType.String;
            chartColumn.Series.Add(columnSeries);

            chartColumn.Titles.Add("Biểu đồ cột - Số lượng xe theo thời gian");
            this.Controls.Add(chartColumn);

            // ===== Biểu đồ tròn =====
            chartPie = new Chart();
            chartPie.Location = new Point(550, 200);
            chartPie.Size = new Size(500, 450);

            ChartArea pieArea = new ChartArea("PieArea");
            chartPie.ChartAreas.Add(pieArea);

            Series pieSeries = new Series("Phân loại xe");
            pieSeries.ChartType = SeriesChartType.Pie;
            chartPie.Series.Add(pieSeries);

            chartPie.Titles.Add("Biểu đồ tròn - Tỷ lệ loại xe");
            this.Controls.Add(chartPie);
        }

        private void CmbFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtpDay.Visible = cmbFilterType.SelectedIndex == 0;
            cmbMonth.Visible = cmbFilterType.SelectedIndex == 1;
            cmbQuarter.Visible = cmbFilterType.SelectedIndex == 2;
        }

        private void BtnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var (total, moto, car) = service.LoadVehicleCount();

                lblTotalValue.Text = total.ToString();
                lblMoto.Text = $"Xe máy: {moto}";
                lblCar.Text = $"Ô tô: {car}";

                DataTable dt = service.GetColumnChart(
                    cmbFilterType.SelectedIndex,
                    dtpDay.Value,
                    cmbMonth.SelectedIndex + 1,
                    cmbQuarter.SelectedIndex + 1
                );

                RenderColumnChart(dt);
                RenderPieChart(moto, car);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void RenderColumnChart(DataTable dt)
        {
            chartColumn.Series[0].Points.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string xValue = "";
                double yValue = Convert.ToDouble(row["Count"]);

                if (cmbFilterType.SelectedIndex == 0) xValue = $"{row["Hour"]}h";
                else if (cmbFilterType.SelectedIndex == 1) xValue = "Ngày " + row["Day"];
                else xValue = "Tháng " + row["Month"];

                chartColumn.Series[0].Points.AddXY(xValue, yValue);
            }
        }

        private void RenderPieChart(int moto, int car)
        {
            chartPie.Series[0].Points.Clear();
            if (moto > 0) chartPie.Series[0].Points.AddXY("Xe máy", moto);
            if (car > 0) chartPie.Series[0].Points.AddXY("Ô tô", car);
        }
    }
}
