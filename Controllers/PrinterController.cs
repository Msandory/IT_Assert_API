using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrinterController : Controller
    {

        private readonly DataContex _dataContex;
        private readonly PrinterService _printerService;

        public PrinterController(DataContex dataContex, PrinterService printerService)
        {
            _dataContex = dataContex;
            _printerService = printerService;
        }

        //Get printer's List
        [HttpGet("Printer")]
        public async Task<ActionResult<IEnumerable<Printer>>> GetServers()
        {
            return await _dataContex.Printers.ToListAsync();
        }

        //Add Printer
        [HttpPost]
        public IActionResult AddPrinter([FromBody] Printer printer)
        {

            try
            {
                _printerService.AddPrinter(printer);
                return Ok(new { message = "Printer added successfully." });
            }
            catch (Exception ex)
            {
                // Return a Bad Request with the error message
                Console.Error.WriteLine($"Error saving changes: {ex.Message}");
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }


        }
        //update Printer
        [HttpPut("{id}")]
        public async Task<IActionResult> EditServer(int id, [FromBody] Printer printer)
        {

            if (!_printerService.PrinterExists(id))
            {
                return NotFound();
            }
            try
            {
                var updated = await _printerService.EditPrinterAsync(id, printer);
                return Ok(new { Message = "Printer updated successfully", updated });
            }
            catch (ArgumentException ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
            }
        }

        //Delete Printer
        [HttpDelete("delete")]
        public IActionResult DeleteServer(int PrinterID)
        {
            try
            {
                _printerService.DeletePrinter(PrinterID);
                return Ok(new { message = "Printer deleted successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }
    }
}
