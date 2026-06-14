namespace Lift.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Operator"; // "Admin" sau "Operator"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}