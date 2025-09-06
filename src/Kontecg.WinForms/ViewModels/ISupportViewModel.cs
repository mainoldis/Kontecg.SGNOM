namespace Kontecg.ViewModels
{
    public interface ISupportViewModel
    {
        object ViewModel { get; }

        void ParentViewModelAttached();
    }
}