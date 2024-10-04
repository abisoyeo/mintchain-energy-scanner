using Microsoft.AspNetCore.Mvc;
using MintChainDropsApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MintChainDropsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Drops : ControllerBase
    {
        // GET: api/<Drops>
        [HttpGet]
        public IEnumerable<string> Get(DropsModel dropsModel)
        {
            /*
             * Hitting this endpoint, pass the auth, and other values needed to call mint api
             * 
             * assign those values to the class here
             * create a function to now query mint api passing in the class
             * in the function you use a loop to 
             */

            return new string[] { "value1", "value2" };
        }

    }
}
