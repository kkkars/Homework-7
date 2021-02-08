using System;
using System.IO;
using System.Threading.Tasks;
using RequestProcessor.App.Logging;
using RequestProcessor.App.Models;
using RequestProcessor.App.Services;

namespace RequestProcessor.App
{
    class ResponseHandler : IResponseHandler
    {
        private ILogger _logger = new Logger();

        public async Task HandleResponseAsync(IResponse response, IRequestOptions requestOptions, IResponseOptions responseOptions)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (requestOptions == null)
                throw new ArgumentNullException(nameof(requestOptions));

            if (responseOptions == null)
                throw new ArgumentNullException(nameof(responseOptions));

            string result = $"Status code: {{{response.Code}}}  Request: {{{requestOptions.Name}}} to {{{requestOptions.Address}}} was Handled {{{response.Handled}}}\n Content: {response.Content}";
            _logger.Log($"Request: {{{requestOptions.Name}}} -> request result was written as a string");

            await File.WriteAllTextAsync(responseOptions.Path, result);
            _logger.Log($"Request: {{{requestOptions.Name}}} -> request result string was written to the result file {{{responseOptions.Path}}}");
        }
    }
}
