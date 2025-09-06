using System.Collections.Generic;
using Kontecg.Dependency;
using Kontecg.Resources.Embedded;

namespace Kontecg.EmbeddedResources
{
    public class EmbeddedResourceImageFileProvider : EmbeddedResourceFileProvider
    {
        private readonly HashSet<string> _allowedFileExtensions;

        public EmbeddedResourceImageFileProvider(IIocResolver iocResolver)
            : base(iocResolver)
        {
            _allowedFileExtensions =
            [
                "svg",
                "ico",
                "png"
            ];
        }

        protected override bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return !_allowedFileExtensions.Contains(resource.FileExtension);
        }
    }
}
