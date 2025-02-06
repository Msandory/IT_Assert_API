using Inventory_System_API.Services_Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_System_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalSearchController : Controller
    {
        private readonly SearchService _searchService;
        private readonly OwnerService _ownerService;

        public GlobalSearchController(SearchService searchService, OwnerService ownerService)
        {
            _searchService = searchService;
            _ownerService = ownerService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty.");
            }

            try
            {
                // Call the dynamic search method without pagination
                var result = await _searchService.DynamicSearchAsync(query);

                // Check if any results were found
                if (result == null || !result.Any())
                {
                    return NotFound("Nothing found in the database.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search-owners")]
        public async Task<IActionResult> SearchOwners([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty.");
            }

            try
            {
                var owners = await _ownerService.SearchOwnersAsync(query);

                if (!owners.Any())
                {
                    return NotFound("No owners found in the database.");
                }

                return Ok(owners);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}