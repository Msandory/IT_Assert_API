using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Services_Interfaces
{
    public class TonerService
    {
        private readonly DataContex _dataContex;
        public TonerService(DataContex dataContex)
        {
            _dataContex = dataContex;
        }

        //Add Toner
        public void AddToner(Toner toner)
        {
            //Check if Toner With the same name exist
            if (_dataContex.Toners.Any(t=>t.Name==toner.Name)) 
            {
                throw new Exception("Toner name already exists");
            }

            var d = new Toner
            {
                Name = toner.Name,
                Printer = toner.Printer,
                Count = toner.Count,
                lastIssued = toner.lastIssued,
               

            };
            _dataContex.Toners.Add(d);
            _dataContex.SaveChanges();

        }
        //Update Toner
        public async Task<Toner> EditTonerAsync(int id, [FromBody] Toner toner)
        {
            var ToUpdate = await _dataContex.Toners.FindAsync(id);

            // Update the properties of the Server entity
            ToUpdate.Name = toner.Name;
            ToUpdate.Printer = toner.Printer;
            ToUpdate.Count = toner.Count;
            ToUpdate.lastIssued = toner.lastIssued;
           
            await _dataContex.SaveChangesAsync();
            return ToUpdate;

        }

        //Delete Printer
        public void DeleteToner(int TonerID)
        {
            var toner = _dataContex.Toners.SingleOrDefault(l => l.Id == TonerID);
            if (toner == null)
            {
                throw new Exception("toner not found.");
            }

            _dataContex.Toners.Remove(toner);
            _dataContex.SaveChanges();
        }
        //check if server is in the list
        public bool TonerExist(int id)
        {
            return _dataContex.Toners.Any(e => e.Id == id);
        }
    }
}
