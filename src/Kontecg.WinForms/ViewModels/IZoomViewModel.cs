using Kontecg.Domain;
using System;

namespace Kontecg.ViewModels
{
    public interface IZoomViewModel
    {
        ISupportZoom ZoomComponent { get; }

        event EventHandler ZoomComponentChanged;
    }
}
