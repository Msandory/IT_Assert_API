using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
  
  
        [ApiController]
        [Route("api/[controller]")]
        public class TurnstileController : Controller
        {
            private readonly DataContex _dataContex;
            private readonly TurnstileService _turnstileService;

            public TurnstileController(DataContex dataContex, TurnstileService turnstileService)
            {
                _dataContex = dataContex;
                _turnstileService = turnstileService;
            }

            // Get Turnstile List
            [HttpGet("Turnstile")]
            public async Task<ActionResult<IEnumerable<Turnstile>>> GetTurnstiles()
            {
                return await _dataContex.Turnstiles.ToListAsync();
            }

            // Add Turnstile
            [HttpPost]
            public IActionResult AddTurnstile([FromBody] Turnstile turnstile)
            {
                try
                {
                    _turnstileService.AddTurnstile(turnstile);
                    return Ok(new { message = "Turnstile added successfully." });
                }
                catch (Exception ex)
                {
                    return new ObjectResult(new { message = ex.Message })
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                    };
                }
            }

            // Update Turnstile
            [HttpPut("{id}")]
            public async Task<IActionResult> EditTurnstile(int id, [FromBody] Turnstile turnstile)
            {
                if (!_turnstileService.TurnstileExists(id))
                {
                    return NotFound();
                }
                try
                {
                    var updated = await _turnstileService.EditTurnstileAsync(id, turnstile);
                    return Ok(new { Message = "Turnstile updated successfully", updated });
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

            // Delete Turnstile
            [HttpDelete("delete")]
            public IActionResult DeleteTurnstile(int turnstileID)
            {
                try
                {
                    _turnstileService.DeleteTurnstile(turnstileID);
                    return Ok(new { message = "Turnstile deleted successfully." });
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
