using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace Kontecg.EmbeddedResources
{
    public class EmbeddedResourceItemDirectoryContents : IDirectoryContents, IEnumerable
    {
        private readonly IEnumerable<IFileInfo> _entries;

        public EmbeddedResourceItemDirectoryContents(IEnumerable<IFileInfo> entries)
        {
            _entries = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        public bool Exists => true;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }
    }
}
