using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Services_Interfaces
{
    public class AdditionDeviceService
    {
        private readonly DataContex _contex;

        public AdditionDeviceService(DataContex contex)
        { _contex = contex; }

        //Add Server
        public void AddServer(PhysicalServer server)
        {
            //check if Serialnumber exist
            if (_contex.PhysicalServers.Any(s => s.Serialnumber == server.Serialnumber))
            {
                throw new Exception("Server with Same Serialnumber already exists");
            }

            var d = new PhysicalServer
            {
                Location = server.Location,
                Name = server.Name,
                Brand = server.Brand,
                Model = server.Model,
                IpAddress = server.IpAddress,
                Serialnumber = server.Serialnumber,
                MacAddress = server.MacAddress,
                ProcessorSpecifications = server.ProcessorSpecifications,
                RamAmount = server.RamAmount,
                Status = server.Status
            };
            _contex.PhysicalServers.Add(d);
            _contex.SaveChanges();

        }
        //check if server is in the list
        public bool ServerExists(int id)
        {
            return _contex.PhysicalServers.Any(e => e.Id == id);
        }

        //check if switch is there
        public bool SwithcExists(int id)
        {
            return _contex.Switches.Any(e => e.Id == id);
        }
        //update Server 
        public async Task<PhysicalServer> EditServerAsync(int id, [FromBody] PhysicalServer Server)
        {
            var ServerToUpdate = await _contex.PhysicalServers.FindAsync(id);
            if (ServerToUpdate == null)
            {
                throw new KeyNotFoundException("Server not found");
            }
            // Update the properties of the Server entity
            ServerToUpdate.Location = Server.Location;
            ServerToUpdate.Name = Server.Model;
            Server.Brand = Server.Brand;
            ServerToUpdate.Model = Server.Model;
            ServerToUpdate.IpAddress = Server.IpAddress;
            ServerToUpdate.Serialnumber = Server.Serialnumber;
            ServerToUpdate.MacAddress = Server.MacAddress;
            ServerToUpdate.ProcessorSpecifications = Server.ProcessorSpecifications;
            ServerToUpdate.RamAmount = Server.RamAmount;

            await _contex.SaveChangesAsync();
            return ServerToUpdate;

        }

        //Delete Server
        public void DeleteServer(int serverId)
        {
            var server = _contex.PhysicalServers.SingleOrDefault(l => l.Id == serverId);
            if (server == null)
            {
                throw new Exception("Server not found.");
            }

            _contex.PhysicalServers.Remove(server);
            _contex.SaveChanges();
        }

        //Delte Switch 
        public void DeleteSwitchAsync(int switchId)
        {
            var switches = _contex.Switches.SingleOrDefault(l => l.Id == switchId);
            if (switches == null)
            {
                throw new Exception("Server not found.");
            }
            _contex.Switches.Remove(switches);
            _contex.SaveChanges();
        }

        //update Switch
        public async Task<Switch> EditSwitchAsync(int id, [FromBody] Switch Server)
        {
            var SwitchToUpdate = await _contex.Switches.FindAsync(id);
            if (SwitchToUpdate == null)
            {
                throw new KeyNotFoundException("Server not found");
            }
            // Update the properties of the Server entity
            SwitchToUpdate.Location = Server.Location;
            SwitchToUpdate.Name = Server.Model;
            SwitchToUpdate.Brand = Server.Brand;
            SwitchToUpdate.Model = Server.Model;
            SwitchToUpdate.IpAddress = Server.IpAddress;
            SwitchToUpdate.Serialnumber = Server.Serialnumber;
            SwitchToUpdate.MacAddress = Server.MacAddress;
            SwitchToUpdate.PoeCapability = Server.PoeCapability;
            SwitchToUpdate.PortsUsed = Server.PortsUsed;
            SwitchToUpdate.FirmwareVersion = Server.FirmwareVersion;

            await _contex.SaveChangesAsync();
            return SwitchToUpdate;

        }

        //Add Switch
        public void AddSwitch(Switch switches)
        {
            //check if Serialnumber exist
            if (_contex.Switches.Any(s => s.Serialnumber == switches.Serialnumber))
            {
                throw new Exception("Switch with Same Serialnumber already exists");
            }

            var s = new Switch
            {
                Location = switches.Location,
                Name = switches.Name,
                Site= switches.Site,
                Brand = switches.Brand,
                Model = switches.Model,
                Status = switches.Status,
                IpAddress = switches.IpAddress,
                Serialnumber = switches.Serialnumber,
                MacAddress = switches.MacAddress,
                PoeCapability = switches.PoeCapability,
                PortsUsed = switches.PortsUsed,
                FirmwareVersion = switches.FirmwareVersion
            };
            _contex.Switches.Add(s);
            _contex.SaveChanges();

        }
    }
}
