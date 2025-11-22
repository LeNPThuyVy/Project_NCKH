using System;
using Npgsql;
using SettingsUI.Models;

namespace SettingsUI.DAL
{
    public class Database
    {
        private readonly string connectionString = "Host=localhost;Port=5433;Username=postgres;Password=123456;Database=postgres";

        public Account GetAccountById(int userId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(
                    "SELECT user_id, username, pass, staff_id, role FROM user_account WHERE user_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Account acc = new Account
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Password = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                StaffId = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Role = reader.IsDBNull(4) ? "Staff" : reader.GetString(4)
                            };
                            return acc;
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdatePassword(int userId, string newPassword)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE user_account SET pass = @p WHERE user_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@p", newPassword);
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool IsUsernameExists(string username, int excludeUserId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM user_account WHERE username = @u AND (user_id <> @id OR @id = -1)", conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@id", excludeUserId);
                    long count = Convert.ToInt64(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool UpdateAccount(int userId, string username, string role)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(
                    "UPDATE user_account SET username = @u, role = @r WHERE user_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@r", role);
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}