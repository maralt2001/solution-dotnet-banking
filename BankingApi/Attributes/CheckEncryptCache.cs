using System;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Controllers;
using BankingApi.Helpers;
using BankingApi.RedisData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceDataProtection;
using ServiceRedis;

namespace BankingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckEncryptCache : Attribute, IAsyncActionFilter, ICheckEncryptCache
    {
        public static ICacheContext CacheContext { get; set; }
        
        public CheckEncryptCache()
        {

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            CacheDataCrypt getFromCache = await CacheContext.LoadHashAsync<CacheDataCrypt>(context.HttpContext.Connection, true);

            if(string.IsNullOrEmpty(getFromCache.EncryptData) || string.IsNullOrEmpty(getFromCache.OriginData))
            {
                await next();
            }
            else
            {
                context.Result = new OkObjectResult(new ResponseEncryt { Cipher = getFromCache.EncryptData, CreatedAt = DateTime.Now.ToShortDateString() });
            }

        }
    }
}
