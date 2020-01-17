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
using ServiceHttp;

namespace BankingClient.Data
{
    public class BankingAccountsService : IBankingAccountsService
    {
        private readonly CookieContainer _cookieContainer;
        private readonly string backEndUri;
        private readonly string relLoginUri;
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
            backEndUri = configuration.GetSection("BackEndUri").Value;
            relLoginUri = configuration.GetSection("RelLoginUri").Value;
            
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
                HttpRequestMessage message = await new RequestMessageFactory(HttpMethod.Get, getAllAccountsFromApi, backEndUri + relLoginUri, _cookieContainer).GetMessageAsync();
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

            
            return Array.Empty<BankingAccount>();
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
                    HttpRequestMessage message = await new RequestMessageFactory(HttpMethod.Get, queryhelper, backEndUri + relLoginUri, _cookieContainer).GetMessageAsync();
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
                    HttpRequestMessage message = await new RequestMessageFactory(HttpMethod.Get, queryhelper, backEndUri + relLoginUri, _cookieContainer).GetMessageAsync();
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
                    return Array.Empty<BankingAccount>();
                }

            });

            return result;
        }

        public async Task<bool> SaveAccountAsync(BankingAccount bankingAccount)
        {
            HttpResponseMessage responseMessage;

            if (_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await new RequestMessageFactory(HttpMethod.Post, postAccountToApi, backEndUri + relLoginUri, _cookieContainer).GetMessageAsync();
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
                HttpRequestMessage message = await new RequestMessageFactory(HttpMethod.Patch, queryhelper, backEndUri + relLoginUri, _cookieContainer).GetMessageAsync();
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
                HttpRequestMessage message = await new RequestMessageFactory(HttpMethod.Delete, queryhelper, backEndUri + relLoginUri, _cookieContainer).GetMessageAsync();
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
