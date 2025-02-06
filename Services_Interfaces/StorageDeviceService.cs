using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Services_Interfaces
{
    public class StorageDeviceService
    {
        private readonly DataContex _context;
        public StorageDeviceService(DataContex context)
        {
            _context = context;
        }

        // Add StorageDevice
        public void AddStorageDevice(StorageDevice storageDevice)
        {
            if (_context.StorageDevices.Any(s => s.Serialnumber == storageDevice.Serialnumber))
            {
                throw new Exception("StorageDevice with the same SerialNumber already exists");
            }

            var sd = new StorageDevice
            {
                Name = storageDevice.Name,
                Model = storageDevice.Model,
                Serialnumber = storageDevice.Serialnumber,
                Type=storageDevice.Type,
                Status = storageDevice.Status,
                IpAddress = storageDevice.IpAddress,
                Capacity = storageDevice.Capacity,
                MacAddress = storageDevice.MacAddress,
                FirmwareVersion = storageDevice.FirmwareVersion,
            };
            _context.StorageDevices.Add(sd);
            _context.SaveChanges();
        }

        public bool StorageDeviceExists(int id)
        {
            return _context.StorageDevices.Any(s => s.Id == id);
        }

        public async Task<StorageDevice> EditStorageDeviceAsync(int id, [FromBody] StorageDevice storageDevice)
        {
            var toUpdate = await _context.StorageDevices.FindAsync(id);
            if (toUpdate == null)
            {
                throw new KeyNotFoundException("StorageDevice not found");
            }

            toUpdate.Name = storageDevice.Name;
            toUpdate.Model = storageDevice.Model;
            toUpdate.Serialnumber = storageDevice.Serialnumber;
            toUpdate.Status = storageDevice.Status;
            toUpdate.IpAddress = storageDevice.IpAddress;
            toUpdate.MacAddress = storageDevice.MacAddress;
            toUpdate.FirmwareVersion = storageDevice.FirmwareVersion;
            toUpdate.Type = storageDevice.Type;
            toUpdate.Capacity = storageDevice.Capacity;

            await _context.SaveChangesAsync();
            return toUpdate;
        }

        public void DeleteStorageDevice(int storageDeviceId)
        {
            var storageDevice = _context.StorageDevices.SingleOrDefault(s => s.Id == storageDeviceId);
            if (storageDevice == null)
            {
                throw new Exception("StorageDevice not found.");
            }

            _context.StorageDevices.Remove(storageDevice);
            _context.SaveChanges();
        }
    }

}
