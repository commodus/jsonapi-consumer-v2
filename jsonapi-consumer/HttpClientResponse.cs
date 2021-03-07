using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonApiConsumer
{
    public class HttpClientResponse<T> where T : class
    {
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        public IList<Error> Errors { get; set; }

        public bool IsSuccessStatusCode { get; set; }

        public int StatusCode { get; set; }

        [JsonPropertyName("subStatusCode")]
        public string SubStatusCode { get; set; }
    }

    public class Error
    {
        public string code { get; set; }
        public string description { get; set; }
    }
}
