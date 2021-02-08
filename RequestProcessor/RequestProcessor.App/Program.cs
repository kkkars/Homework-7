using System;
using System.Net.Http;
using System.Threading.Tasks;
using RequestProcessor.App.Menu;
using RequestProcessor.App.Services;

namespace RequestProcessor.App
{
    /// <summary>
    /// Entry point.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <returns>Returns exit code.</returns>
        private static async Task<int> Main()
        {
            Console.WriteLine("HTTP-request Processor by Bilotska Karyna\n");

            var requestPerformer = new RequestPerformer(new RequestHandler(new HttpClient()), new ResponseHandler(), new Logger());
            var optionSource = new OptionsSource("options.json");
            var logger = new Logger();

            try
            {
                var mainMenu = new MainMenu(requestPerformer, optionSource, logger);
                return await mainMenu.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Critical unhandled exception");
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
