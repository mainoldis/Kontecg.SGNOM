using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Runtime.Events
{
    public interface IStartupProcessHandler : IEventHandler<StartupProcessChanged>, IEventHandler<StartupProcessCompleted>
    {
    }
}
