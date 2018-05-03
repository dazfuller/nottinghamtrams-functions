using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Twitter.Reader;

namespace Checker
{
    public class IssueManager
    {
        private readonly IConfigurationRoot _config;

        private readonly Uri _cosmosEndpoint;

        private readonly string _cosmosAccountKey;

        private readonly string _databaseId;

        private readonly string _collectionId;

        private readonly string _smtpServer;

        private readonly string _smtpPort;

        private readonly string _smtpUsername;

        private readonly string _smtpPassword;

        public IssueManager(IConfigurationRoot config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _cosmosEndpoint = new Uri(_config["TweetStoreEndPoint"]);
            _cosmosAccountKey = _config["TweetStoreAccountKey"];
            _databaseId = _config["DatabaseId"];
            _collectionId = _config["CollectionId"];
            _smtpServer = _config["SmtpServer"];
            _smtpPort = _config["SmtpPort"];
            _smtpUsername = _config["SmtpUsername"];
            _smtpPassword = _config["SmtpPassword"];
        }

        public async Task SaveIssues(IEnumerable<Tweet> tweets)
        {
            SendEmail(tweets);
            
            using (var client = new DocumentClient(_cosmosEndpoint, _cosmosAccountKey))
            {
                var collectionUri = await Initialise(client);

                foreach (var tweet in tweets)
                {
                    var document = new
                    {
                        id = $"{tweet.Id:d19}",
                        CreatedAtString = tweet.CreatedAtString,
                        CreatedAt = tweet.CreatedAt,
                        Language = tweet.Language,
                        Text = tweet.Text
                    };

                    await client.CreateDocumentAsync(collectionUri, document);
                }
            }
        }

        private void SendEmail(IEnumerable<Tweet> tweets)
        {
            var message = new MailMessage(_config["FromAddress"], _config["ToAddress"])
            {
                Subject = "DELAYS: Possible posts about Tram issues"
            };

            var messageBuilder = new StringBuilder("<p>The following posts have been identified as discussing potential issues with the trams</p>");

            foreach (var tweet in tweets)
            {
                messageBuilder.AppendFormat("<p><b>{0:o}</b> - {1}</p>", tweet.CreatedAt, tweet.Text);
            }

            message.Body = messageBuilder.ToString();
            message.IsBodyHtml = true;

            using (var client = new SmtpClient(_smtpServer, Convert.ToInt32(_smtpPort)))
            {
                var credential = new System.Net.NetworkCredential(_smtpUsername, _smtpPassword);
                client.Credentials = credential;

                client.Send(message);
            }
        }

        private async Task<Uri> Initialise(DocumentClient client)
        {
            var database = client.CreateDatabaseQuery()
                .Where(db => db.Id == _databaseId)
                .AsEnumerable()
                .FirstOrDefault();
            
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = _databaseId });
            }

            var databaseUri = UriFactory.CreateDatabaseUri(_databaseId);

            var collection = client.CreateDocumentCollectionQuery(databaseUri)
                .Where(c => c.Id == _collectionId)
                .AsEnumerable()
                .FirstOrDefault();

            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(databaseUri, new DocumentCollection { Id = _collectionId });
            }

            return UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId);
        }
    }
}