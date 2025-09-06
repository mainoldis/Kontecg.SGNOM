namespace Kontecg.Organizations
{
    public class OrganizationUnitAncestor
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public long? ParentId { get; set; }

        public int Level { get; set; }

        public string Kind { get; set; }

        public string WorkPlacePaymentCode { get; set; }

        public OrganizationUnitAncestor(long id, string code, string displayName, long? parentId, int level, string kind, string workPlacePaymentCode)
        {
            Id = id;
            Code = code;
            DisplayName = displayName;
            ParentId = parentId;
            Level = level;
            Kind = kind;
            WorkPlacePaymentCode = workPlacePaymentCode;
        }
    }
}