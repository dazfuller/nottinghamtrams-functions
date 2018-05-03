using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Twitter.Reader
{
    public static class TwitterAuth
    {
        public static async Task<TwitterClient> CreateClient(IConfigurationRoot config)
        {
            var bearerProvider = new BearerTokenProvider(config);
            var bearerToken = await bearerProvider.GetBearerToken();

            return new TwitterClient(bearerToken);
        }
    }
}
