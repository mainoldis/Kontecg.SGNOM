using Itenso.TimePeriod;
using Kontecg.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Kontecg.WorkRelations
{
    public interface IFilterRequest : IPagedResultRequest, ISortedResultRequest
    {
        [Range(1, int.MaxValue)]
        int? Exp { get; set; }

        int? Year { get; set; }

        YearMonth? Month { get; set; }

        long[] ExcludedOrganizationUnitIds { get; set; }

        long[] OrganizationUnitIds { get; set; }

        [CanBeNull] string WorkPlacePaymentCode { get; set; }
    }
}