using Kontecg.Dependency;
using Kontecg.Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kontecg.Services
{
    public class WorkspaceService : IWorkspaceService, ITransientDependency
    {
        private ISupportLayout _supportLayout;
        private static string _defaultWorkspaceLayout;
        private static IDictionary<string, string> _layouts;

        public void Initialize(ISupportLayout supportLayout)
        {
            _supportLayout = supportLayout;
        }

        /// <inheritdoc />
        public void SetupDefaultWorkspace()
        {
            _layouts ??= new Dictionary<string, string>();
            _defaultWorkspaceLayout = Store();
        }

        /// <inheritdoc />
        public void SaveWorkspace(string workspaceName)
        {
            _layouts[workspaceName] = Store();
        }

        /// <inheritdoc />
        public void RestoreWorkspace(string workspaceName)
        {
            if (!_layouts.TryGetValue(workspaceName, out string layout))
                layout = _defaultWorkspaceLayout;
            Restore(layout);
        }

        /// <inheritdoc />
        public void ResetWorkspace(string workspaceName)
        {
            _layouts.Remove(workspaceName);
            Restore(_defaultWorkspaceLayout);
        }

        private string Store()
        {
            if (_supportLayout == null) return null;

            using var ms = new MemoryStream();
            _supportLayout.SaveLayoutToStream(ms);
            return Convert.ToBase64String(ms.ToArray());
        }

        private void Restore(string layout)
        {
            if (_supportLayout == null) return;
            if (string.IsNullOrEmpty(layout)) return;
            var bytes = Convert.FromBase64String(layout);
            using var ms = new MemoryStream(bytes);
            _supportLayout.RestoreLayoutFromStream(ms);
        }
    }
}