using BankingApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BankingClient.Data
{
    public class BankingAccountsService
    {

        public async Task<BankingAccount[]> GetAccountsAsync()
        {
            string baseUrl = "http://localhost:5000/api/banking/accounts/getall";

            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(baseUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<BankingAccount[]>(jsonstring);

                return result;
            }

            return new BankingAccount[0];
        }

        public async IAsyncEnumerable<BankingAccount> GetAccountsAsyncEnumerable()
        {
            string baseUrl = "http://localhost:5000/api/banking/accounts/getall";

            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(baseUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<IAsyncEnumerable<BankingAccount>>(jsonstring);

                await foreach (var item in result)
                {
                    yield return item;
                }
            }

        }
    }
}
