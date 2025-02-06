namespace Inventory_System_API.Models
{
    public class DeviceDTO
    {
        public string ComputerName { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
    }

    public class CpuTypeSummary
    {
        public string CpuType { get; set; }
        public int Count { get; set; }
    }
    public class DeviceMakeSummary
    {
        public string Make { get; set; }
        public string DeviceType { get; set; }
        public int Count { get; set; }
    }
    public class AgeDistribution
    {
        public string AgeRange { get; set; }
        public int Count { get; set; }
        public int Laptop { get; set; }
        public int Desktop { get; set; }
        public int Tablet { get; set; }
        public int Mobile { get; set; }
        
    }

}
