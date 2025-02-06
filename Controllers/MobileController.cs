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
    public class MobileController : Controller
    {
        private readonly DataContex _contex;
        private readonly MobileService _mobileService;

        public MobileController(DataContex contex, MobileService mobileService)
        {
            _contex = contex;
            _mobileService = mobileService;
        }
        [HttpGet("Mobile")]
        public async Task<ActionResult<IEnumerable<Mobilephones>>> GetDesktops()
        {

            return await _contex.Mobilephones.ToListAsync();
        }

        [HttpGet("{MobileID}/owner")]
        public ActionResult<TabletOwnerDTO> GetDesktopOwners(int MobileID)
        {
            var result = _mobileService.GetMobileOwner(MobileID);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        //Add Mobile
        [HttpPost]
        public IActionResult AddMobile([FromForm] MobileDTO mobileDTO)
        {
            try
            {
                _mobileService.AddMobile(mobileDTO);
                return Ok(new { message = "Mobile Added Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { MessageProcessingHandler = ex.Message});
            }
        }

        //Edit Mobile
        [HttpPut("{MobileID}")]
        public async Task<IActionResult>EditTablet(int MobileID, [FromForm] MobileDTO mobileDTO)
        {
            if (!_mobileService.TabletExists(MobileID)) 
            {
                return NotFound();
            }
            try 
            {
                var files = Request.Form.Files;
                var updateMobile = await _mobileService.EditMobileAsync(MobileID, mobileDTO,files);
                return Ok(new { message = "Tablet Successfully Edit" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

        //Delete Tab
        
        [HttpDelete("Delete/{MobileID}")]
        public IActionResult DeleteTablet(int MobileID)
        {
            try 
            {
                _mobileService.DeleteMobile(MobileID);
                return Ok(new { message = "Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
