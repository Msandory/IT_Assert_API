using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WifiControllerController : Controller
    {
        private readonly DataContex _dataContex;
        private readonly WifiControllerService _wifiControllerService;

        public WifiControllerController(DataContex dataContex, WifiControllerService wifiControllerService)
        {
            _dataContex = dataContex;
            _wifiControllerService = wifiControllerService;
        }

        // Get WifiController List
        [HttpGet("WifiController")]
        public async Task<ActionResult<IEnumerable<WifiController>>> GetWifiControllers()
        {
            return await _dataContex.WifiControllers.ToListAsync();
        }

        // Add WifiController
        [HttpPost]
        public IActionResult AddWifiController([FromBody] WifiController wifiController)
        {
            try
            {
                _wifiControllerService.AddWifiController(wifiController);
                return Ok(new { message = "WifiController added successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }

        // Update WifiController
        [HttpPut("{id}")]
        public async Task<IActionResult> EditWifiController(int id, [FromBody] WifiController wifiController)
        {
            if (!_wifiControllerService.WifiControllerExists(id))
            {
                return NotFound();
            }
            try
            {
                var updated = await _wifiControllerService.EditWifiControllerAsync(id, wifiController);
                return Ok(new { Message = "WifiController updated successfully", updated });
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

        // Delete WifiController
        [HttpDelete("delete")]
        public IActionResult DeleteWifiController(int wifiControllerID)
        {
            try
            {
                _wifiControllerService.DeleteWifiController(wifiControllerID);
                return Ok(new { message = "WifiController deleted successfully." });
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
