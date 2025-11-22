namespace SettingsUI.Models
{
    public class Account
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string StaffId { get; set; }
        public string Role { get; set; }

        public Account()
        {
            Username = string.Empty;
            Password = string.Empty;
            StaffId = string.Empty;
            Role = "Staff";
        }
    }
}