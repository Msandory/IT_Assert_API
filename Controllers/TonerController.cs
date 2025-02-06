using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TonerController : Controller
    {
        private readonly DataContex _dataContex;
        private readonly TonerService _tonerService;
        public TonerController(DataContex dataContex, TonerService tonerService)
        {
            _dataContex = dataContex;
            _tonerService = tonerService;
        }

        //Get Toner List
        [HttpGet("toner")]
        public async Task<ActionResult<IEnumerable<Toner>>> GetToners()
        {
            return await _dataContex.Toners.ToListAsync();
        }

        //Add Toner
        [HttpPost]
        public IActionResult AddToner([FromBody] Toner toner)
        {
            try
            {
                _tonerService.AddToner(toner);
                return Ok(new { message = "Toner added successfully." });
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.Error.WriteLine($"Error adding toner:");
                Console.Error.WriteLine($"Message: {ex.Message}");
                Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                return BadRequest(new { message = ex.Message });
            }
        }
        //update Toner
        [HttpPut("{id}")]
        public async Task<IActionResult> EditToner(int id, [FromBody] Toner toner)
        {

            if (!_tonerService.TonerExist(id))
            {
                return NotFound();
            }
            try
            {
                var updatedToner = await _tonerService.EditTonerAsync(id, toner);
                return Ok(new { Message = "Toner updated successfully", updatedToner });
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
        //Delete Toner
        [HttpDelete("delete")]
        public IActionResult DeleteServer(int tonerID)
        {
            try
            {
                _tonerService.DeleteToner(tonerID);
                return Ok(new { message = "Toner deleted successfully." });
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
