using Itenso.TimePeriod;
using Kontecg.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace Kontecg.WorkRelations
{
    public class FilterRequest : IFilterRequest, IShouldNormalize
    {
        /// <inheritdoc />
        [Range(1, int.MaxValue)]
        public int MaxResultCount { get; set; } = KontecgCoreConsts.DefaultPageSize;

        /// <inheritdoc />
        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        /// <inheritdoc />
        public string Sorting { get; set; }

        /// <inheritdoc />
        public int? Exp { get; set; }

        /// <inheritdoc />
        public int? Year { get; set; }

        /// <inheritdoc />
        public YearMonth? Month { get; set; }

        /// <inheritdoc />
        public long[] OrganizationUnitIds { get; set; } = [];

        public long[] ExcludedOrganizationUnitIds { get; set; } = [];

        /// <inheritdoc />
        public string WorkPlacePaymentCode { get; set; }

        /// <inheritdoc />
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting)) Sorting = "Exp";

            if (!string.IsNullOrEmpty(WorkPlacePaymentCode))
            {
                if (!int.TryParse(WorkPlacePaymentCode, out var number) && number > 0)
                    WorkPlacePaymentCode = null;
            }
        }
    }
}