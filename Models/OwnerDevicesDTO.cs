namespace Inventory_System_API.Models
{
    public class OwnerDevicesDTO
    {
       
        
        public List<DeviceDTO> Laptops { get; set; }
        public List<DeviceDTO> Desktops { get; set; }
        public List<DeviceDTO> Mobilephones { get; set; }
        public List<DeviceDTO> Tablets { get; set; }
    }
}
