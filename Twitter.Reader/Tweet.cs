using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Twitter.Reader
{
    public class Tweet
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAtString { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonIgnore]
        public DateTimeOffset CreatedAt => DateTimeOffset.ParseExact(CreatedAtString, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
    }
}