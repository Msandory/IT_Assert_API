using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Services_Interfaces
{
    public class BiometricDeviceService
    {
        private readonly DataContex _context;
        public BiometricDeviceService(DataContex context)
        {
            _context = context;
        }

        // Add BiometricDevice
        public void AddBiometricDevice(BiometricDevice biometricDevice)
        {
            if (_context.BiometricDevices.Any(b => b.Serialnumber == biometricDevice.Serialnumber))
            {
                throw new Exception("BiometricDevice with the same SerialNumber already exists");
            }

            var bd = new BiometricDevice
            {
                DeviceName = biometricDevice.DeviceName,
                Model = biometricDevice.Model,
                Status = biometricDevice.Status,
                IpAddress = biometricDevice.IpAddress,
                Site = biometricDevice.Site,
                Manufacturer = biometricDevice.Manufacturer,
                MacAddress = biometricDevice.MacAddress,
                Serialnumber = biometricDevice.Serialnumber,
            };
            _context.BiometricDevices.Add(bd);
            _context.SaveChanges();
        }

        public bool BiometricDeviceExists(int id)
        {
            return _context.BiometricDevices.Any(b => b.Id == id);
        }

        public async Task<BiometricDevice> EditBiometricDeviceAsync(int id, [FromBody] BiometricDevice biometricDevice)
        {
            var toUpdate = await _context.BiometricDevices.FindAsync(id);
            if (toUpdate == null)
            {
                throw new KeyNotFoundException("BiometricDevice not found");
            }

            toUpdate.DeviceName = biometricDevice.DeviceName;
            toUpdate.Status = biometricDevice.Status;
            toUpdate.IpAddress = biometricDevice.IpAddress;
            toUpdate.Site = biometricDevice.Site;
            toUpdate.Manufacturer = biometricDevice.Manufacturer;
            toUpdate.MacAddress = biometricDevice.MacAddress;
            toUpdate.Serialnumber = biometricDevice.Serialnumber;
            toUpdate.Model = biometricDevice.Model;   

            await _context.SaveChangesAsync();
            return toUpdate;
        }

        public void DeleteBiometricDevice(int biometricDeviceId)
        {
            var biometricDevice = _context.BiometricDevices.SingleOrDefault(b => b.Id == biometricDeviceId);
            if (biometricDevice == null)
            {
                throw new Exception("BiometricDevice not found.");
            }

            _context.BiometricDevices.Remove(biometricDevice);
            _context.SaveChanges();
        }
    }

}
