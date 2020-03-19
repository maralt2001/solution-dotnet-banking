
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static HttpService.Content;

namespace ServiceHttp
{
    public class RequestMessageFactory
    {
        private readonly HttpMethod _httpMethod;
        private readonly string _requestUri;
        private readonly string _loginUri;
        private readonly CookieContainer _cookieContainer;

        public RequestMessageFactory(HttpMethod httpMethod, string requestUri, string loginUri, CookieContainer cookieContainer)
        {
            _httpMethod = httpMethod;
            _requestUri = requestUri;
            _loginUri = loginUri;
            _cookieContainer = cookieContainer;
        }

        public async Task<HttpRequestMessage> GetMessageAsync()
        {
            Task<HttpRequestMessage> result = Task.Run(() => {
                var x = PutCookiesOnRequest(new HttpRequestMessage(_httpMethod, _requestUri), _cookieContainer, _loginUri);
                return x;
            });

            return await result;
            
        }
    }
}
