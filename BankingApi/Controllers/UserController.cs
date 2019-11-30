using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoService;

namespace BankingApi.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IDBContext _dBContext;

        public UserController(IDBContext dBContext, ILogger<UserController> logger) 
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpPost]
        [Route("api/user/register")]
        [Produces("application/json")]

        public async Task<IActionResult> RegisterUser([FromBody] ApplicationUser applicationUser)
        {
            ApplicationUser newApiUser = new ApplicationUser(applicationUser.email, applicationUser.password);

            try
            {

                bool result = await _dBContext.InsertRecordAsync("Application_User", newApiUser);

                if (result)
                {
                     return Ok("User Account is created");
                }
                else
                {
                     return StatusCode(400, "Account is not created");
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        [Route("api/user/login")]
        [Produces("application/json")]
        public async Task<IActionResult> LoginUser(ApplicationUser applicationUser)
        {
            ApplicationUser appUser = await _dBContext.LoadOneRecordRegexAsync<ApplicationUser>("Application_User", "email", applicationUser.email);
            if(appUser != null)
            {
                //VerificationResults are "Success" or "Failed"
                PasswordVerificationResult result = await appUser.PasswordVerification(appUser.password, applicationUser.password);
                if (result.ToString() == "Success")
                {
                    return Ok("You are logged in");
                }
                else
                {
                    //To Do update accessFailedCount
                    return StatusCode(400, "You are not logged in");
                }
            }
            else
            {
                return StatusCode(400);
            }
        }
    }
}