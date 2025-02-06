using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Services_Interfaces
{
    public class AccessPointService
    {
        private readonly DataContex _context;
        public AccessPointService(DataContex context)
        {
            _context = context;
        }

        // Add AccessPoint
        public void AddAccessPoint(AccessPoint accessPoint)
        {
            if (_context.AccessPoints.Any(a => a.Serialnumber == accessPoint.Serialnumber))
            {
                throw new Exception("AccessPoint with the same SerialNumber already exists");
            }

            var ap = new AccessPoint
            {
                Name= accessPoint.Name,
                Location = accessPoint.Location,
                Model = accessPoint.Model,
                Serialnumber = accessPoint.Serialnumber,
                MacAddress = accessPoint.MacAddress,
                IpAddress = accessPoint.IpAddress,
                ConnectedSwitch = accessPoint.ConnectedSwitch,
                PortNumber = accessPoint.PortNumber,
                Status = accessPoint.Status,
            };
            _context.AccessPoints.Add(ap);
            _context.SaveChanges();
        }

        public bool AccessPointExists(int id)
        {
            return _context.AccessPoints.Any(a => a.Id == id);
        }

        public async Task<AccessPoint> EditAccessPointAsync(int id, [FromBody] AccessPoint accessPoint)
        {
            var toUpdate = await _context.AccessPoints.FindAsync(id);
            if (toUpdate == null)
            {
                throw new KeyNotFoundException("AccessPoint not found");
            }

            toUpdate.Location = accessPoint.Location;
            toUpdate.Model = accessPoint.Model;
            toUpdate.Serialnumber = accessPoint.Serialnumber;
            toUpdate.MacAddress = accessPoint.MacAddress;
            toUpdate.IpAddress = accessPoint.IpAddress;
            toUpdate.ConnectedSwitch = accessPoint.ConnectedSwitch;
            toUpdate.PortNumber = accessPoint.PortNumber;
            toUpdate.Name = accessPoint.Name;
            toUpdate.Status = accessPoint.Status;

            await _context.SaveChangesAsync();
            return toUpdate;
        }

        public void DeleteAccessPoint(int accessPointId)
        {
            var accessPoint = _context.AccessPoints.SingleOrDefault(a => a.Id == accessPointId);
            if (accessPoint == null)
            {
                throw new Exception("AccessPoint not found.");
            }

            _context.AccessPoints.Remove(accessPoint);
            _context.SaveChanges();
        }
    }

}
