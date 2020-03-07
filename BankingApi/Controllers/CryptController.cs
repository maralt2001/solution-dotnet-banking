﻿
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceDataProtection;
using BankingApi.Attributes;

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
        
        //implement check length of data
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
            if(response.Decrypted != string.Empty)
            {
                _logger.LogInformation("Return decrytion value");
                return Ok(response);
            }
            response.Decrypted = "decryption failed";
            _logger.LogWarning("decrytion failed");
            return StatusCode(400, response);


        }

        [HttpPost]
        [Route("api/banking/data/encrypt")]
        [Produces("application/json")]
        [CheckEncryptRequestBody(3)]
        public async Task<IActionResult> EncryptPost([FromBody] RequestEncrypt request)
        {
            var result = Task.Run(() => 
            {
                return EncryptionHelper.Encrypt(request.encryptData, request.key);
            });
            ResponseEncryt response = new ResponseEncryt
            {
                Cipher = await result
            };
            _logger.LogInformation("Return encryption value ");
            return Ok(response);
        }

        [HttpPost]
        [Route("api/banking/data/decrypt")]
        [Produces("application/json")]
        [CheckDecryptRequestBody(1)]
        public async Task<IActionResult> DecryptPost([FromBody] RequestDecrypt request)
        {
            var result = Task.Run(() =>
            {
                return EncryptionHelper.Decrypt(request.cipher, request.key);
            });
            ResponseDecrypt response = new ResponseDecrypt
            {
                Decrypted = await result
            };
            if(!string.IsNullOrEmpty(response.Decrypted))
            {
                _logger.LogInformation("Return decryption value ");
                return Ok(response);
            }
            response.Decrypted = "decryption failed";
            _logger.LogWarning("decrytion failed");
            return StatusCode(400, response);
           

        }
    }
}