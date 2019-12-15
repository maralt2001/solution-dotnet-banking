using ApiAccess;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BankingApi.Models;
using Microsoft.Net.Http.Headers;
using System.Net;
using BankingClient.Provider;
using Microsoft.Extensions.Configuration;

namespace BankingClient.Data
{
    public class ApiUserService
    {
        private readonly CookieContainer _cookieContainer;
        private readonly string loginUrl;
        private readonly string logoutUrl;

        public ApiUserService(CookieContainer cookieContainer, IConfiguration configuration)
        {
            _cookieContainer = cookieContainer;
            loginUrl = configuration.GetSection("BankingApiLoginPath").Value;
            logoutUrl = configuration.GetSection("BankingApiLogoutPath").Value;
            
        }

        // Login Request to api/user/login return LoginResult an Authorization Cookie
        public async Task<LoginResult> LoginUser(ApplicationUser applicationUser)
        {
            
            
            Uri uri = new Uri(loginUrl);
            var json = JsonConvert.SerializeObject(applicationUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
           
            using HttpClient client = new HttpClient();
            var response = await client.PostAsync(loginUrl, stringContent);
           
             if(response.Headers.Contains(HeaderNames.SetCookie))
             {
                var cookies = response.Headers.GetValues(HeaderNames.SetCookie);

                foreach (var cookie in cookies)
                {

                    _cookieContainer.SetCookies(uri, cookie);
                    
                }

             }
           
            var jsonstring = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResult>(jsonstring);
            return result;
            
        }

       
        //Logout Request to api/user/logout return LoginResult and Authorization Cookie be deleted
        public async Task<LoginResult> LogoutUser()
        {
            
            HttpResponseMessage responseMessage;

            using HttpClient client = new HttpClient();
            if (_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await CookieHelper.PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, logoutUrl),_cookieContainer, loginUrl);
                responseMessage = await client.SendAsync(message);
            }
            else
            {
                responseMessage = await client.GetAsync(logoutUrl);
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
    }
}
