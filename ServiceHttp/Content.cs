using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace HttpService
{
    public abstract class Content
    {
        private static StringContent _stringContent;
        private static IEnumerable<string> _cookieCollection;
        
        
        // Serialize an Object to StringContent
        public static Task<StringContent> GetSerializeStringContentAsync<T>(T DataObject)
        {
            Task<StringContent> result = Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(DataObject);
                _stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                return _stringContent;
            });

            return result;
        }

        // Deserialize to Object
        public static Task<T> GetDeserializeObjectAsync<T>(string JsonString)
        {
            Task<T> result = Task.Run(() =>
            {
                var _content = JsonConvert.DeserializeObject<T>(JsonString);
                return _content;
            });

            return result;
        }

        // Get Cookies from ResponseMessage and return IEnumerable
        public static Task<IEnumerable<string>> GetCookiesAsync(HttpResponseMessage ResponseMessage)
        {
            var result = Task.Run(() => 
            {
                if (ResponseMessage.Headers.Contains(HeaderNames.SetCookie))
                {
                    _cookieCollection = ResponseMessage.Headers.GetValues(HeaderNames.SetCookie);
                    return _cookieCollection;
                }
                else
                {
                    return _cookieCollection;
                }
            });

            return result;

            
        }

        public static Task<HttpRequestMessage> PutCookiesOnRequest(HttpRequestMessage message, CookieContainer cookieContainer, string LoginUrl)
        {
            Task<HttpRequestMessage> result = Task.Run(() =>
            {
                var cookies = cookieContainer.GetCookies(new Uri(LoginUrl)).AsQueryable().Cast<Cookie>();
                

                var aspnetCookies = cookies.Where(c => c.Name == ".AspNetCore.Cookies");

                foreach (var cookie in aspnetCookies)
                {
                    message.Headers.Add("Cookie", new CookieHeaderValue(cookie.Name, cookie.Value).ToString());
                }

                return message;

            });


            return result;

        }

        public static Task<AuthenticationHeaderValue> GetAuthenticationHeaderWithJWT(string jsonWebToken)
        {
            return Task.Run(() =>
            {
                AuthenticationHeaderValue x = new AuthenticationHeaderValue("Bearer", jsonWebToken);
                return x;
            });
           

        }



    }
}
