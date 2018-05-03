using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Twitter.Reader
{
    public class TwitterClient
    {
        private const string UserTimelineApi = "https://api.twitter.com/1.1/statuses/user_timeline.json";

        private string _bearerToken;

        public TwitterClient(string bearerToken)
        {
            _bearerToken = bearerToken ?? throw new ArgumentNullException(nameof(bearerToken));
        }

        public async Task<IEnumerable<Tweet>> GetLatestTweets(string screenName, long? sinceId = null)
        {
            var queryStringParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("screen_name", screenName),
                new KeyValuePair<string, string>("trim_user", "1"),
                new KeyValuePair<string, string>("exclude_replies", "true"),
                new KeyValuePair<string, string>("include_rts", "false")
            };

            if (sinceId.HasValue)
            {
                queryStringParameters.Add(new KeyValuePair<string, string>("since_id", sinceId.Value.ToString()));
            }

            var queryString = string.Join("&", queryStringParameters.Select(p => $"{p.Key}={p.Value}"));
            var uri = new Uri($"{UserTimelineApi}?{queryString}");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = GetAuthorizationHeader();

                var response = await client.GetAsync(uri);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new TwitterException($"Unable to retrieve user timeline: {(await response.Content.ReadAsStringAsync())}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tweets = JsonConvert.DeserializeObject<List<Tweet>>(responseContent);

                return tweets;
            }
        }

        private AuthenticationHeaderValue GetAuthorizationHeader() => new AuthenticationHeaderValue("Bearer", _bearerToken);
    }
}