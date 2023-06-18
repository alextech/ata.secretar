using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SharedTestingKernel
{
    public class XUnitLoggerProvider : ILoggerProvider
    {
        private ITestOutputHelper Writer { get; set; }

        public XUnitLoggerProvider(ITestOutputHelper writer)
        {
            Writer = writer;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XUnitLogger(Writer);
        }

        private class XUnitLogger : ILogger
        {
            private ITestOutputHelper Writer { get; }

            public XUnitLogger(ITestOutputHelper writer)
            {
                Writer = writer;
                Name = nameof(XUnitLogger);
            }

            private string Name { get; set; }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                if (!this.IsEnabled(logLevel))
                    return;

                if (formatter == null)
                    throw new ArgumentNullException(nameof(formatter));

                string message = formatter(state, exception);
                if (string.IsNullOrEmpty(message) && exception == null)
                    return;

                string line = $"{logLevel}: {this.Name}: {message}";

                Writer.WriteLine(line);

                if (exception != null)
                    Writer.WriteLine(exception.ToString());
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new XUnitScope();
            }
        }

        private class XUnitScope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
