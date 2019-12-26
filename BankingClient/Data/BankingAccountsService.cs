using ApiDataService;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly string getOneAccountRegexFromApi;
        private readonly string getAccountsRegexFromApi;

        public BankingAccountsService(CookieContainer cookieContainer, IConfiguration configuration)
        {
            _cookieContainer = cookieContainer;
            loginUrl = configuration.GetSection("BankingApiLoginPath").Value;
            getAllAccountsFromApi = configuration.GetSection("BankingApiGetAllAccounts").Value;
            getOneAccountRegexFromApi = configuration.GetSection("BankingApiGetOneAccountRegex").Value;
            getAccountsRegexFromApi = configuration.GetSection("BankingApiGetAccountsRegex").Value;
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

        public Task<BankingAccount> GetOneAccountAsync(string field, string value)
        {
            var result = Task.Run(async() => 
            {
                HttpResponseMessage response;
                string concatUrl = ($"{getOneAccountRegexFromApi}?field={field}&regexvalue={value}");

                using HttpClient client = new HttpClient();

                if (_cookieContainer.Count > 0)
                {
                    HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, concatUrl), _cookieContainer, loginUrl);
                    response = await client.SendAsync(message);
                }
                else
                {
                    response = await client.GetAsync(concatUrl);
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonstring = await response.Content.ReadAsStringAsync();

                    return await GetDeserializeObjectAsync<BankingAccount>(jsonstring);
                }
                else
                {
                    return new BankingAccount();
                }

            });

            return result;
            
        }

        public Task<BankingAccount[]> GetAccountsRegexAsync(string field, string value)
        {
            var result = Task.Run(async () =>
            {
                HttpResponseMessage response;
                string concatUrl = ($"{getAccountsRegexFromApi}?field={field}&regexvalue={value}");

                using HttpClient client = new HttpClient();

                if (_cookieContainer.Count > 0)
                {
                    HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, concatUrl), _cookieContainer, loginUrl);
                    response = await client.SendAsync(message);
                }
                else
                {
                    response = await client.GetAsync(concatUrl);
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonstring = await response.Content.ReadAsStringAsync();

                    return await GetDeserializeObjectAsync<BankingAccount[]>(jsonstring);
                }
                else
                {
                    return new BankingAccount[0];
                }

            });

            return result;
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
