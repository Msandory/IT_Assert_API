using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Inventory_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly LicenseService _licenseService;
        private readonly DataContex _dataContex;
        private readonly ILogger<AuthController> _logger;

        public LicenseController(LicenseService licenseService, DataContex dataContex ,ILogger<AuthController> logger)
        {
            _licenseService = licenseService;
            _dataContex = dataContex;
            _logger = logger;
        }

        [HttpGet("license")]
        public async Task<ActionResult<IEnumerable<Licence>>> GetLicenses()
        {
            var licenses = await _dataContex.Licences.ToListAsync();
            return Ok(licenses);
        }

        // Add License
        [HttpPost]
        public IActionResult AddLicense([FromBody] LicenseDTO licenseDTO)
        {
            var Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                _licenseService.AddLicense(licenseDTO);
                _logger.LogInformation($"License information added at {Time}");
                return Ok(new { message = "License information successfully added." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when adding License details at {Time}: {ex.Message}");
                return new ObjectResult(new { message = ex.Message }) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }

        // Delete License
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteLicense(int id)
        {
            try
            {
                _licenseService.DeleteLicense(id);
                return Ok(new { message = "License information deleted successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }

        // Edit License
        [HttpPut("{id}")]
        public async Task<IActionResult> EditLicenseAsync(int id, [FromBody] LicenseDTO licenseDTO)
        {
            var Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (!_licenseService.LicenseExists(id))
            {
                return NotFound();
            }

            try
            {
                var updatedLicense = await _licenseService.EditLicenseAsync(id, licenseDTO);
                _logger.LogInformation($"License information updated at {Time}");
                return Ok(new { message = "License updated successfully.", updatedLicense });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Error when adding License details at {Time}: {ex.Message}");
                return new ObjectResult(new { message = ex.Message }) { StatusCode = StatusCodes.Status400BadRequest };

            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when Editing License details at {Time}: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error.", details = ex.Message });
                
            }
        }
    }
}
