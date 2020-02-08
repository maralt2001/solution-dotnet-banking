using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiAccess;
using Microsoft.Extensions.Configuration;
using MongoService;
using ServiceApiData;

namespace BankingApi.Controllers
{
   
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IDBContext _dBContext;

        public AuthController(IConfiguration configuration, IDBContext dbContext)
        {
            this.configuration = configuration;
            _dBContext = dbContext;
        }

        [HttpPost]
        [Route("api/token")]
        [Produces("application/json")]
        public async Task<IActionResult> GetToken([FromBody] ApplicationUser applicationUser)
        {
            string token = string.Empty;

            if(applicationUser != null)
            {
                
                var result = await _dBContext.LoadRecordsRegexAsync<ApplicationUser>("Application_User", "email", applicationUser.email);
                

                if (result != null)
                {
                    token = await new ApplicationToken(configuration).GetJwtSecurityTokenAsync();
                    var jwtUser = new JwtUserResult { UserEmail = applicationUser.email, UserToken = token };
                    return Ok(jwtUser);
                }
                else
                {
                    return NotFound();
                }

            }


            return NotFound();
        }
    }
}