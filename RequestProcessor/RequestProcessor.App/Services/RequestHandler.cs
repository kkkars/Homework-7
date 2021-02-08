using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RequestProcessor.App.Logging;
using RequestProcessor.App.Mappings;
using RequestProcessor.App.Models;
using RequestProcessor.App.Services;

namespace RequestProcessor.App
{
    internal class RequestHandler : IRequestHandler
    {
        private readonly HttpClient _client;
        private ILogger _logger = new Logger();

        public RequestHandler(HttpClient client)
        {
            _client = client;
            _client.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<IResponse> HandleRequestAsync(IRequestOptions requestOptions)
        {
            if (requestOptions == null) throw new ArgumentNullException(nameof(requestOptions));

            if (!requestOptions.IsValid) throw new ArgumentOutOfRangeException(nameof(requestOptions));
            _logger.Log($"Request: {{{requestOptions.Name}}} -> Request options are valid");

            using var message = new HttpRequestMessage(
                HttpMethodMapping.MapMethod(requestOptions.Method),
                new Uri(requestOptions.Address));
            _logger.Log("$Request: {{{ requestOptions.Name}}} -> Message was successfully created");

            if (requestOptions.Body != null)
            {
                message.Content = new StringContent(
                    requestOptions.Body,
                    Encoding.UTF8,
                    requestOptions.ContentType ?? "text/plain");
            }

            using var response = await _client.SendAsync(message);
            _logger.Log($"Sent an HTTP request for Request: {{{ requestOptions.Name}}}");

            string content = await GetContent(response);

            _logger.Log($"Request: {{{requestOptions.Name}}} -> Creating and returning response");
            return new Response(
                (int)response.StatusCode, 
                content, 
                true);
        }

        private async Task<string> GetContent(HttpResponseMessage message)
        {
            string content = default;

            try
            {
                content = await message.Content.ReadAsStringAsync();
            }
            catch (InvalidOperationException)
            {
                message.Content.Headers.ContentType.CharSet = "UTF-8";
                var byteArray = await message.Content.ReadAsByteArrayAsync();
                if (byteArray.Length != 0)
                {
                    content = Encoding.ASCII.GetString(byteArray);
                }
            }

            return content;
        }
    }
}