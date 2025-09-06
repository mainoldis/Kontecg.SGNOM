namespace Kontecg.ViewModels
{
    public class SingleObjectViewModelState
    {
        public object[] Key { get; set; }

        public string Title { get; set; }

        public bool AllowSaveReset { get; set; }

        public bool IsSharedTreeRoot { get; set; }
    }
}