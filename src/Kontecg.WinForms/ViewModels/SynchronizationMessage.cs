using Kontecg.Application.Services.Dto;
using Kontecg.Events.Bus;

namespace Kontecg.ViewModels
{
    public class SynchronizationMessage<TEntityDto>
        : SynchronizationMessage<TEntityDto, int> where TEntityDto : class, IEntityDto<int>
    {
        /// <inheritdoc />
        public SynchronizationMessage(TEntityDto entity, ViewModelEntityState state) : base(entity, state)
        {
        }
    }

    public class SynchronizationMessage<TEntityDto, TPrimaryKey> : EventData
        where TEntityDto : class, IEntityDto<TPrimaryKey>
    {
        /// <inheritdoc />
        public SynchronizationMessage(TEntityDto entity, ViewModelEntityState state)
        {
            Entity = entity;
            State = state;
        }

        public TEntityDto Entity { get; private set; }

        public ViewModelEntityState State { get; private set; }
    }
}