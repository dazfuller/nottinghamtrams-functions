using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Checker;
using Twitter.Reader;

namespace TheTramFunctions
{
    public static class TwitterReaderFunction
    {
        [FunctionName("TwitterReaderFunction")]
        public static async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup=true)]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", true)
                .Build();
            
            var stateManager = new FunctionStateManager("TwitterReader", configuration);
            var latestId = await stateManager.GetLatestState();

            var client = await TwitterAuth.CreateClient(configuration);
            var tweets = await client.GetLatestTweets("NETTram", latestId);

            var checker = new SimpleTextChecker();
            var issueManager = new IssueManager(configuration);

            var issueTweets = tweets.Where(t => checker.TextMatchesKeywords(t.Text));
            
            if (issueTweets.Count() > 0)
            {
                await issueManager.SaveIssues(issueTweets);
            }

            if (tweets.Count() > 0)
            {
                var maxId = tweets.Max(t => t.Id);
                await stateManager.SaveLatestState(maxId);
            }
        }
    }
}
