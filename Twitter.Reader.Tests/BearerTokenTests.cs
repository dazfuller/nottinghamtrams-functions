using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;
using Twitter.Reader;

namespace Twitter.Reader.Tests
{
    public class BearerTokenTests
    {
        private IConfigurationRoot _config;

        public BearerTokenTests()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", false, true)
                .Build();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestValidBearerTokenRequest()
        {
            var tokenProvider = new BearerTokenProvider(_config);
            var token = tokenProvider.GetBearerToken().Result;

            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestBearerTokenWithInvalidCredential()
        {
            _config["TwitterConsumerSecret"] = "BadSecret";

            var tokenProvider = new BearerTokenProvider(_config);
            
            Func<Task> functionCall = async () => {
                var token = await tokenProvider.GetBearerToken();
            };
            
            functionCall.Should()
                .Throw<TwitterAuthenticationException>()
                .And
                .Message
                .Contains("authenticity_token_error");
        }
    }
}
