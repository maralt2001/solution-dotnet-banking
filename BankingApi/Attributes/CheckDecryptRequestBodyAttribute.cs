using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceDataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckDecryptRequestBodyAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _cipherLength;
        public CheckDecryptRequestBodyAttribute(int cipherLength)
        {
            _cipherLength = cipherLength;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var body = context.ActionArguments["request"] as RequestDecrypt;

            if(!string.IsNullOrEmpty(body.key) && !string.IsNullOrEmpty(body.cipher))
            {
                if(body.cipher.Length > _cipherLength)
                {
                    await next();
                }
                else
                {
                    context.Result = new BadRequestObjectResult("Cipher length to sort");
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult($"Can not decrypt your request");
            }
        }
    }
}
