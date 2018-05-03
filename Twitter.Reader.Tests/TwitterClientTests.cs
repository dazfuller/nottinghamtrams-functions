using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;
using Twitter.Reader;
using System.Linq;

namespace Twitter.Reader.Tests
{
    public class TwitterClientTests
    {
        private IConfigurationRoot _config;

        public TwitterClientTests()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", false, true)
                .Build();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task TestUserTimelineRequest()
        {
            var client = await TwitterAuth.CreateClient(_config);
            var tweets = await client.GetLatestTweets("NETTram");

            tweets.Should().NotBeNull().And.HaveCountGreaterThan(0);
        }
    }
}
