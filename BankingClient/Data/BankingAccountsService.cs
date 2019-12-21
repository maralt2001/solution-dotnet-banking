using BankingApi.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static HttpService.Content;

namespace BankingClient.Data
{
    public class BankingAccountsService
    {
        private readonly CookieContainer _cookieContainer;
        private readonly string loginUrl;
        private readonly string getAllAccountsFromApi;

        public BankingAccountsService(CookieContainer cookieContainer, IConfiguration configuration)
        {
            _cookieContainer = cookieContainer;
            loginUrl = configuration.GetSection("BankingApiLoginPath").Value;
            getAllAccountsFromApi = configuration.GetSection("BankingApiGetAllAccounts").Value;
        }

        // Request api/banking/accounts/getall + set Authorization Cookie
        public async Task<BankingAccount[]> GetAccountsAsync()
        {
            
            HttpResponseMessage responseMessage;

            using HttpClient client = new HttpClient();
            if(_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, getAllAccountsFromApi), _cookieContainer, loginUrl);
                responseMessage = await client.SendAsync(message);
            }
            else
            {
                responseMessage = await client.GetAsync(getAllAccountsFromApi);
            }
            
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonstring = await responseMessage.Content.ReadAsStringAsync();
                return await GetDeserializeObjectAsync<BankingAccount[]>(jsonstring);
                
            }

            return new BankingAccount[0];
        }

        public async IAsyncEnumerable<BankingAccount> GetAccountsAsyncEnumerable()
        {

            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(getAllAccountsFromApi);
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
