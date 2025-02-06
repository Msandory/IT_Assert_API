using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageDeviceController : Controller
    {
        private readonly DataContex _dataContex;
        private readonly StorageDeviceService _storageDeviceService;

        public StorageDeviceController(DataContex dataContex, StorageDeviceService storageDeviceService)
        {
            _dataContex = dataContex;
            _storageDeviceService = storageDeviceService;
        }

        // Get StorageDevice List
        [HttpGet("StorageDevice")]
        public async Task<ActionResult<IEnumerable<StorageDevice>>> GetStorageDevices()
        {
            return await _dataContex.StorageDevices.ToListAsync();
        }

        // Add StorageDevice
        [HttpPost]
        public IActionResult AddStorageDevice([FromBody] StorageDevice storageDevice)
        {
            try
            {
                _storageDeviceService.AddStorageDevice(storageDevice);
                return Ok(new { message = "StorageDevice added successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }

        // Update StorageDevice
        [HttpPut("{id}")]
        public async Task<IActionResult> EditStorageDevice(int id, [FromBody] StorageDevice storageDevice)
        {
            if (!_storageDeviceService.StorageDeviceExists(id))
            {
                return NotFound();
            }
            try
            {
                var updated = await _storageDeviceService.EditStorageDeviceAsync(id, storageDevice);
                return Ok(new { Message = "StorageDevice updated successfully", updated });
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

        // Delete StorageDevice
        [HttpDelete("delete")]
        public IActionResult DeleteStorageDevice(int storageDeviceID)
        {
            try
            {
                _storageDeviceService.DeleteStorageDevice(storageDeviceID);
                return Ok(new { message = "StorageDevice deleted successfully." });
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
