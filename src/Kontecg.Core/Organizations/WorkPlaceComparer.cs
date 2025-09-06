using System.Collections.Generic;

namespace Kontecg.Organizations
{
    public class WorkPlaceComparer : IComparer<WorkPlaceUnit>
    {
        public static readonly WorkPlaceComparer Instance = new();

        public int Compare(WorkPlaceUnit x, WorkPlaceUnit y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (ReferenceEquals(null, y))
                return 1;

            if (ReferenceEquals(null, x))
                return -1;

            if (int.Parse(x.Code) < int.Parse(y.Code))
                return -1;

            return int.Parse(x.Code) > int.Parse(y.Code) ? 1 : 0;
        }
    }
}
