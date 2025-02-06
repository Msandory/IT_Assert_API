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
    public class OwnerController : Controller
    {
        private readonly DataContex _contex;
        private readonly OwnerService _ownerService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public OwnerController(DataContex datacontex, OwnerService ownerService, IConfiguration configuration, IWebHostEnvironment environment )
        {
            _contex = datacontex;
            _ownerService = ownerService;
            _configuration = configuration;
            _environment = environment;
        }


        //Get list of Owners
        [HttpGet("Owner")]
        public async Task<ActionResult<IEnumerable<Owner>>> GetOwner()
        {
            var owners = await _contex.Owners.Where(o => !o.IsDeleted).ToListAsync();
            return Ok(owners);
        }

        //Add Owner
        [HttpPost]
        public IActionResult AddOwner([FromForm] OwnerDTO owner)
        {
            try
            {
                _ownerService.AddOwner(owner);
                return Ok(new { message = "Owner Added Successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        //Edit Owner
        [HttpPut("{id}")]
        public async Task<IActionResult> EditOwner(int id, [FromBody] OwnerDTO owner)
        {
            if (!_ownerService.OnwerExist(id)) {
                return NotFound();
            }
            try
            {
                var updateOwner = await _ownerService.EditOwner(id, owner);
                return Ok(new { Message = "Onwer Successfully Edited" });
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
        //Delete Owner
        
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteOwner(int id) {
            try
            {
                bool isDeleted = await _ownerService.DeleteOwner(id);
                if (!isDeleted)
                {
                    return NotFound();
                }

                return Ok(new { message = "Owner deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Get Devices for owner
        [HttpGet("{ownerId}/devices")]
        public IActionResult GetOwnerDevices(int ownerId)
        {
            var ownerDevices = _ownerService.GetOwnerDevices(ownerId);

            if (ownerDevices != null)
            {
                return Ok(ownerDevices);
            }
            else
            {
                return NotFound("Owner not found.");
            }
        }
        //searching AD users
        [HttpGet("SearchAdUsers")]
        public ActionResult<IEnumerable<AdUserDto>> SearchAdUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty.");
            }

            try
            {
                var users = _ownerService.SearchUsers(query);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("users/{id}/filepath")]
        public async Task<IActionResult> DeleteFilePath(int id, [FromBody] string filePath)
        {
            var user = await _contex.Owners.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Filepath == null || !user.Filepath.Contains(filePath))
            {
                return NotFound("File path not found.");
            }

            // Remove the file path from the list
            user.Filepath = user.Filepath.Where(f => f != filePath).ToList();

            _contex.Owners.Update(user);
            await _contex.SaveChangesAsync();

            return Ok("File path removed successfully.");
        }

        [HttpPost("users/{userId}/documents")]
        
        public async Task<IActionResult> UploadDocuments(int userId, [FromForm] List<IFormFile> files)
        {
            // Configuration and existence check

            var virtualDirectoryUrl = _configuration["FileStorage:VirtualDirectoryUrl"];
            var physicalUploadPath = _configuration[$"FileStorage:PhysicalPath:{_environment.EnvironmentName}"];

            if (files == null || !files.Any())
            {
                return BadRequest("No files were uploaded.");
            }
            if (string.IsNullOrEmpty(virtualDirectoryUrl) || string.IsNullOrEmpty(physicalUploadPath))
            {
                return StatusCode(500, "File storage configuration is missing");
            }

            if (!await _ownerService.UserExistsAsync(userId)) // Check UserExists!
            {
                return NotFound($"User with ID {userId} not found.");
            }

            try
            {

                if (!Directory.Exists(physicalUploadPath))
                {
                    Directory.CreateDirectory(physicalUploadPath);
                }

                // We'll only process one file since we're storing a single filepath
                var file = files.First();
                string virtualFilePath = "";

                if (file.Length > 0)
                {
                    string fileName = file.FileName;
                    string baseFileName = Path.GetFileNameWithoutExtension(fileName);
                    string extension = Path.GetExtension(fileName);
                    int counter = 1;

                    string physicalFilePath = Path.Combine(physicalUploadPath, fileName);

                    while (System.IO.File.Exists(physicalFilePath))
                    {
                        fileName = $"{baseFileName}({counter}){extension}";
                        physicalFilePath = Path.Combine(physicalUploadPath, fileName);
                        counter++;
                    }

                    virtualFilePath = $"{virtualDirectoryUrl}/{fileName}";

                    try
                    {
                        using (var stream = new FileStream(physicalFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error saving file: {fileName}");
                    }
                }

                // Update the user's file path
                try
                {
                    await _ownerService.UpdateUserFilePathsAsync(userId, virtualFilePath);

                    return Ok(new
                    {
                        message = "File uploaded and user updated successfully",
                        filePath = virtualFilePath
                    });
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "Error updating user file path for user: {UserId}", userId);
                    // If this fails, we should probably delete the uploaded file.  For now, just log the error.
                    return StatusCode(500, new
                    {
                        error = "File uploaded but failed to update user file path",
                        filePath = virtualFilePath
                    });
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error in UploadDocuments");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

    }


}
