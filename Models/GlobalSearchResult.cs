namespace Inventory_System_API.Models
{
    public class GlobalSearchResult
    {
        public List<OwnerDTO> Owners { get; set; }
        public List<LaptopDto> Laptops { get; set; }
        public List<DesktopDto> Desktops { get; set; }
        public List<MobileDTO> Mobilephones { get; set; }
        public List<TabletDTO> Tablets { get; set; }
        public List<LicenseDTO> Licences { get; set; }
        public List<PhysicalServer> PhysicalServers { get; set; }
        public List<Switch> Switches { get; set; }
        public List<Printer> Printers { get; set; }
        public List<Firewall> Firewalls { get; set; }
        public List<WifiController> WifiControllers { get; set; }
        public List<AccessPoint> AccessPoints { get; set; }
        public List<StorageDevice> StorageDevices { get; set; }
        public List<Turnstile> Turnstiles { get; set; }
        public List<BiometricDevice> BiometricDevices { get; set; }

        public GlobalSearchResult()
        {
            Owners = new List<OwnerDTO>();
            Laptops = new List<LaptopDto>();
            Desktops = new List<DesktopDto>();
            Mobilephones = new List<MobileDTO>();
            Tablets = new List<TabletDTO>();
            Licences = new List<LicenseDTO>();
            PhysicalServers = new List<PhysicalServer>();
            Switches = new List<Switch>();
            Printers = new List<Printer>();
            Firewalls = new List<Firewall>();
            WifiControllers = new List<WifiController>();
            AccessPoints = new List<AccessPoint>();
            StorageDevices = new List<StorageDevice>();
            Turnstiles = new List<Turnstile>();
            BiometricDevices = new List<BiometricDevice>();
        }
    }

}
