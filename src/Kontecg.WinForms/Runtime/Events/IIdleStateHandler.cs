using Kontecg.Events.Bus.Handlers;

namespace Kontecg.Runtime.Events
{
    public interface IIdleStateHandler : IEventHandler<IdleStateEntering>
    {
    }
}
