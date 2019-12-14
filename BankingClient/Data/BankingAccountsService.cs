using BankingApi.Models;
using BankingClient.Provider;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BankingClient.Data
{
    public class BankingAccountsService
    {
        private readonly CookieContainer _cookieContainer;

        public BankingAccountsService(CookieContainer cookieContainer)
        {
            _cookieContainer = cookieContainer;
        }

        // Request api/banking/accounts/getall + set Authorization Cookie
        public async Task<BankingAccount[]> GetAccountsAsync()
        {
            string baseUrl = "http://localhost:5000/api/banking/accounts/getall";
            string cookieLoginUrl = "http://localhost:5000/api/user/login";
            HttpResponseMessage responseMessage;

            using HttpClient client = new HttpClient();
            if(_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await CookieHelper.PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, baseUrl), _cookieContainer, cookieLoginUrl);
                responseMessage = await client.SendAsync(message);
            }
            else
            {
                responseMessage = await client.GetAsync(baseUrl);
            }
            
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonstring = await responseMessage.Content.ReadAsStringAsync();

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
