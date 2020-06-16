using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;

namespace OrleansExperiment
{
    public class HelloGrain : Grain, IHello
    {
        private readonly ILogger _logger;

        public HelloGrain(ILogger<HelloGrain> logger)
        {
            _logger = logger;
        }

        Task<string> IHello.SayHello(string greeting)
        {
            _logger.LogInformation($"SayHello message received: greeting = '{greeting}'");
            return Task.FromResult($"You said: '{greeting}', I say: Hello!");
        }
    }
}