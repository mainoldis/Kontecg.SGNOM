using System;

namespace Kontecg.Domain
{
    public interface ISupportCustomFilters
    {
        event EventHandler CustomFiltersReset;
        void ResetCustomFilters();
    }
}
