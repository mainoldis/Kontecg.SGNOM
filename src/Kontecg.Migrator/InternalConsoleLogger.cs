using System;
using Castle.Core.Logging;
using Kontecg.Timing;

namespace Kontecg.Migrator
{
    internal class InternalConsoleLogger : ConsoleLogger
    {
        public InternalConsoleLogger()
            : base(string.Empty, LoggerLevel.Info)
        {
        }

        protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
        {
            Console.Out.WriteLine("{0:yyyy-MM-dd HH:mm:ss} | {1}", Clock.Now, (object)message);
            if (exception == null)
                return;

            Console.Out.WriteLine("{0:yyyy-MM-dd HH:mm:ss} | {1}: {2}", Clock.Now, (object)exception.GetType().FullName, (object)exception.Message);
        }
    }
}
