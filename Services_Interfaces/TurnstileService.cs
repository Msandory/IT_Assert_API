using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Services_Interfaces
{
    public class TurnstileService
    {
        private readonly DataContex _context;
        public TurnstileService(DataContex context)
        {
            _context = context;
        }

        // Add Turnstile
        public void AddTurnstile(Turnstile turnstile)
        {
            if (_context.Turnstiles.Any(t => t.Serialnumber == turnstile.Serialnumber))
            {
                throw new Exception("Turnstile with the same SerialNumber already exists");
            }

            var ts = new Turnstile
            {
                DeviceName = turnstile.DeviceName,
                Serialnumber = turnstile.Serialnumber,
                Status = turnstile.Status,
                AreaName = turnstile.AreaName,
                NetworkConnectionType = turnstile.NetworkConnectionType,
                IpAddress = turnstile.IpAddress,
                DeviceModel = turnstile.DeviceModel,
                IsRegistered = turnstile.IsRegistered,
                FirmwareVersion = turnstile.FirmwareVersion,
            };
            _context.Turnstiles.Add(ts);
            _context.SaveChanges();
        }

        public bool TurnstileExists(int id)
        {
            return _context.Turnstiles.Any(t => t.Id == id);
        }

        public async Task<Turnstile> EditTurnstileAsync(int id, [FromBody] Turnstile turnstile)
        {
            var toUpdate = await _context.Turnstiles.FindAsync(id);
            if (toUpdate == null)
            {
                throw new KeyNotFoundException("Turnstile not found");
            }

            toUpdate.DeviceName = turnstile.DeviceName;
            toUpdate.Serialnumber = turnstile.Serialnumber;
            toUpdate.Status = turnstile.Status;
            toUpdate.AreaName = turnstile.AreaName;
            toUpdate.NetworkConnectionType = turnstile.NetworkConnectionType;
            toUpdate.IpAddress = turnstile.IpAddress;
            toUpdate.DeviceModel = turnstile.DeviceModel;
            toUpdate.IsRegistered = turnstile.IsRegistered;
            toUpdate.FirmwareVersion = turnstile.FirmwareVersion;

            await _context.SaveChangesAsync();
            return toUpdate;
        }

        public void DeleteTurnstile(int turnstileId)
        {
            var turnstile = _context.Turnstiles.SingleOrDefault(t => t.Id == turnstileId);
            if (turnstile == null)
            {
                throw new Exception("Turnstile not found.");
            }

            _context.Turnstiles.Remove(turnstile);
            _context.SaveChanges();
        }
    }

}
