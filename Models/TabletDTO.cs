namespace Inventory_System_API.Models
{
    public class TabletDTO
    {
        public int ID { get;  set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Serialnumber { get; set; }
        public string ramsize { get; set; }
        public string OS { get; set; }
        public string storagesize { get; set; }
        public string? comments { get; set; }
        public string? Upgrades { get; set; }
        public DateOnly? DOP { get; set; }
        // Calculated property
        
        public decimal cost { get; set; }
        // Warranty properties
        public DateOnly? WarrantyStartDate { get; set; }
        public DateOnly? WarrantyEndDate { get; set; }
        public string? WarrantyTerms { get; set; } = string.Empty;
        public string status { get; set; }
        
        // Foreign key
       
        // Navigation property
        
        public string name { get; set; }
        
    }
}
