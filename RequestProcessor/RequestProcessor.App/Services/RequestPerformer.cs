using System;
using System.Threading.Tasks;
using RequestProcessor.App.Logging;
using RequestProcessor.App.Models;
using RequestProcessor.App.Exceptions;

namespace RequestProcessor.App.Services
{
    /// <summary>
    /// Request performer.
    /// </summary>
    internal class RequestPerformer : IRequestPerformer
    {
        private readonly IRequestHandler _requestHandler;
        private readonly IResponseHandler _responseHandler;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="requestHandler">Request handler implementation.</param>
        /// <param name="responseHandler">Response handler implementation.</param>
        /// <param name="logger">Logger implementation.</param>

        public RequestPerformer(
            IRequestHandler requestHandler,
            IResponseHandler responseHandler,
            ILogger logger)
        {
            _requestHandler = requestHandler;
            _responseHandler = responseHandler;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> PerformRequestAsync(
            IRequestOptions requestOptions,
            IResponseOptions responseOptions)
        {
            bool isSuccess;

            if (requestOptions == null)
                throw new ArgumentNullException(nameof(requestOptions));

            if (responseOptions == null)
                throw new ArgumentNullException(nameof(responseOptions));

            if (!requestOptions.IsValid)
                throw new ArgumentOutOfRangeException(nameof(responseOptions));
            _logger.Log("Request options are valid");

            if (!responseOptions.IsValid)
                throw new ArgumentOutOfRangeException(nameof(responseOptions));
            _logger.Log("Response options are valid");

            try
            {
                var responce = await _requestHandler.HandleRequestAsync(requestOptions);
                _logger.Log("Request was send and the response was received");

                await _responseHandler.HandleResponseAsync(responce, requestOptions, responseOptions);
                _logger.Log($"Request: {{{requestOptions.Name}}} -> response was written to the file {{{responseOptions.Path}}}");

                isSuccess = true;
                _logger.Log($"PerformRequestAsync for request {{{requestOptions.Name}}} was success: {isSuccess}");
            }
            catch (TaskCanceledException ex)
            {
                _logger.Log(ex, $"TimeOut Exception: waiting for the response for too long");

                await _responseHandler.HandleResponseAsync(new Response(-1, "Timeout Error", false), requestOptions, responseOptions);
                _logger.Log($"Unhandled response was written to the file {{{responseOptions.Path}}} successfully");

                isSuccess = false;
                _logger.Log($"PerformRequestAsync for request {{{requestOptions.Name}}} was success: {isSuccess}");
            }
            catch (Exception e)
            {
                _logger.Log(e, $"Unexpected Error");
                throw new PerformException($"Failed to perform the request {{{requestOptions.Name}}}", e);
            }

            return isSuccess;
        }
    }
}
