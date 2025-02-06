namespace Inventory_System_API.Services_Interfaces
{
    public interface IDevice
    {
        public int Id { get; set; }
        public string Serialnumber { get; set; }
        public string Status { get; set; }
    }
}
