using ApiAccess;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Configuration;
using ApiDataService;
using static HttpService.Content;
using ServiceHttp;

namespace BankingClient.Data
{
    public class ApiUserService
    {
        private readonly CookieContainer _cookieContainer;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string backEndUri;
        private readonly string relLoginUri;
        private readonly string relLogoutUri;
        private readonly string relRegisterUri;

        public ApiUserService(CookieContainer cookieContainer, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _cookieContainer = cookieContainer;
            _clientFactory = clientFactory;
            backEndUri = configuration.GetSection("BackEndUri").Value;
            relLoginUri = configuration.GetSection("RelLoginUri").Value;
            relLogoutUri = configuration.GetSection("RelLogoutUri").Value;
            relRegisterUri = configuration.GetSection("RelRegisterUri").Value;

        }

        // Login Request to api/user/login return LoginResult an Authorization Cookie
        public async Task<LoginResult> LoginUser(ApplicationUser applicationUser)
        {

            StringContent content = await GetSerializeStringContentAsync(applicationUser);
            Uri fullLoginPath = new Uri(backEndUri+relLoginUri);
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(backEndUri);
            

            var response = await client.PostAsync(relLoginUri, content);



            var cookies = await GetCookiesAsync(response);

            if(cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    _cookieContainer.SetCookies(fullLoginPath, cookie);
                }
                var jsonstring = await response.Content.ReadAsStringAsync();
                return await GetDeserializeObjectAsync<LoginResult>(jsonstring);
            }
            else
            {
                return new LoginResult { IsLoggedin = false };
            }

        }

        //Logout Request to api/user/logout return LoginResult and Authorization Cookie be deleted
        public async Task<LoginResult> LogoutUser()
        {

            HttpResponseMessage responseMessage;

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(backEndUri);

            if (_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await new RequestMessageFactory(HttpMethod.Get, backEndUri+relLogoutUri, backEndUri+relLoginUri, _cookieContainer ).GetMessageAsync();
                responseMessage = await client.SendAsync(message);
            }
            else
            {
                responseMessage = await client.GetAsync(relLogoutUri);
            }

            if (responseMessage.IsSuccessStatusCode)
            {

                var jsonstring = await responseMessage.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LoginResult>(jsonstring);
                return result;
            }
            else
            {
                var result = new LoginResult
                {
                    IsLoggedin = true
                };
                return result;
            }


        }

        //Register Request to api/user/register return a RegisterResult
        public async Task<RegisterResult> RegisterUser(ApplicationUser applicationUser)
        {
            StringContent content = await GetSerializeStringContentAsync(applicationUser);
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(backEndUri);

            HttpResponseMessage response = await client.PostAsync(relRegisterUri, content);
            if(response.IsSuccessStatusCode)
            {
                var jsonstring = await response.Content.ReadAsStringAsync();
                return await GetDeserializeObjectAsync<RegisterResult>(jsonstring);
               
            }
            else
            {
                return new RegisterResult { IsRegistered = false };
            }
            
        }
        
    }
}
