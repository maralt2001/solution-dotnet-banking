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

namespace BankingClient.Data
{
    public class ApiUserService
    {
        public async Task<LoginResult> LoginUser(ApplicationUser applicationUser)
        {
            string baseUrl = "http://localhost:5000/api/user/login";
            var json = JsonConvert.SerializeObject(applicationUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpClient client = new HttpClient();
            var response = await client.PostAsync(baseUrl, stringContent);
            var jsonstring = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResult>(jsonstring);
            return result;
            
        }

        public async Task<LoginResult> LogoutUser(ApplicationUser applicationUser)
        {
            string baseUrl = "http://localhost:5000/api/user/logout";
            var json = JsonConvert.SerializeObject(applicationUser);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpClient client = new HttpClient();
            var response = await client.PostAsync(baseUrl, stringContent);

            if(response.IsSuccessStatusCode)
            {
                var jsonstring = await response.Content.ReadAsStringAsync();
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
