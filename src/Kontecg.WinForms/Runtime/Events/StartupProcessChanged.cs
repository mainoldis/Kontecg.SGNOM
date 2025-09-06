using Kontecg.Events.Bus;

namespace Kontecg.Runtime.Events
{
    public class StartupProcessChanged : EventData
    {
        public string Status { get; }

        public StartupProcessChanged(string status)
        {
            Status = status;
        }
    }
}
