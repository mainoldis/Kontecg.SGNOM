using System;

namespace Kontecg.ViewModels
{
    public class PrintEventArgs : EventArgs
    {
        public PrintEventArgs(object parameter)
        {
            Parameter = parameter;
        }
        public object Parameter { get; private set; }
    }
}
