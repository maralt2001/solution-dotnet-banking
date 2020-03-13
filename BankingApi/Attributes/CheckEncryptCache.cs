using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceDataProtection;
using StackExchange.Redis;

namespace BankingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckEncryptCache : Attribute, IAsyncActionFilter, ICheckEncryptCache
    {
        public static IServiceProvider CacheProvider { get; set; }

        public CheckEncryptCache()
        {
            
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cache = (IDatabase) CacheProvider.GetService(typeof(IDatabase));
            
            if(cache.StringGet("originData") != context.ActionArguments.Values.FirstOrDefault().ToString())
            {
                await next();
            }
            else
            {
                var getCacheValue = cache.StringGet("encryptdata");
                context.Result = new OkObjectResult(new ResponseEncryt { Cipher = getCacheValue });
            }
            

        }
    }
}
