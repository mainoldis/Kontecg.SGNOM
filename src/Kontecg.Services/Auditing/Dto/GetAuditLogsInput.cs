using System;
using Kontecg.Common;
using Kontecg.Dto;
using Kontecg.Extensions;
using Kontecg.Runtime.Validation;

namespace Kontecg.Auditing.Dto
{
    public class GetAuditLogsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string UserName { get; set; }

        public string ServiceName { get; set; }

        public string MethodName { get; set; }

        public string ClientInfo { get; set; }

        public bool? HasException { get; set; }

        public int? MinExecutionDuration { get; set; }

        public int? ExecutionDuration { get; set; }

        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace()) Sorting = "ExecutionTime DESC";

            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                if (s.IndexOf("UserName", StringComparison.OrdinalIgnoreCase) >= 0)
                    s = "User." + s;
                else
                    s = "AuditLog." + s;

                return s;
            });
        }
    }
}
