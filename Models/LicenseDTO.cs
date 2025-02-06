namespace Inventory_System_API.Models
{
    public class LicenseDTO
    {
        public int ID { get; set; }
        public string Product { get; set; }
        public string Vendor { get; set; }
        public string LicenseType { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string ContactEmail { get; set; }
    }
}
