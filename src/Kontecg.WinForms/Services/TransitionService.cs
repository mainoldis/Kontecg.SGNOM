using System;
using Kontecg.Dependency;
using Kontecg.Domain;

namespace Kontecg.Services
{
    public class TransitionService : ITransitionService, ITransientDependency
    {
        private ISupportTransitions _supportTransitions;

        public void Initialize(ISupportTransitions supportTransitions)
        {
            _supportTransitions = supportTransitions;
        }

        /// <inheritdoc />
        public IDisposable StartTransition(bool forward, object waitParameter)
        {
            _supportTransitions?.StartTransition(forward, waitParameter);
            return _supportTransitions;
        }

        /// <inheritdoc />
        public IDisposable EndTransition()
        {
            _supportTransitions?.EndTransition();
            return _supportTransitions;
        }
    }
}