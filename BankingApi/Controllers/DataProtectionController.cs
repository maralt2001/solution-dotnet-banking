
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceApiData;
using ServiceDataProtection;

namespace BankingApi.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class DataProtectionController : ControllerBase
    {
        private readonly ILogger<DataProtectionController> _logger;
        private readonly IProtector _protector;

        public DataProtectionController(ILogger<DataProtectionController> logger, IProtector protector)
        {
            _logger = logger;
            _protector = protector;
        }

        [HttpPost]
        [Route("api/banking/data/encrypt")]
        [Produces("application/json")]
        public async Task<IActionResult> EncryptData([FromBody] DataProtection protection )
        {
            
            var result = Task.Run(() => {
                string encrypt = _protector.EncryptData<DataProtection>(protection, "Stage1");
                return encrypt;
            });

            var output = await result;
            if(output != null)
            {
                return Ok(output);
            }
            else
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("api/banking/data/decrypt")]
        [Produces("application/json")]
        public async Task<IActionResult> DecryptData([FromBody] DataProtection protection)
        {
            var result = Task.Run(() => {

                var decyrpt = _protector.GetDecryptData<DataProtection>(protection.data, "Stage1");
                return decyrpt;
            
            });

            var output = await result;

            if (output != null)
            {
                return Ok(output);
            }
            else
            {
                return StatusCode(500);
            }

        }
    }
}