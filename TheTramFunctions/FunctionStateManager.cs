using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace TheTramFunctions
{
    public class FunctionStateManager
    {
        private const string CollectionId = "FunctionState";

        private readonly IConfigurationRoot _config;

        private readonly string _functionName;

        private readonly Uri _cosmosEndpoint;

        private readonly string _cosmosAccountKey;

        private readonly string _databaseId;

        public FunctionStateManager(string functionName, IConfigurationRoot config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _functionName = functionName ?? throw new ArgumentNullException(nameof(functionName));
            _cosmosEndpoint = new Uri(_config["TweetStoreEndPoint"]);
            _cosmosAccountKey = _config["TweetStoreAccountKey"];
            _databaseId = _config["DatabaseId"];
        }

        public async Task SaveLatestState(long latestId)
        {
            using (var client = new DocumentClient(_cosmosEndpoint, _cosmosAccountKey))
            {
                var collectionUri = await Initialise(client);

                var state = new StateEntity()
                {
                    Id = _functionName,
                    LatestId = latestId,
                    LastUpdated = DateTimeOffset.UtcNow
                };

                await client.UpsertDocumentAsync(collectionUri, state);
            }
        }

        public async Task<long?> GetLatestState()
        {
            using (var client = new DocumentClient(_cosmosEndpoint, _cosmosAccountKey))
            {
                var collectionUri = await Initialise(client);

                try
                {
                    var currentStateResult = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, CollectionId, _functionName));
                    var currentState = (StateEntity)(dynamic)currentStateResult.Resource;
                    return currentState.LatestId;
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && e.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                }

                return null;
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
                .Where(c => c.Id == CollectionId)
                .AsEnumerable()
                .FirstOrDefault();

            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(databaseUri, new DocumentCollection { Id = CollectionId });
            }

            return UriFactory.CreateDocumentCollectionUri(_databaseId, CollectionId);
        }
    }
}