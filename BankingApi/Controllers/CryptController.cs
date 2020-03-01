
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceDataProtection;

namespace BankingApi.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CryptController : ControllerBase
    {
        private readonly ILogger<CryptController> _logger;

        public CryptController(ILogger<CryptController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("api/banking/data/encrypt")]
        [Produces("application/json")]
        public async Task<IActionResult> Encrypt([FromQuery] string data)
        {
            var result = Task.Run(() => 
            {
                return EncryptionHelper.Encrypt(data, "abc");
            });
            ResponseEncryt response = new ResponseEncryt
            {
                Cipher = await result
            };
            _logger.LogInformation("Return encryption value ");
            return Ok(response);

        }

        [HttpGet]
        [Route("api/banking/data/decrypt")]
        [Produces("application/json")]
        public async Task<IActionResult> Decrypt([FromQuery] string cipher)
        {
            var result = Task.Run(() => 
            {
                return EncryptionHelper.Decrypt(cipher, "abc");
            });
            ResponseDecrypt response = new ResponseDecrypt
            {
                Decrypted = await result
            };
            _logger.LogInformation("Return decrytion value");
            return Ok(response);
        }
    }
}