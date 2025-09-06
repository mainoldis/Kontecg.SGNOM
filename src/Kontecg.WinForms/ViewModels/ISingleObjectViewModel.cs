using Kontecg.Application.Services.Dto;

namespace Kontecg.ViewModels
{
    /// <summary>
    /// The base interface for view models exposing a entity of the given type.
    /// </summary>
    public interface ISingleObjectViewModel<out TEntityDto, out TPrimaryKey>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// The loaded entity.
        /// </summary>
        TEntityDto Entity { get; }

        /// <summary>
        /// The loaded entity primary key.
        /// </summary>
        TPrimaryKey Id { get; }
    }
}