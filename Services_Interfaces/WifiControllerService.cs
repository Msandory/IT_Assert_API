using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Services_Interfaces
{
    public class WifiControllerService
    {
        private readonly DataContex _context;
        public WifiControllerService(DataContex context)
        {
            _context = context;
        }

        // Add WifiController
        public void AddWifiController(WifiController wifiController)
        {
            if (_context.WifiControllers.Any(w => w.Serialnumber == wifiController.Serialnumber))
            {
                throw new Exception("WifiController with the same SerialNumber already exists");
            }

            var wc = new WifiController
            {
                Name = wifiController.Name,
                Model = wifiController.Model,
                Serialnumber = wifiController.Serialnumber,
                MacAddress = wifiController.MacAddress,
                IpAddress = wifiController.IpAddress,
                FirmwareVersion = wifiController.FirmwareVersion,
                ConnectedApCount = wifiController.ConnectedApCount,
                Status = wifiController.Status,
            };
            _context.WifiControllers.Add(wc);
            _context.SaveChanges();
        }

        public bool WifiControllerExists(int id)
        {
            return _context.WifiControllers.Any(w => w.Id == id);
        }

        public async Task<WifiController> EditWifiControllerAsync(int id, [FromBody] WifiController wifiController)
        {
            var toUpdate = await _context.WifiControllers.FindAsync(id);
            if (toUpdate == null)
            {
                throw new KeyNotFoundException("WifiController not found");
            }

            toUpdate.Name = wifiController.Name;
            toUpdate.Model = wifiController.Model;
            toUpdate.Serialnumber = wifiController.Serialnumber;
            toUpdate.MacAddress = wifiController.MacAddress;
            toUpdate.IpAddress = wifiController.IpAddress;
            toUpdate.FirmwareVersion = wifiController.FirmwareVersion;
            toUpdate.ConnectedApCount = wifiController.ConnectedApCount;
            toUpdate.Status = wifiController.Status;

            await _context.SaveChangesAsync();
            return toUpdate;
        }

        public void DeleteWifiController(int wifiControllerId)
        {
            var wifiController = _context.WifiControllers.SingleOrDefault(w => w.Id == wifiControllerId);
            if (wifiController == null)
            {
                throw new Exception("WifiController not found.");
            }

            _context.WifiControllers.Remove(wifiController);
            _context.SaveChanges();
        }
    }

}
