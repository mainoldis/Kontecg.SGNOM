using System;
using System.Diagnostics;

namespace Kontecg.Presenters
{
    public class EntityEventArgs<TPrimaryKey> : EventArgs
    {
        private readonly TPrimaryKey _key;

        /// <inheritdoc />
        public EntityEventArgs(TPrimaryKey key)
        {
            _key = key;
        }

        public TPrimaryKey Key
        {
            [DebuggerStepThrough] get => _key;
        }
    }
}