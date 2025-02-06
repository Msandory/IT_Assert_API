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
    public class LaptopController : Controller
    {
        private readonly DataContex _contex;
        private readonly LaptopService _laptopService;
        public LaptopController (DataContex dataContex, LaptopService laptopService )
        {
            _contex=dataContex;
            _laptopService = laptopService;
        }
        // GET: api/laptop
        [HttpGet("laptop")]
        public async Task<ActionResult<IEnumerable<Laptop>>> GetLaptops()
        {

            return await _contex.Laptops.ToListAsync();
        }
        // GET: api/laptop/{make}
        [HttpGet("{make}")]
        public async Task<ActionResult<Laptop>> GetLaptop(string make)
        {

            var laptop = await _contex.Laptops.FindAsync(make);

            if (laptop == null)
            {
                return NotFound();
            }

            return laptop;
        }

        //Add laptop
        [HttpPost]
        public IActionResult AddLaptop([FromForm] LaptopDto laptopDto)
        {
            var username = User.Identity?.Name;
            var logonTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                _laptopService.AddLaptop(laptopDto);
                return Ok(new { message = "Laptop added successfully." });
            }
            catch (Exception ex)
            {
                // Return a Bad Request with the error message
                Console.Error.WriteLine($"Error saving changes: { ex.Message}");
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                }; 
            }

            ///     return new ObjectResult(new { message = $"Hello {UserExists.fullnames}" })
            
               // StatusCode = StatusCodes.Status200OK
                        

        }
        //delete laptop using ID
        
        [HttpDelete("delete/{laptopId}")]
        public IActionResult DeleteLaptop(int laptopId)
        {
            try
            {
                _laptopService.DeleteLaptop(laptopId);
                return Ok(new { message = "Laptop deleted successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message })
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }

        //API to get get owner information on that is related to the laptop using the laptop ID
        [HttpGet("{laptopId}/owner")]
        public ActionResult<LaptopOwnerDTO> GetLaptopOwner(int laptopId)
        {
            var result = _laptopService.GetLaptopOwner(laptopId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Laptop>> EditLaptop(int id, [FromForm] LaptopDto laptopDto)
        {
            try
            {
                var files = Request.Form.Files;
                var updatedLaptop = await _laptopService.EditLaptopAsync(id, laptopDto, files);
                return Ok(updatedLaptop);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }


    

}
