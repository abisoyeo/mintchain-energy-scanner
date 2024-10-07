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

                // Return the result in the response
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully and return a meaningful error response
                return StatusCode(500, new { Message = "An error occurred while fetching stealable energy", Details = ex.Message });
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