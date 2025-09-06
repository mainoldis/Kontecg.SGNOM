using System;
using Kontecg.Runtime.Validation;

namespace Kontecg.ViewModels
{
    /// <summary>
    /// Excepción para validaciones de UI/negocio específicas del ViewModel.
    /// Se distingue de las validaciones de datos que maneja ABP.
    /// </summary>
    public class ViewModelValidationException : KontecgValidationException
    {
        public ViewModelValidationException(string message) : base(message) { }
        public ViewModelValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}