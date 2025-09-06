using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kontecg.Presenters
{
    public class GroupEventArgs<TKey> : EventArgs
    {
        private readonly IEnumerable<TKey> _keysCore;

        public GroupEventArgs(IEnumerable<TKey> keysCore)
        {
            _keysCore = keysCore;
        }

        public IEnumerable<TKey> Entities
        {
            [DebuggerStepThrough] get => _keysCore;
        }
    }
}