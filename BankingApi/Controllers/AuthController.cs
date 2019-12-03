using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
            Task<JwtSecurityToken> result = Task.Run(() =>

            {
                string securityKey = "mystrongsecretkey";

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(

                    issuer: "mar.in",
                    audience: "readers",
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials

                    );
                return token;

            });
            

            return Ok(new JwtSecurityTokenHandler().WriteToken(await result));
        }
    }
}