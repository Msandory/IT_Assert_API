namespace Inventory_System_API.Models
{
    public class Logins
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LastActivityTime { get; set; }
    }
}
