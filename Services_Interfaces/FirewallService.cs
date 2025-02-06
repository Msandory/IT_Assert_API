using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Services_Interfaces
{
    public class FirewallService
    {
        private readonly DataContex _contex;
        public FirewallService(DataContex contex)
        {
            _contex = contex;
        }

        //Add FirewallDevice
        public void AddFirewall(Firewall firewall)
        {
            //check if Serialnumber exist
            if (_contex.Firewalls.Any(s => s.Serialnumber == firewall.Serialnumber))
            {
                throw new Exception("Firewall Device with Same Serialnumber already exists");
            }

            var d = new Firewall
            {
                Location = firewall.Location,
                Brand = firewall.Brand,
                Name   = firewall.Name,
                Model = firewall.Model,
                IpAddress = firewall.IpAddress,
                Serialnumber = firewall.Serialnumber,
                MacAddress = firewall.MacAddress,
                FirmwareVersion = firewall.FirmwareVersion,
                Status = firewall.Status,

            };
            _contex.Firewalls.Add(d);
            _contex.SaveChanges();

        }

        //check if fIREWALL is in the list
        public bool FirewallExists(int id)
        {
            return _contex.Firewalls.Any(e => e.Id == id);
        }

        //Update Firewall
        public async Task<Firewall> EditFirewallAsync(int id, [FromBody] Firewall firewall)
        {
            var ToUpdate = await _contex.Firewalls.FindAsync(id);
            if (ToUpdate == null)
            {
                throw new KeyNotFoundException("Server not found");
            }
            // Update the properties of the Server entity
            ToUpdate.Location = firewall.Location;
            ToUpdate.Brand = firewall.Brand;
            ToUpdate.Model = firewall.Model;
            ToUpdate.IpAddress = firewall.IpAddress;
            ToUpdate.Serialnumber = firewall.Serialnumber;
            ToUpdate.MacAddress = firewall.MacAddress;
            ToUpdate.Status = firewall.Status;

            await _contex.SaveChangesAsync();
            return ToUpdate;

        }
        //Delete Firewall
        public void DeleteFirewall(int firewallID)
        {
            var firewall = _contex.Firewalls.SingleOrDefault(l => l.Id == firewallID);
            if (firewall == null)
            {
                throw new Exception("Server not found.");
            }

            _contex.Firewalls.Remove(firewall);
            _contex.SaveChanges();
        }
    }
}
