using System;
using System.IO;
using System.Linq;
using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Resources.Embedded;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Kontecg.EmbeddedResources
{
    public class EmbeddedResourceFileProvider : IFileProvider
    {
        private readonly Lazy<IKontecgCoreConfiguration> _configuration;
        private readonly Lazy<IEmbeddedResourceManager> _embeddedResourceManager;
        private readonly IIocResolver _iocResolver;
        private bool _isInitialized;

        public EmbeddedResourceFileProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            _embeddedResourceManager = new Lazy<IEmbeddedResourceManager>(
                iocResolver.Resolve<IEmbeddedResourceManager>,
                true
            );

            _configuration = new Lazy<IKontecgCoreConfiguration>(
                iocResolver.Resolve<IKontecgCoreConfiguration>,
                true
            );
        }

        public IFileInfo GetFileInfo(string subPath)
        {
            if (!IsInitialized()) return new NotFoundFileInfo(subPath);

            var filename = Path.GetFileName(subPath);
            var resource = _embeddedResourceManager.Value.GetResource(subPath);

            if (resource == null || IsIgnoredFile(resource)) return new NotFoundFileInfo(subPath);

            return new EmbeddedResourceItemFileInfo(resource, filename);
        }

        public IDirectoryContents GetDirectoryContents(string subPath)
        {
            if (!IsInitialized()) return new NotFoundDirectoryContents();

            // The file name is assumed to be the remainder of the resource name.
            if (subPath == null) return new NotFoundDirectoryContents();

            var resources = _embeddedResourceManager.Value.GetResources(subPath);
            return new EmbeddedResourceItemDirectoryContents(resources
                .Where(r => !IsIgnoredFile(r))
                .Select(r => new EmbeddedResourceItemFileInfo(r, r.FileName.Substring(subPath.Length - 1))));
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        protected virtual bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != null &&
                   _configuration.Value.IgnoredFileExtensions.Contains(resource.FileExtension);
        }

        private bool IsInitialized()
        {
            if (_isInitialized) return true;

            _isInitialized = _iocResolver.IsRegistered<IEmbeddedResourceManager>();

            return _isInitialized;
        }
    }
}
