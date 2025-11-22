using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Npgsql;

namespace QLBGX
{
    public partial class QLX : Form
    {
        private const string PLACEHOLDER_TEXT = "Nhập biển số...";
        private Color PLACEHOLDER_COLOR = Color.Gray;
        private Color FOREGROUND_COLOR = Color.Black;
        public QLX()
        {
            InitializeComponent();
            loadData_Xe();
            Load_TextBoxSearch();

        }

        private void btnQLRaVao_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loadData_Xe()
        {
            DataProvider dataProvider = new DataProvider();
            string query = "SELECT license_plate, time_check_in FROM parking_info";
            DataTable dataTable = dataProvider.ExecuteQuery(query);
            fomat_DataGridView(dataTable);
            dataGridView.DataSource = dataTable;
        }

        private string fotmat_Time(TimeSpan timeSpan)
        {
            return string.Format("{0}h:{1}m", (int)timeSpan.TotalHours, timeSpan.Minutes);
        }



        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Load_TextBoxSearch()
        {

            textBoxSearch.Text = PLACEHOLDER_TEXT;
            textBoxSearch.ForeColor = PLACEHOLDER_COLOR;
            textBoxSearch.GotFocus += textBoxSearch_Enter;
            textBoxSearch.LostFocus += textBoxSearch_Leave;
        }

        private void textBoxSearch_Enter(object sender, EventArgs e)
        {
            if (textBoxSearch.Text == PLACEHOLDER_TEXT)
            {
                textBoxSearch.Text = "";
                textBoxSearch.ForeColor = FOREGROUND_COLOR;
            }

        }

        private void textBoxSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                textBoxSearch.Text = PLACEHOLDER_TEXT;
                textBoxSearch.ForeColor = PLACEHOLDER_COLOR;
            }
        }

        private void fomat_DataGridView(DataTable tb)
        {
            tb.Columns.Add("time_in_park", typeof(String));
            DateTime currentTime = DateTime.Now;
            foreach (DataRow row in tb.Rows)
            {
                DateTime checkInTime = row.Field<DateTime>("time_check_in");
                TimeSpan duration = currentTime.Subtract(checkInTime);
                string timeFormatted = fotmat_Time(duration);
                row["time_in_park"] = timeFormatted;
            }
            BienSo.DataPropertyName = "license_plate";
            GioVao.DataPropertyName = "time_check_in";
            ThoiGian.DataPropertyName = "time_in_park";
        }

        private void tbSearch()
        {
            DataProvider dataProvider = new DataProvider();
            string searchText = textBoxSearch.Text;
            if (searchText == PLACEHOLDER_TEXT)
            {
                searchText = "";
            }
            string query = $"SELECT license_plate, time_check_in FROM parking_info WHERE license_plate LIKE '%{searchText}%'";
            DataTable dataTable = dataProvider.ExecuteQuery(query);
            fomat_DataGridView(dataTable);
            dataGridView.DataSource = dataTable;
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                tbSearch();
                Load_TextBoxSearch();
                e.Handled = true;
                e.SuppressKeyPress = true;
                dataGridView.Focus();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            tbSearch();
            Load_TextBoxSearch();
        }

      
    }
}
