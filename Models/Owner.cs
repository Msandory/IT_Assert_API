using static NetTopologySuite.Geometries.Utilities.GeometryMapper;

namespace Inventory_System_API.Models
{
    public class Owner
    {
        public int id { get; set; }
        public string name { get; set; }
        
        public string department { get; set; }
        public string designation { get; set; }
        public bool IsDeleted { get; set; }
        public List<string>? Filepath { get; set; }

        // Navigation properties
        public ICollection<Laptop> Laptop { get; set; }
        public ICollection<Desktop> Desktop { get; set; }
        public ICollection<Mobilephones> Mobilephones { get; set; }
        public ICollection<Tablets> Tablets { get; set; }

    }
}
