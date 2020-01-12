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
using BankingClient.Provider;


namespace BankingClient.Data
{
    public class BankingAccountsService : IBankingAccountsService
    {
        private readonly CookieContainer _cookieContainer;
        private readonly string loginUrl;
        private readonly string getAllAccountsFromApi;
        private readonly string getAccountsRegexFromApi;
        private readonly string postAccountToApi;
        private readonly string updateAccountToApi;
        private readonly string deleteAccountToApi;

        private readonly ILogger<BankingAccountsService> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly UserState _userState;


        public BankingAccountsService(CookieContainer cookieContainer, IConfiguration configuration, ILogger<BankingAccountsService> logger, UserState userState, IHttpClientFactory clientFactory)
        {
            _cookieContainer = cookieContainer;
            loginUrl = configuration.GetSection("BankingApiLoginPath").Value;
            getAllAccountsFromApi = configuration.GetSection("BankingApiGetAllAccounts").Value;
            getAccountsRegexFromApi = configuration.GetSection("BankingApiGetAccountsRegex").Value;
            postAccountToApi = configuration.GetSection("BankingApiPostAccount").Value;
            updateAccountToApi = configuration.GetSection("BankingApiUpdateAccount").Value;
            deleteAccountToApi = configuration.GetSection("BankingApiDeleteAccount").Value;
            _logger = logger;
            _clientFactory = clientFactory;
            _userState = userState;

        }

        // Request api/banking/accounts/getall + set Authorization Cookie
        public async Task<BankingAccount[]> GetAccountsAsync()
        {

            HttpResponseMessage responseMessage;
            
            if (_cookieContainer.Count > 0)
            {

                HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, getAllAccountsFromApi), _cookieContainer, loginUrl);
                _logger.LogInformation("Get Request to {0} ", getAllAccountsFromApi);
                responseMessage = await _clientFactory.CreateClient().SendAsync(message);
            }
            else
            {
                responseMessage = await _clientFactory.CreateClient().GetAsync(getAllAccountsFromApi);
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

                if (_cookieContainer.Count > 0)
                {
                    HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, queryhelper), _cookieContainer, loginUrl);
                    response = await _clientFactory.CreateClient().SendAsync(message);
                }
                else
                {
                    response = await _clientFactory.CreateClient().GetAsync(queryhelper);
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

                

                if (_cookieContainer.Count > 0)
                {
                    HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, queryhelper), _cookieContainer, loginUrl);
                    response = await _clientFactory.CreateClient().SendAsync(message);
                }
                else
                {
                    response = await _clientFactory.CreateClient().GetAsync(queryhelper);
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

            if (_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Post, postAccountToApi), _cookieContainer, loginUrl);
                message.Content = await GetSerializeStringContentAsync<BankingAccount>(bankingAccount);
                _logger.LogInformation("Post Request to {0} ", postAccountToApi);
                responseMessage = await _clientFactory.CreateClient().SendAsync(message);
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

        public async Task<bool> UpdateAccountAsync(BankingAccount bankingAccount)
        {
            HttpResponseMessage responseMessage;

            var query = new Dictionary<string, string>
                {
                    { "id", bankingAccount._id }
                   
                };
            string queryhelper = QueryHelpers.AddQueryString(updateAccountToApi, query).Replace("\"", string.Empty);

            if (_cookieContainer.Count > 0) //Request with Cookies
            {
                HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Patch, queryhelper), _cookieContainer, loginUrl);
                message.Content = await GetSerializeStringContentAsync<PatchBankingAccount>(new PatchBankingAccount(bankingAccount, DateTime.Now, _userState.Username));
                _logger.LogInformation("Patch Request to {0} ", queryhelper);
                responseMessage = await _clientFactory.CreateClient().SendAsync(message);
            }
            else // Request without Cookies
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Patch, queryhelper)
                {
                    Content = await GetSerializeStringContentAsync<PatchBankingAccount>(new PatchBankingAccount(bankingAccount, DateTime.Now, _userState.Username))
                };
                _logger.LogInformation("Patch Request to {0} ", queryhelper);
                responseMessage = await _clientFactory.CreateClient().SendAsync(message);
            }

            if(responseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> DeleteAccount(string id)
        {
            HttpResponseMessage responseMessage;

            var query = new Dictionary<string, string>
            {
                {"id", id }
            };

            string queryhelper = QueryHelpers.AddQueryString(deleteAccountToApi, query).Replace("\"", string.Empty);

            if(_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Delete, queryhelper), _cookieContainer, loginUrl);
                _logger.LogInformation("Delete Request to {0} ", queryhelper);
                responseMessage = await _clientFactory.CreateClient().SendAsync(message);
            }
            else
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, queryhelper);
                responseMessage = await _clientFactory.CreateClient().SendAsync(message);
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async IAsyncEnumerable<BankingAccount> GetAccountsAsyncEnumerable()
        {
            
            var response = await _clientFactory.CreateClient().GetAsync(getAllAccountsFromApi);
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
