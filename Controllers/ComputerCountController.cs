using Inventory_System_API.Models;
using Microsoft.AspNetCore.Mvc;
using static Inventory_System_API.Models.ComputerCount;

[ApiController]
[Route("api/[controller]")]
public class ComputerCountController : ControllerBase
{
    private readonly ComputerCountService _computerCountService;

    public ComputerCountController(ComputerCountService computerCountService)
    {
        _computerCountService = computerCountService;
    }

    [HttpGet]
    public ActionResult<ComputerCountDto> GetComputerCounts()
    {
        var (adComputerCount, dbComputerCount) = _computerCountService.GetComputerCounts();

        var result = new ComputerCountDto
        {
            AdComputerCount = adComputerCount,
            DbComputerCount = dbComputerCount,
            AreCountsEqual = adComputerCount == dbComputerCount
        };

        return Ok(result);
    }

    [HttpGet("history")]
    public ActionResult<IEnumerable<ComputerCount>> GetComputerCountHistory()
    {
        var history = _computerCountService.GetComputerCountHistory();
        return Ok(history);
    }
    [HttpGet("TabletCount")]
    public ActionResult<TabletCountDto> GetTabletCounts()
    {
        var dbTabletCount = _computerCountService.GetNumberOfTabletsInDatabase();

        var result = new TabletCountDto
        {

            DbTabletCount = dbTabletCount

        };

        return Ok(result);

    }
    [HttpGet("compare-latest")]
    public ActionResult<InventoryComparisonResult> CompareLatestEntries()
    {
        try
        {
            // Get the most recent entry first
            var latestEntry = _computerCountService.GetComputerCountHistory()
                .OrderByDescending(c => c.Date)
                .FirstOrDefault();

            if (latestEntry == null)
            {
                return NotFound("No computer count entries found");
            }

            var comparisonResult = _computerCountService.CompareWithPreviousEntry(latestEntry);

            return Ok(comparisonResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while comparing entries: {ex.Message}");
        }
    }
    [HttpGet("TabletInUse")]
    public ActionResult<TabletCountDto> GetTabletInUse()
    {
        var dbTabletCount = _computerCountService.GetNumberOfTabletsInUse();

        var result = new TabletCountDto
        {

            DbTabletCount = dbTabletCount

        };

        return Ok(result);

    }
    [HttpGet("MobileCount")]
    public ActionResult<MobileCountDto> GetMobileCounts()
    {
        var dbMobileCount = _computerCountService.GetNumberOfMobileInDatabase();

        var result = new MobileCountDto
        {

            DbMobileCount = dbMobileCount

        };

        return Ok(result);

    }
    [HttpGet("MobileCountInUse")]
    public ActionResult<MobileCountDto> GetMobileCountsInUse()
    {
        var dbMobileCount = _computerCountService.GetNumberOfMobileInUse();

        var result = new MobileCountDto
        {

            DbMobileCount = dbMobileCount

        };

        return Ok(result);

    }
    [HttpGet("LaptopsInStock")]
    public ActionResult<MobileCountDto> GetLaptopsInStock()
    {
        var dbMobileCount = _computerCountService.GetNumberOfLaptopsInStock();

        var result = new MobileCountDto
        {

            DbMobileCount = dbMobileCount

        };

        return Ok(result);

    }
    [HttpGet("DesktopsInStock")]
    public ActionResult<MobileCountDto> GetDesktopsInStock()
    {
        var dbMobileCount = _computerCountService.GetNUmberOfDesktopInStock();

        var result = new MobileCountDto
        {

            DbMobileCount = dbMobileCount

        };

        return Ok(result);

    }
    [HttpGet("DesktopsInUse")]
    public ActionResult<MobileCountDto> GetDesktopsInUse()
    {
        var dbMobileCount = _computerCountService.GetNUmberOfDesktopInUse();

        var result = new MobileCountDto
        {

            DbMobileCount = dbMobileCount

        };

        return Ok(result);

    }
    [HttpGet("device-count")]
    public ActionResult<IEnumerable<DeviceCountDTO>> GetDeviceCount()
    {
        var deviceCounts = _computerCountService.GetDeviceCounts();
        return Ok(deviceCounts);
    }
    [HttpGet("cpu-summary")]
    public async Task<IActionResult> GetCpuTypeSummary()
    {
        var summary = await _computerCountService.GetCpuTypeSummaryAsync();
        return Ok(summary);
    }
    [HttpGet("cpu-summary-Desktop")]
    public async Task<IActionResult> GetCpuTypeSummaryDesktop()
    {
        var summary = await _computerCountService.GetCpuTypeSummaryAsyncDesktop();
        return Ok(summary);
    }
    [HttpGet("devices-by-make")]
    public async Task<IActionResult> GetDevicesByMake()
    {
        var result = await _computerCountService.GetDevicesByMakeAsync();
        return Ok(result);
    }
    [HttpGet("age-distribution")]
    public async Task<IActionResult> GetAgeDistribution()
    {
        try
        {
            var ageDistribution = await _computerCountService.GetAgeDistributionAsync();
            return Ok(ageDistribution);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpGet("status/{deviceType}")]
    public async Task<ActionResult<DeviceStatusResponse>> GetDeviceStatus(string deviceType)
    {
        try
        {
            var statusCounts = await _computerCountService.GetDeviceStatusCountsAsync(deviceType);

            var response = new DeviceStatusResponse
            {
                DeviceType = deviceType,
                StatusCounts = statusCounts
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
        
            return BadRequest(new { error = "Invalid device type" });
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }
}