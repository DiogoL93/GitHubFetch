using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Call_Github_API
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
               .AddJsonFile("appSettings.json", false, false);

            IConfiguration config = builder.Build();

            try
            {
                var task = GitHubFetch.getStatistics(config["GitHubOwner"], config["GitHubRepo"]);
                task.Wait();
                var stats = task.Result;
                var lines = stats.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
