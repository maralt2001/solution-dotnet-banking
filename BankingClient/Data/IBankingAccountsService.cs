using ApiDataService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingClient.Data
{
    public interface IBankingAccountsService
    {
        Task<BankingAccount[]> GetAccountsAsync();
        IAsyncEnumerable<BankingAccount> GetAccountsAsyncEnumerable();
        Task<BankingAccount[]> GetAccountsRegexAsync(string field, string value);
        Task<BankingAccount> GetOneAccountAsync(string field, string value);
        Task<bool> SaveAccountAsync(BankingAccount bankingAccount);
        Task<bool> UpdateAccountAsync(BankingAccount bankingAccount);
    }
}