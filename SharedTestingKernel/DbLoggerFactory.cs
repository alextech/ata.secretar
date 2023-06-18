using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SharedTestingKernel
{
    public static class DbLoggerFactory
    {
        public static ILoggerFactory CreateLoggerForOutput(ITestOutputHelper output)
        {
            ServiceProvider serviceProvider = new ServiceCollection().AddLogging().BuildServiceProvider();
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new XUnitLoggerProvider(output));

            return loggerFactory;
        }
    }
}