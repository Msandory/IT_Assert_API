using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly ReportService _reportService;
        private readonly ILogger<AuthController> _logger;

        public ReportController(ReportService reportService, ILogger<AuthController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("age")]
        public async Task<IActionResult> GetDevicesByAgeCriteria(string deviceType, string ageRangeCriteria)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(deviceType) || string.IsNullOrWhiteSpace(ageRangeCriteria))
                {
                    return new ObjectResult(new { message = "Device type and age range criteria are required." })
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                    };
                }

                // Get the devices that match the criteria
                var devices = await _reportService.GetDevicesByAgeCriteriaAsync(deviceType, ageRangeCriteria);


                // If no devices match the criteria
                if (devices == null || !devices.Any())
                {
                    return new ObjectResult(new { message = "No devices match the given criteria." })
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                    };
                }

                // Return the devices that match the criteria
                return new ObjectResult(devices)
                {
                    StatusCode = StatusCodes.Status200OK,
                };
            }
            catch (Exception ex)
            {
                // Handle any errors that occur
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            }
        }

        [HttpGet("Status")]
        public async Task<IActionResult> GetDeviceStatus(string deviceType, string StatusCriteria)
        {
            if (string.IsNullOrWhiteSpace(deviceType) || string.IsNullOrWhiteSpace(StatusCriteria))
            {
                return BadRequest(new { message = "Device type and Status criteria are required." });
            }

            try
            {
                var devices = await _reportService.GetDevicesByStatusCriteriaAsync(deviceType, StatusCriteria);

                if (devices == null || !devices.Any())
                {
                    return NotFound(new { message = "No devices match the given criteria." });
                }

                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDeviceStatus with deviceType: {DeviceType}, StatusCriteria: {StatusCriteria}", deviceType, StatusCriteria);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing the request." }); // Generic message
            }
        }

        [HttpGet("OS")]

        public async Task<IActionResult> GetDeviceByOS(string deviceType, string OSCriteria)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(deviceType) || string.IsNullOrWhiteSpace(OSCriteria))
                {
                    return new ObjectResult(new { message = "Device type and Status criteria are required." })
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                    };
                }
                // Get the devices that match the criteria
                var devices = await _reportService.GetDeviceDetailsByOsAsync(deviceType, OSCriteria);

                // If no devices match the criteria
                if (devices == null || !devices.Any())
                {
                    return new ObjectResult(new { message = "No devices match the given criteria." })
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                    };
                }

                // Return the devices that match the criteria
                return new ObjectResult(devices)
                {
                    StatusCode = StatusCodes.Status200OK,
                };

            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            }

        }

        [HttpGet("Warrenty")]
        public async Task <IActionResult>GetDeviceByWarrenty(string deviceType, string WarrentyStatus)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(deviceType) || string.IsNullOrWhiteSpace(WarrentyStatus))
                {
                    return new ObjectResult(new { message = "Device type and Status criteria are required." })
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                    };
                }
                // Get the devices that match the criteria
                var devices = await _reportService.GetDeviceDetailsByWarrentiesAsync(deviceType, WarrentyStatus);

                // If no devices match the criteria
                if (devices == null || !devices.Any())
                {
                    return new ObjectResult(new { message = "No devices match the given criteria." })
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                    };
                }

                // Return the devices that match the criteria
                return new ObjectResult(devices)
                {
                    StatusCode = StatusCodes.Status200OK,
                };

            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                };
            }

        }

    }

}
