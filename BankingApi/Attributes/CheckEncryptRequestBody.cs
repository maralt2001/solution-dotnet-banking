using BankingApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceDataProtection;
using System;
using System.Threading.Tasks;


namespace BankingApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CheckEncryptRequestBodyAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _value;
        
        public CheckEncryptRequestBodyAttribute(int value)
        {
            _value = value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var body = context.ActionArguments["request"] as RequestEncrypt;
            await CheckBody(context, next, body);

        }

        private async Task CheckBody(ActionExecutingContext context, ActionExecutionDelegate next, RequestEncrypt body)
        {
            if (!string.IsNullOrEmpty(body.key) && !string.IsNullOrEmpty(body.encryptData))
            {
                if (body.key.Length >= _value && body.encryptData.Length >= _value)
                {
                    await next();
                }
                else
                {
                    context.Result = new BadRequestObjectResult($"Proberty value is less then {_value}");
                }

            }
            else
            {
                context.Result = new BadRequestObjectResult($"Can not encrypt your request");
            }
        }

    }
}
