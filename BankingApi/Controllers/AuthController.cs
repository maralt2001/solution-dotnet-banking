using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiAccess;
using Microsoft.Extensions.Configuration;

namespace BankingApi.Controllers
{
   
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("api/token")]
        [Produces("application/json")]
        public async Task<IActionResult> GetToken()
        {
            string token = await new ApplicationToken(configuration).GetJwtSecurityTokenAsync();
            
            return Ok(token);
        }
    }
}