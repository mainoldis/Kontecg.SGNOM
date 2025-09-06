using System;
using Castle.Core.Logging;

namespace Kontecg.SampleApp
{
    internal class InternalConsoleLoggerFactory : ILoggerFactory
    {
        public InternalConsoleLoggerFactory()
        {
        }

        public ILogger Create(Type type) => (ILogger)new InternalConsoleLogger();

        public ILogger Create(string name) => (ILogger)new InternalConsoleLogger();

        public ILogger Create(Type type, LoggerLevel level) => (ILogger)new InternalConsoleLogger();

        public ILogger Create(string name, LoggerLevel level) => (ILogger)new InternalConsoleLogger();
    }
}
