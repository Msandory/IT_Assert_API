namespace Inventory_System_API.Models
{
    public class OwnerDTO
    {
        public int Id { get; set; }
        public string name { get; set; }
        public List<string>? Filepath { get; set; }
        public string department { get; set; }
        public string designation { get; set; }
    }
}
