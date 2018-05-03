using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Twitter.Reader
{
    public class BearerTokenProvider
    {
        private IConfigurationRoot _config;

        private string _bearerToken;

        public BearerTokenProvider(IConfigurationRoot config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _bearerToken = _config["TwitterBearerToken"];
        }

        public async Task<string> GetBearerToken()
        {
            if (string.IsNullOrEmpty(_bearerToken))
            {
                _bearerToken = await CreateBearerToken();
                _config["TwitterBearerToken"] = _bearerToken;
            }
            
            return _bearerToken;
        }

        private async Task<string> CreateBearerToken()
        {
            var credential = GetConsumerCredential();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);

                var content = new FormUrlEncodedContent(new [] {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var response = await client.PostAsync(_config["TwitterAuthApi"], content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(responseContent)["access_token"].ToString();
                }
                else
                {
                    throw new TwitterAuthenticationException($"Unable to retrieve bearer token: {(await response.Content.ReadAsStringAsync())}");
                }
            }
        }

        private string GetConsumerCredential()
        {
            var consumerKey = WebUtility.UrlEncode(_config["TwitterConsumerKey"]);
            var consumerSecret = WebUtility.UrlEncode(_config["TwitterConsumerSecret"]);

            var bearerTokenCredential = $"{consumerKey}:{consumerSecret}";
            var credentialBytes = Encoding.UTF8.GetBytes(bearerTokenCredential);
            var base64Credential = Convert.ToBase64String(credentialBytes);

            return base64Credential;
        }
    }
}