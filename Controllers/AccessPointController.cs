using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessPointController : Controller
    {
        private readonly DataContex _dataContex;
        private readonly AccessPointService _accessPointService;

        public AccessPointController(DataContex dataContex, AccessPointService accessPointService)
        {
            _dataContex = dataContex;
            _accessPointService = accessPointService;
        }

        // Get AccessPoint List
        [HttpGet("AccessPoint")]
        public async Task<ActionResult<IEnumerable<AccessPoint>>> GetAccessPoints()
        {
            return await _dataContex.AccessPoints.ToListAsync();
        }

        // Add AccessPoint
        [HttpPost]
        public IActionResult AddAccessPoint([FromBody] AccessPoint accessPoint)
        {
            try
            {
                _accessPointService.AddAccessPoint(accessPoint);
                return Ok(new { message = "AccessPoint added successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }

        // Update AccessPoint
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAccessPoint(int id, [FromBody] AccessPoint accessPoint)
        {
            if (!_accessPointService.AccessPointExists(id))
            {
                return NotFound();
            }
            try
            {
                var updated = await _accessPointService.EditAccessPointAsync(id, accessPoint);
                return Ok(new { Message = "AccessPoint updated successfully", updated });
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

        // Delete AccessPoint
        [HttpDelete("delete")]
        public IActionResult DeleteAccessPoint(int accessPointID)
        {
            try
            {
                _accessPointService.DeleteAccessPoint(accessPointID);
                return Ok(new { message = "AccessPoint deleted successfully." });
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
