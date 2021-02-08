using System;
using System.Text.Json.Serialization;
using RequestProcessor.App.Models;

namespace RequestProcessor.App
{
    internal class RequestOptions : IRequestOptions, IResponseOptions
    {
        public RequestOptions()
        {
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("method")]
        public RequestMethod Method { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonIgnore]
        bool IRequestOptions.IsValid =>
            Uri.TryCreate(Address ?? string.Empty, UriKind.Absolute, out _) &&
            Method != RequestMethod.Undefined &&
            ((ContentType == null && Body == null) || (!string.IsNullOrWhiteSpace(ContentType) && !string.IsNullOrWhiteSpace(Body)));

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonIgnore]
        bool IResponseOptions.IsValid => !string.IsNullOrWhiteSpace(Path);
    }
}
