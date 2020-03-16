using System;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Controllers;
using BankingApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ServiceDataProtection;
using ServiceRedis;
using StackExchange.Redis;

namespace BankingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckEncryptCache : Attribute, IAsyncActionFilter, ICheckEncryptCache
    {
        public static IServiceProvider CacheProvider { get; set; }
        public static ILogger Logger { get; set; }
        
        public CheckEncryptCache()
        {

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cache = (ICacheContext)CacheProvider.GetService(typeof(ICacheContext));
            RedisValue[] getValues = await cache.LoadStringsAsync(new RedisKey[] { "originData", "encryptData" },true);

            if (getValues[0] != context.ActionArguments.Values.FirstOrDefault().ToString()
                || string.IsNullOrEmpty(getValues[0])
                || string.IsNullOrEmpty(getValues[1]))
            {
                await next();
            }
            else
            {
                context.Result = new OkObjectResult(new ResponseEncryt { Cipher = getValues[1], CreatedAt = DateTime.Now.ToShortDateString() });
            }

        }
    }
}
