using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController : Controller
    {
        private readonly DataContex _dataContex;
        private readonly AdditionDeviceService _additionDeviceService;
        
        public ServerController (DataContex dataContex, AdditionDeviceService additionDeviceService)
        {
            _dataContex = dataContex;
            _additionDeviceService = additionDeviceService;
        }

        //Get Server's List
        [HttpGet("Server")]
        public async Task<ActionResult<IEnumerable<PhysicalServer>>> GetServers()
        {
            return await _dataContex.PhysicalServers.ToListAsync();
        }

        //Add Server
        [HttpPost]
        public IActionResult AddServer([FromBody] PhysicalServer server)
        {
           
            try
            {
                _additionDeviceService.AddServer(server);
                return Ok(new { message = "Server added successfully." });
            }
            catch (Exception ex)
            {
                // Return a Bad Request with the error message
                Console.Error.WriteLine($"Error saving changes: {ex.Message}");
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }

            
        }
        //update Server
        [HttpPut("{id}")]
        public async Task<IActionResult> EditServer(int id, [FromBody] PhysicalServer server)
        {

            if (!_additionDeviceService.ServerExists(id))
            {
                return NotFound();
            }
            try
            {
                var updatedServer = await _additionDeviceService.EditServerAsync(id, server);
                return Ok(new { Message = "Server updated successfully", updatedServer });
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
        //Delete Server
        [HttpDelete("delete")]
        public IActionResult DeleteServer(int ServerID)
        {
            try
            {
                _additionDeviceService.DeleteServer(ServerID);
                return Ok(new { message = "Server deleted successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }
        //------------------------------------------------------------------------------------------------------

        //delete switch
        [HttpDelete("switch/{id}")]
        public async Task<IActionResult> DeleteSwitch(int id)
        {
            try
            {
                _additionDeviceService.DeleteSwitchAsync(id);
                //_logger.LogInformation("Switch {Id} deleted successfully", id);
                return Ok(new { message = "Switch deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                //_logger.LogWarning("Switch {Id} not found for deletion", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
              //  _logger.LogError(ex, "Error deleting switch {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while deleting the switch." });
            }
        }
        [HttpPut("Switch/{id}")]
        public async Task<IActionResult> EditSwithc(int id, [FromBody] Switch @switch)
        {

            if (!_additionDeviceService.SwithcExists(id))
            {
                return NotFound();
            }
            try
            {
                var updatedSwitch = await _additionDeviceService.EditSwitchAsync(id, @switch);
                return Ok(new { Message = "Server updated successfully", updatedSwitch });
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
        //Get Switches
        [HttpGet("switch")]
        public async Task<ActionResult<IEnumerable<Switch>>> GetSwitch()
        {
            return await _dataContex.Switches.ToListAsync();
        }
        //add swithc
        [HttpPost("addswitch")]
        public IActionResult AddSwithc([FromBody] Switch @switch)
        {

            try
            {
                _additionDeviceService.AddSwitch(@switch);
                return Ok(new { message = "Switch added successfully." });
            }
            catch (Exception ex)
            {
                // Return a Bad Request with the error message
                Console.Error.WriteLine($"Error saving changes: {ex.Message}");
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }


        }

    }
}
