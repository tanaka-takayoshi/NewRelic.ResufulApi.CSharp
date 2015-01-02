using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewRelic.ResufulApi.CSharp
{
    internal class NewRelicAuthenticationHandler : DelegatingHandler
    {
        readonly string apikey;

        public NewRelicAuthenticationHandler(string apikey)
            : this(apikey, new HttpClientHandler())
        { }

        public NewRelicAuthenticationHandler(string apikey, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            this.apikey = apikey;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.Add("x-api-key", apikey);
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class NewRelicClient
    {
        private static readonly string baseUri = "https://api.newrelic.com/";
        
        readonly HttpClient httpClient;

        public long MaxResponseContentBufferSize
        {
            get
            {
                return httpClient.MaxResponseContentBufferSize;
            }
            set
            {
                httpClient.MaxResponseContentBufferSize = value;
            }
        }
 
        public TimeSpan Timeout
        {
            get
            {
                return httpClient.Timeout;
            }
            set
            {
                httpClient.Timeout = value;
            }
        }

        public NewRelicClient(string apikey)
        {
            httpClient = new HttpClient(new NewRelicAuthenticationHandler(apikey));
        }

        public NewRelicClient(string apikey, HttpMessageHandler innerHandler)
        {
            httpClient = new HttpClient(new NewRelicAuthenticationHandler(apikey, innerHandler));
        }

        public async Task NotifyDeployAsync(string appName, string description, string revision, string changelog, string user)
        {
            var dict = new Dictionary<string, string>
            {
                {"deployment[app_name]", appName},
                {"deployment[description]", description},
                {"deployment[revision]", revision},
                {"deployment[changelog]", changelog},
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(dict),
                RequestUri = new Uri(baseUri + "deployments.xml")
            };
            var res = await httpClient.SendAsync(request).ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
            {
                var message = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                Console.WriteLine(message);
            }
        }
    }
}
