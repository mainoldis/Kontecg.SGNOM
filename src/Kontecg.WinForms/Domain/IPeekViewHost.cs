using Kontecg.Views;

namespace Kontecg.Domain
{
    public interface IPeekViewHost
    {
        bool IsDockedModule(Module moduleType);

        void DockModule(Module moduleType);

        void UndockModule(Module moduleType);

        void ShowPeek(Module moduleType);
    }
}
