using ApiDataService;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static HttpService.Content;
using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BankingClient.Data
{
    public class BankingAccountsService : IBankingAccountsService
    {
        private readonly CookieContainer _cookieContainer;
        private readonly string loginUrl;
        private readonly string getAllAccountsFromApi;
        private readonly string getAccountsRegexFromApi;
        private readonly string postAccountToApi;


        private readonly ILogger<BankingAccountsService> _logger;


        public BankingAccountsService(CookieContainer cookieContainer, IConfiguration configuration, ILogger<BankingAccountsService> logger)
        {
            _cookieContainer = cookieContainer;
            loginUrl = configuration.GetSection("BankingApiLoginPath").Value;
            getAllAccountsFromApi = configuration.GetSection("BankingApiGetAllAccounts").Value;
            getAccountsRegexFromApi = configuration.GetSection("BankingApiGetAccountsRegex").Value;
            postAccountToApi = configuration.GetSection("BankingApiPostAccount").Value;
            _logger = logger;

        }

        // Request api/banking/accounts/getall + set Authorization Cookie
        public async Task<BankingAccount[]> GetAccountsAsync()
        {

            HttpResponseMessage responseMessage;

            using HttpClient client = new HttpClient();
            if (_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, getAllAccountsFromApi), _cookieContainer, loginUrl);
                _logger.LogInformation("Get Request to {0} ", getAllAccountsFromApi);
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
            var result = Task.Run(async () =>
            {
                HttpResponseMessage response;

                var query = new Dictionary<string, string>
                {
                    { "field", field },
                    { "regexvalue", value }
                };
                // Create Url with Query Params. Replace the double quotes from template string getAccountsRegexFromApi
                string queryhelper = QueryHelpers.AddQueryString(getAccountsRegexFromApi, query).Replace("\"", string.Empty);
                _logger.LogInformation("Get Request to {0}", queryhelper);

                using HttpClient client = new HttpClient();

                if (_cookieContainer.Count > 0)
                {
                    HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, queryhelper), _cookieContainer, loginUrl);
                    response = await client.SendAsync(message);
                }
                else
                {
                    response = await client.GetAsync(queryhelper);
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

                var query = new Dictionary<string, string>
                {
                    { "field", field },
                    { "regexvalue", value }
                };

                // Create Url with Query Params. Replace the double quotes from template string getAccountsRegexFromApi
                string queryhelper = QueryHelpers.AddQueryString(getAccountsRegexFromApi, query).Replace("\"", string.Empty);
                _logger.LogInformation("Get Request to {0}", queryhelper);

                using HttpClient client = new HttpClient();

                if (_cookieContainer.Count > 0)
                {
                    HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, queryhelper), _cookieContainer, loginUrl);
                    response = await client.SendAsync(message);
                }
                else
                {
                    response = await client.GetAsync(queryhelper);
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

        public async Task<bool> SaveAccountAsync(BankingAccount bankingAccount)
        {
            HttpResponseMessage responseMessage;

            using HttpClient client = new HttpClient();

            if (_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Post, postAccountToApi), _cookieContainer, loginUrl);
                message.Content = await GetSerializeStringContentAsync<BankingAccount>(bankingAccount);
                _logger.LogInformation("Post Request to {0} ", postAccountToApi);
                responseMessage = await client.SendAsync(message);
            }
            else
            {
                return false;
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
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
