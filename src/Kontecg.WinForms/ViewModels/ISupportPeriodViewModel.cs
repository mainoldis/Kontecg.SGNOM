using System;
using Itenso.TimePeriod;

namespace Kontecg.ViewModels
{
    public interface ISupportPeriodViewModel
    {
        ITimePeriod Period { get; set; }

        event EventHandler PeriodChanged;
    }
}