using Npgsql;
using System;
using System.Data;

namespace ReportUI.DAL
{
    public class ParkingRepository
    {
        public int CountAll()
        {
            var conn = Database.GetConnection();
            conn.Open();
            string sql = "SELECT COUNT(*) FROM PARKING_INFO";

            var cmd = new NpgsqlCommand(sql, conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int CountMoto()
        {
            var conn = Database.GetConnection();
            conn.Open();
            string sql = "SELECT COUNT(*) FROM PARKING_INFO WHERE LICENSE_PLATE LIKE 'XM%'";
            var cmd = new NpgsqlCommand(sql, conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int CountCar()
        {
            var conn = Database.GetConnection();
            conn.Open();
            string sql = "SELECT COUNT(*) FROM PARKING_INFO WHERE LICENSE_PLATE LIKE 'OTO%'";
            var cmd = new NpgsqlCommand(sql, conn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public DataTable LoadColumnChart(string sql, params NpgsqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            var conn = Database.GetConnection();
            conn.Open();

            var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);

            var adapter = new NpgsqlDataAdapter(cmd);
            adapter.Fill(dt);

            return dt;
        }
    }
}

