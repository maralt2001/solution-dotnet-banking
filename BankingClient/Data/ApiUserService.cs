﻿using ApiAccess;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using BankingApi.Models;
using System.Net;
using Microsoft.Extensions.Configuration;
using static HttpService.Content;

namespace BankingClient.Data
{
    public class ApiUserService
    {
        private readonly CookieContainer _cookieContainer;
        private readonly string loginUrl;
        private readonly string logoutUrl;
        private readonly string registerUrl;

        public ApiUserService(CookieContainer cookieContainer, IConfiguration configuration)
        {
            _cookieContainer = cookieContainer;
            loginUrl = configuration.GetSection("BankingApiLoginPath").Value;
            logoutUrl = configuration.GetSection("BankingApiLogoutPath").Value;
            registerUrl = configuration.GetSection("BankingApiRegisterPath").Value;

        }

        // Login Request to api/user/login return LoginResult an Authorization Cookie
        public async Task<LoginResult> LoginUser(ApplicationUser applicationUser)
        {

            StringContent content = await GetSerializeStringContentAsync(applicationUser);
            Uri uri = new Uri(loginUrl);

            using HttpClient client = new HttpClient();
            var response = await client.PostAsync(loginUrl, content);

            var cookies = await GetCookiesAsync(response);

            if(cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    _cookieContainer.SetCookies(uri, cookie);
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

            using HttpClient client = new HttpClient();
            if (_cookieContainer.Count > 0)
            {
                HttpRequestMessage message = await PutCookiesOnRequest(new HttpRequestMessage(HttpMethod.Get, logoutUrl), _cookieContainer, loginUrl);
                
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

        //Register Request to api/user/register return a RegisterResult
        public async Task<RegisterResult> RegisterUser(ApplicationUser applicationUser)
        {
            StringContent content = await GetSerializeStringContentAsync(applicationUser);
            Uri uri = new Uri(registerUrl);

            using HttpClient client = new HttpClient();

            var response = await client.PostAsync(uri, content);
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
