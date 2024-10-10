using EnergyStealLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MintChainDropsApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyStealController : ControllerBase
    {
        private readonly EnergyStealService _energyStealService;

        public EnergyStealController(EnergyStealService energyStealService)
        {
            _energyStealService = energyStealService;
        }

        [HttpPost]
        public async Task<IActionResult> GetStealableEnergy([FromBody] EnergyRequest request)
        {
            try
            {
                // Call the service method to fetch stealable energy and pass the Bearer token
                var result = await _energyStealService.GetStealableEnergy(
                    request.AuthKey,
                    request.StartTreeId,
                    request.StopTreeId,
                    request.MinDrop
                );

                // If there's an error message in the result, return a 400 Bad Request with the error message
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    return BadRequest(new { Message = "Error occurred while fetching stealable energy", Error = result.ErrorMessage });
                }

                // If no stealable energy is found, return a 204 No Content response
                if (!result.StealableEnergies.Any())
                {
                    return NoContent(); // 204 status code
                }

                // Return the result data with 200 OK status code
                return new JsonResult(result.StealableEnergies); // 200 status code
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully and return a 500 Internal Server Error with the error details
                return StatusCode(500, new { Message = "An internal error occurred while processing the request", Details = ex.Message });
            }
        }

    }

    // Define the request body structure
    public class EnergyRequest
    {
        public string AuthKey { get; set; }
        public int StartTreeId { get; set; }
        public int StopTreeId { get; set; }
        public int MinDrop { get; set; }
    }
}