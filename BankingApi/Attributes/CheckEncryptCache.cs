using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceDataProtection;
using StackExchange.Redis;

namespace BankingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckEncryptCache : Attribute, IAsyncActionFilter
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
                var getCacheValue = cache.StringGet("encryptData");
                context.Result = new OkObjectResult(new ResponseEncryt { Cipher = getCacheValue, CreatedAt = DateTime.Now.ToShortDateString() });
            }
            

        }
    }
}
