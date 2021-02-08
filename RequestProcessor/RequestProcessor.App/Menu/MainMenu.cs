using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RequestProcessor.App.Logging;
using RequestProcessor.App.Models;
using RequestProcessor.App.Services;
using RequestProcessor.App.Exceptions;

namespace RequestProcessor.App.Menu
{
    /// <summary>
    /// Main menu.
    /// </summary>
    internal class MainMenu : IMainMenu
    {
        IRequestPerformer _performer;
        IOptionsSource _optionsSource;
        ILogger _logger;

        IEnumerable<(IRequestOptions, IResponseOptions)> options;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="options">Options source</param>
        /// <param name="performer">Request performer.</param>
        /// <param name="logger">Logger implementation.</param>
        public MainMenu(
            IRequestPerformer performer,
            IOptionsSource options,
            ILogger logger)
        {
            _performer = performer;
            _optionsSource = options;
            _logger = logger;
        }

        public async Task<int> StartAsync()
        {
            try
            {
                await GetAllOptions();

                if (options.Any(option => !(option.Item1.IsValid && option.Item2.IsValid)))
                {
                    ShowInvalidResults();
                    GetValidOptions();
                }

                Task<bool>[] tasks = default;

                if (options.ToList().Count != 0)
                {
                    ShowValidResults();
                    tasks = PerformRequests();
                }

                if (tasks.Length != 0)
                {
                    ShowHandlingResult(tasks);
                }

                return 0;
            }
            catch (PerformException)
            {
                Console.WriteLine("Something went wrong.");
                return -1;
            }
        }

        private async Task GetAllOptions()
        {
            Console.WriteLine("Getting all request options from the file...\n");
            options = await _optionsSource.GetOptionsAsync();
            Console.WriteLine("Got all request options from the file.\n");
        }

        private void GetValidOptions()
        {
            Console.WriteLine("Getting rid of invalid request options...\n");
            options = options.Where(option => option.Item1.IsValid && option.Item2.IsValid).Select(option => option);
            Console.WriteLine("Got rid of invalid request options.\n");
        }

        private Task<bool>[] PerformRequests()
        {
            Console.WriteLine("Performing requests...\n");

            var tasks = options.Select(async opt =>  await _performer.PerformRequestAsync(opt.Item1, opt.Item2)).ToArray();
            Task.WaitAll(tasks);

            Console.WriteLine("Requests performing is finished.\n");
            return tasks;
        }

        private void ShowInvalidResults()
        {
            Console.WriteLine("Invalid request: requests below will not be handled.");
            options.Where(option => !(option.Item1.IsValid && option.Item2.IsValid)).ToList().ForEach(option => Console.WriteLine($"Request: {option.Item1.Name}."));
            Console.WriteLine();
        }

        private void ShowValidResults()
        {
            Console.WriteLine("Valid requests: request below are going to be handled");
            options.ToList().ForEach(option => Console.WriteLine($"Request: {option.Item1.Name}."));
            Console.WriteLine();
        }

        private void ShowHandlingResult(Task<bool>[] tasks)
        {
            Console.WriteLine("Handling results:");

            for (int i = 0; i < tasks.Length; i++)
            {
                Console.WriteLine($"Request: {options.ToArray()[i].Item1.Name} was Handled successfully: {tasks[i].Result}");
            }

            Console.WriteLine();
        }
    }
}
