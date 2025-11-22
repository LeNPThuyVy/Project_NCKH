using SettingsUI.DAL;
using SettingsUI.Models;

namespace SettingsUI.BLL
{
    public class AccountService
    {
        private readonly Database db = new Database();

        public Account LoadAccount(int userId)
        {
            if (userId <= 0) return null;
            return db.GetAccountById(userId);
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword)) return false;
            return db.UpdatePassword(userId, newPassword);
        }

        public (bool Success, string Message) UpdateAccountInfo(int userId, string newUsername, string role, string currentPassword, string inputPassword)
        {
            if (inputPassword != currentPassword)
                return (false, "Mật khẩu xác nhận không đúng!");

            if (string.IsNullOrWhiteSpace(newUsername))
                return (false, "Username không được để trống!");

            if (db.IsUsernameExists(newUsername, userId))
                return (false, "Username đã tồn tại!");

            bool ok = db.UpdateAccount(userId, newUsername, role);
            return ok ? (true, "Cập nhật thành công!") : (false, "Lỗi khi cập nhật CSDL");
        }
    }
}