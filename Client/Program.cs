using System;
using System.Threading.Tasks;
using Contracts;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;

namespace Client
{
    public class Program
    {
        const int InitializeAttemptsBeforeFailing = 5;

        private static int _attempt = 0;

        public static async Task<int> Main()
        {
            try
            {
                using var client = await StartClientWithRetries();
                await DoClientWork(client);
                Console.ReadKey();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> StartClientWithRetries()
        {
            _attempt = 0;
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect(RetryFilter);
            Console.WriteLine("Client successfully connect to silo host");
            return client;
        }

        private static async Task<bool> RetryFilter(Exception exception)
        {
            if (exception.GetType() != typeof(SiloUnavailableException))
            {
                Console.WriteLine(
                    $"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
                return false;
            }

            _attempt++;
            Console.WriteLine(
                $"Cluster client attempt {_attempt} of {InitializeAttemptsBeforeFailing} failed to connect to cluster.  Exception: {exception}");
            if (_attempt > InitializeAttemptsBeforeFailing)
            {
                return false;
            }

            await Task.Delay(TimeSpan.FromSeconds(4));
            return true;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            var friend = client.GetGrain<IHello>(0);
            var response = await friend.SayHello("Good morning, my friend!");
            Console.WriteLine("\n\n{0}\n\n", response);
        }
    }
}