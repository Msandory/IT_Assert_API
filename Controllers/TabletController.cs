using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TabletController : Controller
    {
        private readonly DataContex _contex;
        private readonly TabletService _tabletService;
        private readonly ILogger<TabletController> _logger;
        public TabletController(DataContex contex, TabletService tabletService, ILogger<TabletController> logger)
        {
            _contex = contex;
            _tabletService = tabletService;
            _logger = logger;
        }
        [HttpGet("Tablet")]
        public async Task<ActionResult<IEnumerable<Tablets>>> GetDesktops()
        {

            return await _contex.Tablets.ToListAsync();
        }
        //Get owner
        [HttpGet("{tabID}/owner")]
        public ActionResult<TabletOwnerDTO> GetDesktopOwners(int tabID)
        {
            var result = _tabletService.GetTabletOwner(tabID);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //Add Tablet
        [HttpPost]
        public IActionResult AddTablet([FromForm] TabletDTO tabletDto)
        {
            try
            {
                _tabletService.AddTablet(tabletDto);
                return Ok(new { message = "Tablet Added Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Edit tablet
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTablet(int id, [FromForm] TabletDTO tabletDto)
        {
            if (!_tabletService.TabletExists(id))
            {
                return NotFound();
            }
            try
            {
                var files = Request.Form.Files;
                var updateTablet = await _tabletService.EditTabletsAsync(id, tabletDto,files);
                return Ok(new { Message = "Tablet Successfully Edit" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
                _logger.LogError(ex.ToString());
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Details = ex.Message });
                _logger.LogError(ex, "error when updating");
            }
        }

        //Delete Tab

        [HttpDelete("Delete/{TabId}")]
        public IActionResult DeleteTablet(int TabId)
        {

            try
            {
                _tabletService.DeleteTablet(TabId);
                return Ok(new { message = "Deleted Successfully" });


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
