using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BiometricDeviceController : Controller
    {
        private readonly DataContex _dataContex;
        private readonly BiometricDeviceService _biometricDeviceService;

        public BiometricDeviceController(DataContex dataContex, BiometricDeviceService biometricDeviceService)
        {
            _dataContex = dataContex;
            _biometricDeviceService = biometricDeviceService;
        }

        // Get BiometricDevice List
        [HttpGet("BiometricDevice")]
        public async Task<ActionResult<IEnumerable<BiometricDevice>>> GetBiometricDevices()
        {
            return await _dataContex.BiometricDevices.ToListAsync();
        }

        // Add BiometricDevice
        [HttpPost]
        public IActionResult AddBiometricDevice([FromBody] BiometricDevice biometricDevice)
        {
            try
            {
                _biometricDeviceService.AddBiometricDevice(biometricDevice);
                return Ok(new { message = "BiometricDevice added successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }

        // Update BiometricDevice
        [HttpPut("{id}")]
        public async Task<IActionResult> EditBiometricDevice(int id, [FromBody] BiometricDevice biometricDevice)
        {
            if (!_biometricDeviceService.BiometricDeviceExists(id))
            {
                return NotFound();
            }
            try
            {
                var updated = await _biometricDeviceService.EditBiometricDeviceAsync(id, biometricDevice);
                return Ok(new { Message = "BiometricDevice updated successfully", updated });
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

        // Delete BiometricDevice
        [HttpDelete("delete")]
        public IActionResult DeleteBiometricDevice(int biometricDeviceID)
        {
            try
            {
                _biometricDeviceService.DeleteBiometricDevice(biometricDeviceID);
                return Ok(new { message = "BiometricDevice deleted successfully." });
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
