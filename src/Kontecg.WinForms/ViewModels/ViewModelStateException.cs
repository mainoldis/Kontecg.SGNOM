namespace Kontecg.ViewModels
{
    /// <summary>
    /// Excepción para operaciones de ViewModel que no están permitidas en el estado actual.
    /// </summary>
    public class ViewModelStateException : KontecgException
    {
        public ViewModelState CurrentState { get; }

        public ViewModelStateException(string message, ViewModelState currentState)
            : base(message)
        {
            CurrentState = currentState;
        }
    }
}