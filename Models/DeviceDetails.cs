namespace Inventory_System_API.Models
{
    public class DeviceDetails
    {
        public string DeviceType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public int Age { get; set; } // Age in years


    }
    public class DeviceDetailsByStatus
    {
        public string DeviceType { get; set; }
        public int Count { get; set; }
        public string Status { get; set; }
    }

    public class DeviceDetailsByOS
    {
        public string DeviceType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string OS { get; set; }

    }
    public class DeviceDetailsByWarrenty
    {
        public string DeviceType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string WarrentyStatus { get; set; }
        public DateOnly WarrentyStart { get; set; }
        public DateOnly WarrentyEnd { get; set; }

    }
    public class DeviceTypeConfig
    {
        public Type EntityType { get; set; }
        public string Name { get; set; }
        public string DbSetPropertyName { get; set; }
        public string StatusPropertyName { get; set; }
    }

}
