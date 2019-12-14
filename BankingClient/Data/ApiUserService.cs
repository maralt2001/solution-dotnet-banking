using ApiAccess;
using BankingApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BankingApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;

using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using System.Net;
using BankingClient.Provider;

namespace BankingClient.Data
{
    public class ApiUserService
    {
        private readonly CookieContainer _cookieContainer;

        public ApiUserService(CookieContainer cookieContainer)
        {
            _cookieContainer = cookieContainer;
            
        }

        // Login Request to api/user/login return LoginResult an Authorization Cookie
        public async Task<LoginResult> LoginUser(ApplicationUser applicationUser)
        {
            
            string baseUrl = "http://localhost:5000/api/user/login";
            Uri uri = new Uri(baseUrl);
            var json = JsonConvert.SerializeObject(applicationUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
           
            using HttpClient client = new HttpClient();
            var response = await client.PostAsync(baseUrl, stringContent);
           
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
            string baseUrl = "http://localhost:5000/api/user/logout";
            string cookieLoginUrl = "http://localhost:5000/api/user/login";
            HttpResponseMessage responseMessage;

            using HttpClient client = new HttpClient();
            if (_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await CookieHelper.PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, baseUrl),_cookieContainer, cookieLoginUrl);
                responseMessage = await client.SendAsync(message);
            }
            else
            {
                responseMessage = await client.GetAsync(baseUrl);
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
