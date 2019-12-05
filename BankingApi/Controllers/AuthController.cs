using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ApiAccess;

namespace BankingApi.Controllers
{
   
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("api/token")]
        [Produces("application/json")]
        public async Task<IActionResult> GetToken()
        {
            string token = await new ApplicationToken("mystrongsecretkey", "mar.in", "readers").GetJwtSecurityTokenAsync();
            
            return Ok(token);
        }
    }
}