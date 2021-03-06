﻿
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceDataProtection;
using BankingApi.Attributes;
using System;
using ServiceRedis;
using BankingApi.RedisData;

namespace BankingApi.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CryptController : ControllerBase
    {
        private readonly ILogger<CryptController> _logger;
        
        private readonly ICacheContext _cacheContext;

        public CryptController(ILogger<CryptController> logger, ICacheContext cacheContext)
        {
            _logger = logger;
            _cacheContext = cacheContext;

        }

        [HttpGet]
        [Route("api/banking/data/encrypt")]
        [Produces("application/json")]
        [CheckEncryptCache]
        //implement check length of data
        public async Task<IActionResult> Encrypt([FromQuery] string data)
        {
            var result = Task.Run(() =>
            {

                return EncryptionHelper.Encrypt(data, "abc");
            });
            ResponseEncryt response = new ResponseEncryt
            {
                Cipher = await result,
                CreatedAt = DateTime.Now.ToShortDateString()
            };
            
            bool saveInCache = await _cacheContext.SaveHashAsync<CacheDataCrypt>(
                new CacheDataCrypt {OriginData = data, EncryptData = response.Cipher }, 
                HttpContext.Connection, true);

            return saveInCache ? new OkObjectResult(response) as IActionResult : 
                new BadRequestObjectResult(new ResponseEncryt {Cipher = "Something went wrong", CreatedAt = DateTime.Now.ToLocalTime().ToString()}) as IActionResult;
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
                return new OkObjectResult(response);
            }
            _logger.LogWarning("decrytion failed");
            return new BadRequestObjectResult($"Can not decrypt your request");


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
                Cipher = await result,
                CreatedAt = DateTime.Now.ToShortDateString()
            };
            _logger.LogInformation("Return encryption value ");
            return new OkObjectResult(response);
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
            _logger.LogWarning("decrytion failed");
            return new BadRequestObjectResult($"Can not decrypt your request");
            
           

        }
    }
}