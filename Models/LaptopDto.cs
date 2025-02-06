namespace Inventory_System_API.Models
{
    public class LaptopDto
    {
        public int ID { get; set; }
        public string ComputerName { get; set; } = string.Empty;
        public string Make { get; set; }
        public string Model { get; set; }
        public string serialnumber { get; set; }
        public string ramsize { get; set; }
        public string OS { get; set; }
        public string CPUModel { get; set; }
        public string storagesize { get; set; }
        public string StorageType { get; set; }
        public string? Upgrades { get; set; }
        public string? comments { get; set; }
        public DateOnly? DOP { get; set; }
       
        public decimal? cost { get; set; }
        public string Status { get; set; }
        
        // Warranty properties
        public DateOnly? WarrantyStartDate { get; set; }
        public DateOnly? WarrantyEndDate { get; set; }
        public string? WarrantyTerms { get; set; } = string.Empty;
        public string name { get; set; }
        



        //for updating on search results




    }
}
