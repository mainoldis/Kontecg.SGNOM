using System.Windows.Forms;
using Kontecg.Dependency;

namespace Kontecg.Services
{
    public class ClipboardService : IClipboardService, ITransientDependency
    {
        /// <inheritdoc />
        public void SetText(string text)
        {
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }
    }
}