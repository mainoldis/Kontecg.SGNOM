using DevExpress.Mvvm;
using Kontecg.Application.Services.Dto;
using System.Collections.ObjectModel;

namespace Kontecg.ViewModels
{
    /// <summary>
    /// The base interface for view models exposing a collection of entities of the given type.
    /// </summary>
    public interface IEntitiesViewModel<TEntityDto, TPrimaryKey> : IDocumentContent where TEntityDto : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// The loaded collection of entities.
        /// </summary>
        ObservableCollection<TEntityDto> Entities { get; }

        /// <summary>
        /// Used to check whether entities are currently being loaded in the background. The property can be used to show the progress indicator.
        /// </summary>
        bool IsLoading { get; }
    }
}