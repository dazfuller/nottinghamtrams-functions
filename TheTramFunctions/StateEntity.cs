using System;
using Newtonsoft.Json;

namespace TheTramFunctions
{
    public class StateEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public long LatestId { get; set; }

        public DateTimeOffset LastUpdated { get; set; }
    }
}