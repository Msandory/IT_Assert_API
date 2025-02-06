using Inventory_System_API.Services_Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Inventory_System_API.Models
{
   
    public class PhysicalServer : IDevice
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Status { get; set; }
        public string Model { get; set; }
        public string IpAddress { get; set; }
        public string Serialnumber { get; set; }
        public string MacAddress { get; set; }
        public string ProcessorSpecifications { get; set; }
        public string RamAmount { get; set; }

    }

    public class Switch :IDevice
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Site { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Serialnumber { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string PoeCapability { get; set; }
        public int PortsUsed { get; set; }
        public string FirmwareVersion { get; set; }
    }

    public class Printer :IDevice
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Serialnumber { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string Credentials { get; set; }
        public string Status { get; set; }
        public string Toner { get; set; }
    }

    public class Firewall : IDevice
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Serialnumber { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string FirmwareVersion { get; set; }
        public string Status { get; set; }
    }

    public class WifiController : IDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Serialnumber { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public string FirmwareVersion { get; set; }
        public int ConnectedApCount { get; set; }
       
        public string Status { get; set; }
    }

    public class AccessPoint : IDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Model { get; set; }
        public string Serialnumber { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public string ConnectedSwitch { get; set; }
        public string PortNumber { get; set; }
        public string Status { get; set; }
    }

    public class StorageDevice : IDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Serialnumber { get; set; }
        public string Capacity { get; set; }
        public string Status { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string FirmwareVersion { get; set; }
    }

    public class Turnstile : IDevice
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string Serialnumber { get; set; }
        public string Status { get; set; }
        public string AreaName { get; set; }
        public string NetworkConnectionType { get; set; }
        public string IpAddress { get; set; }
        public string DeviceModel { get; set; }
        public string IsRegistered { get; set; }
        public string FirmwareVersion { get; set; }
    }

    public class BiometricDevice : IDevice
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string Model { get; set; }
        public string Status { get; set; }
        public string IpAddress { get; set; }
        public string Site { get; set; }
        public string Manufacturer { get; set; }
        public string MacAddress { get; set; }
        public string Serialnumber { get; set; }
    }

    public class Toner
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Printer { get; set; }

        public int Count { get; set; }
        public DateOnly? lastIssued { get; set; }
    }
}
