using Microsoft.Extensions.Configuration;
using Npgsql;
using Supabase; 
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QLBGX
{
    public class DataProvider
    {

        private string connStr = "Host=localhost;Port=5432;Database=NCKH;Username=postgres;Password=postgres;";
        
        public DataTable ExecuteQuery(string query)
        {
            DataTable data = new DataTable();
            try {
                using (NpgsqlConnection connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    adapter.Fill(data);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loi ket noi: {ex.Message}");
            } 

            return data;
        }

        public void ConnectToDatabase()
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Kết nối đến Supabase thành công!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi kết nối: {ex.Message}");
                }
            }
        }
    }
}
