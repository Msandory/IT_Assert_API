using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Services_Interfaces
{
    public class PrinterService
    {
        private readonly DataContex _contex;
        public PrinterService(DataContex contex) 
        {
            _contex = contex; 
        }

        //Add Printer
        public void AddPrinter(Printer printer)
        {
            //check if Serialnumber exist
            if (_contex.Printers.Any(s => s.Serialnumber == printer.Serialnumber))
            {
                throw new Exception("Printer with Same Serialnumber already exists");
            }

            var d = new Printer
            {
                Location = printer.Location,
                Brand = printer.Brand,
                Model = printer.Model,
                IpAddress = printer.IpAddress,
                Serialnumber = printer.Serialnumber,
                MacAddress = printer.MacAddress,
                Credentials = printer.Credentials,
                Status = printer.Status,
                Toner= printer.Toner,

            };
            _contex.Printers.Add(d);
            _contex.SaveChanges();

        }

        //check if server is in the list
        public bool PrinterExists(int id)
        {
            return _contex.Printers.Any(e => e.Id == id);
        }

        //Update Printer
        public async Task<Printer> EditPrinterAsync(int id, [FromBody] Printer printer)
        {
            var PrinterToUpdate = await _contex.Printers.FindAsync(id);
            if (PrinterToUpdate == null)
            {
                throw new KeyNotFoundException("Printer not found");
            }
            // Update the properties of the Server entity
            PrinterToUpdate.Location = printer.Location;
            PrinterToUpdate.Brand = printer.Brand;
            PrinterToUpdate.Model = printer.Model;
            PrinterToUpdate.IpAddress = printer.IpAddress;
            PrinterToUpdate.Serialnumber = printer.Serialnumber;
            PrinterToUpdate.MacAddress = printer.MacAddress;
            PrinterToUpdate.Credentials = printer.Credentials;
            PrinterToUpdate.Status= printer.Status;
           PrinterToUpdate.Toner= printer.Toner;
            await _contex.SaveChangesAsync();
            return PrinterToUpdate;

        }
        //Delete Printer
        public void DeletePrinter(int printerId)
        {
            var printer = _contex.Printers.SingleOrDefault(l => l.Id == printerId);
            if (printer == null)
            {
                throw new Exception("Printer not found.");
            }

            _contex.Printers.Remove(printer);
            _contex.SaveChanges();
        }


    }
}
