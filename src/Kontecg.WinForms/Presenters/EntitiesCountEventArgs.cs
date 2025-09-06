using System;

namespace Kontecg.Presenters
{
    public class EntitiesCountEventArgs : EventArgs
    {
        /// <inheritdoc />
        public EntitiesCountEventArgs(int count)
        {
            Count = count;
        }

        public int Count { get; private set; }
    }
}