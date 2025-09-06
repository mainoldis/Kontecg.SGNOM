using System;

namespace Kontecg.ViewModels
{
    /// <summary>
    /// Información estadística del ViewModel para dashboards o reportes.
    /// </summary>
    public class ViewModelStatistics
    {
        public int TotalEntities { get; set; }

        public int FilteredEntities { get; set; }

        public int LoadCount { get; set; }

        public int CreateCount { get; set; }

        public int UpdateCount { get; set; }

        public int DeleteCount { get; set; }

        public DateTime LastLoadTime { get; set; }

        public TimeSpan AverageLoadTime { get; set; }
    }
}