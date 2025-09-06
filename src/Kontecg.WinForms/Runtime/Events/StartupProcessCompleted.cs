using System;
using Kontecg.Events.Bus;

namespace Kontecg.Runtime.Events
{
    public class StartupProcessCompleted : EventData
    {
        public TimeSpan Elapsed { get; }

        public StartupProcessCompleted(TimeSpan elapsed)
        {
            Elapsed = elapsed;
        }
    }
}
