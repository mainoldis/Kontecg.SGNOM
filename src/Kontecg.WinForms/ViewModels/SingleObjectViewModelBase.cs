#if false
using Kontecg.Application.Services.Dto;
using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace Kontecg.ViewModels
{
    [POCOViewModel]
    public class SingleObjectViewModelBase<TEntityDto, TPrimaryKey> : KontecgViewModelBase,
        ISingleObjectViewModel<TEntityDto, TPrimaryKey>,
        ISupportParameter,
        IDocumentContent,
        ISupportLogicalLayout<SingleObjectViewModelState>,
        ISupportLogicalLayout,
        ISupportParentViewModel
        where TEntityDto : class, IEntityDto<TPrimaryKey>
    {
        private Action<TEntityDto> _entityInitializer;
        private ViewModelEntityState _entityState;
        private bool _dontUpdateEntityState;
        private object _title;

        public virtual bool AllowSaveReset { get; protected set; }

        /// <summary>
        ///   <para>The display text for a given entity used as a title in the corresponding view.</para>
        /// </summary>
        public object Title => _title;

        /// <inheritdoc />
        public virtual TEntityDto Entity { get; protected set; }

        /// <inheritdoc />
        public TPrimaryKey Id { get; }

        private enum ViewModelEntityState
        {
            ExistingUnchanged,
            New,
            Changed,
        }
    }
}
#endif