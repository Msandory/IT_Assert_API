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
    public class DesktopController : Controller
    {
        private readonly DataContex _contex;
        private readonly DesktopService _desktopService;
        public DesktopController(DataContex contex, DesktopService desktopService)
        {
            _contex = contex;
            _desktopService = desktopService;
        }
        // GET: api/desktop
        [HttpGet("desktop")]
        public async Task<ActionResult<IEnumerable<Desktop>>> GetDesktops()
        {

            return await _contex.Desktops.ToListAsync();
        }

        [HttpGet("{desktopId}/owner")]
        //Get Desktop owner
        public ActionResult<DesktopOwnerDTO> GetDesktopOwners(int desktopId)
        {
            var result = _desktopService.GetDesktopOwner(desktopId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //Add Desktop Controlller
        [HttpPost]
        public IActionResult AddDesktop([FromForm] DesktopDto desktopDto)
        {
            try
            {
                _desktopService.AddDesktop(desktopDto);
                return Ok(new { message = "Desktop Successfully Added" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }

        //Delete Desktop
        
        [HttpDelete("delete/{desktopId}")]
        public IActionResult DeleteDesktop(int desktopId)
        {
            try
            {
                _desktopService.DeleteDesktop(desktopId);
                return Ok(new { message = "Desktop deleted successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }

        //Update Desktop

        [HttpPut("{id}")]
        public async Task<IActionResult> EditDesktop(int id, [FromForm] DesktopDto desktopDto)
        {
            if (!_desktopService.DesktopExists(id))
            {

                return NotFound();

            }
            try
            {
                var files = Request.Form.Files;
                var updatedDesktop = await _desktopService.EditDesktopAsync(id, desktopDto, files);
                return Ok(new { Message = "Desktop updated successfully", updatedDesktop });
            }
            catch (ArgumentException ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
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

    }
}
