using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FirewallController : Controller
    {

        private readonly DataContex _dataContex;
        private readonly FirewallService _firewallService;

        public FirewallController(DataContex dataContex, FirewallService firewallservice)
        {
            _dataContex = dataContex;
            _firewallService = firewallservice;
        }

        //Get Firewall's List
        [HttpGet("Firewall")]
        public async Task<ActionResult<IEnumerable<Firewall>>> GetFirewall()
        {
            return await _dataContex.Firewalls.ToListAsync();
        }

        //Add Firewall
        [HttpPost]
        public IActionResult AddFirewall([FromBody] Firewall firewall)
        {

            try
            {
                _firewallService.AddFirewall(firewall);
                return Ok(new { message = "Firewall device added successfully." });
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
        //update Firewall
        [HttpPut("{id}")]
        public async Task<IActionResult> EditFirewall(int id, [FromBody] Firewall firewall)
        {

            if (!_firewallService.FirewallExists(id))
            {
                return NotFound();
            }
            try
            {
                var updated = await _firewallService.EditFirewallAsync(id, firewall);
                return Ok(new { Message = "Firewall updated successfully", updated });
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
        public IActionResult DeleteFirewall(int FirewallID)
        {
            try
            {
                _firewallService.DeleteFirewall(FirewallID);
                return Ok(new { message = "Firewall Device deleted successfully." });
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
