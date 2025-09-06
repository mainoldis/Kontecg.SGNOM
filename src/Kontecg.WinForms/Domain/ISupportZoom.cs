using System;

namespace Kontecg.Domain
{
    public interface ISupportZoom
    {
        int ZoomLevel { get; set; }

        event EventHandler ZoomChanged;
    }
}
