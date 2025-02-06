namespace Inventory_System_API.Models
{
    public class DeviceStatus
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
    public class DeviceStatusResponse
    {
        public string DeviceType { get; set; }
        public List<DeviceStatus> StatusCounts { get; set; }
    }

}
