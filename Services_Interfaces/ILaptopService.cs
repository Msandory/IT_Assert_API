using Inventory_System_API.Models;

namespace Inventory_System_API.Services_Interfaces
{
    public interface ILaptopService
    {
        void AddLaptop(LaptopDto laptopDto);
        void UpdateLaptop(LaptopDto laptopDto);
        void DeleteLaptop(int laptopid);
      


    }
}
