using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace BankingApi.Attributes
{
    public interface ICheckEncryptCache
    {
        Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next);
    }
}