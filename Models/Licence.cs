namespace Inventory_System_API.Models
{
    public class Licence
    {
        public int id {  get; set; }

        public string product { get; set; }
        public string Vendor { get; set; }
        public string LicenseType { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public string ContactEmail { get; set; }
    }
}
