namespace Kontecg.Organizations.Dto
{
    public class WorkPlaceUnitDto : OrganizationUnitDto
    {
        public WorkPlaceUnitDto Parent { get; set; }

        public string Description { get; set; }

        public int MaxMembersApproved { get; set; }

        public string Classification { get; set; }

        public int ClassificationId { get; set; }

        public int Level { get; set; }

        public string WorkPlacePaymentCode { get; set; }

        public long? WorkPlacePaymentId { get; set; }

        public int CenterCostCount { get; set; }
    }
}
