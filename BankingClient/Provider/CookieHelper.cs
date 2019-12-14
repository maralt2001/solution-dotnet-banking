using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BankingClient.Provider
{
    public class CookieHelper
    {

        public static Task<HttpRequestMessage> PutCookiesOnRequest(HttpRequestMessage message, CookieContainer cookieContainer, string url)
        {
            var result = Task.Run(() => 
            {
                var cookies = cookieContainer.GetCookies(new Uri(url)).AsQueryable<Cookie>();

                var aspnetCookies = cookies.Where(c => c.Name == ".AspNetCore.Cookies");

                foreach (var cookie in aspnetCookies)
                {
                    message.Headers.Add("Cookie", new CookieHeaderValue(cookie.Name, cookie.Value).ToString());
                }

                return message;

            });


            return result;
            
        }

       

    }
}
