using System;

namespace Kontecg.Services
{
    public interface ITransitionService
    {
        IDisposable StartTransition(bool forward, object waitParameter);

        IDisposable EndTransition();
    }
}