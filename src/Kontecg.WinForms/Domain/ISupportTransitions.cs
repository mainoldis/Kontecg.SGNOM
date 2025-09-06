using System;

namespace Kontecg.Domain
{
    public interface ISupportTransitions : IDisposable
    {
        void StartTransition(bool forward, object waitParameter);

        void EndTransition();
    }
}
