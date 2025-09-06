using Kontecg.Domain.Entities.Auditing;
using System;
using Kontecg.Timing;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Kontecg.ExceptionHandling
{
    [Serializable]
    public class SnapshotCacheItem : IHasCreationTime
    {
        public const string CacheStoreName = "KontecgWinformSnapshots";

        public SnapshotCacheItem()
        {
            CreationTime = Clock.Now;
        }

        public SnapshotCacheItem(long? userId, Bitmap image)
            : this()
        {
            UserId = userId;
            if (image != null)
            {
                var stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                Snapshot = stream.ToArray();
            }
        }

        public long? UserId { get; set; }

        public byte[] Snapshot { get; set; }

        /// <inheritdoc />
        public DateTime CreationTime { get; set; }
    }
}