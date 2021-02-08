using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RequestProcessor.App.Logging;
using RequestProcessor.App.Models;
using RequestProcessor.App.Services;

namespace RequestProcessor.App
{
    /// <summary>
    /// OptionSource class consume data from options file
    /// </summary>
    internal class OptionsSource : IOptionsSource
    {
        private readonly string _path;
        private ILogger _logger = new Logger();

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
                {
                  new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
        };

        public OptionsSource(string path)
        {
            _path = path;
        }

        public async Task<IEnumerable<(IRequestOptions, IResponseOptions)>> GetOptionsAsync()
        {
            var json = await File.ReadAllTextAsync(_path);
            _logger.Log($"All options were written from the file {{{_path}}}");

            var options = JsonSerializer.Deserialize<List<RequestOptions>>(json, JsonOptions);
            _logger.Log("All options were deserialized");

            return options.Select(option => ((IRequestOptions)option, (IResponseOptions)option)).ToArray();
        }
    }
}
