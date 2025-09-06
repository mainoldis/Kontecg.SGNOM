using System.Drawing;
using System.Windows.Forms;
using Kontecg.Runtime;
using System.Drawing.Imaging;
using System.IO;
using Kontecg.Application.Clients;
using Kontecg.Storage;

namespace Kontecg.ExceptionHandling
{
    public class SnapshotManager : KontecgCoreDomainServiceBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IClientFactory _clientFactory;
        private readonly IWinFormsRuntime _winFormsRuntime;

        /// <inheritdoc />
        public SnapshotManager(
            IWinFormsRuntime winFormsRuntime, 
            ITempFileCacheManager tempFileCacheManager, 
            IClientFactory clientFactory)
        {
            _winFormsRuntime = winFormsRuntime;
            _tempFileCacheManager = tempFileCacheManager;
            _clientFactory = clientFactory;
        }

        public virtual TempFileInfo GrabSnapshot()
        {
            var mainFormTarget = _winFormsRuntime.MainForm?.Target as Form;
            if (mainFormTarget == null) return null;

            // Shot size = screen size
            Size shotSize = Screen.FromControl(mainFormTarget).Bounds.Size;
            // the upper left point in the screen to start shot
            // 0,0 to get the shot from upper left point
            Point upperScreenPoint = new Point(mainFormTarget.Left, mainFormTarget.Top);

            // create image to get the shot in it
            Bitmap shot = new Bitmap(shotSize.Width, shotSize.Height);

            // new Graphics instance
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(shot);

            // get the shot by Graphics class
            graphics.CopyFromScreen(upperScreenPoint, Point.Empty, shotSize);

            using var stream = new MemoryStream();
            shot.Save(stream, ImageFormat.Jpeg);

            var clientInfo = _clientFactory.Create();
            TempFileInfo tempFileInfo = new TempFileInfo($"snapshot_{clientInfo.Id}.jpg", "image/jpeg", stream.ToArray());
            _tempFileCacheManager.SetFile(clientInfo.Id, tempFileInfo);
            return tempFileInfo;
        }
    }
}