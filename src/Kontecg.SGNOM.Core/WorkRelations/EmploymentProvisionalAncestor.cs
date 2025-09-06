using System;

namespace Kontecg.WorkRelations
{
    public class EmploymentProvisionalAncestor
    {
        public long Id { get; set; }

        public long? PreviousId { get; set; }

        public EmploymentType Type { get; set; }

        public EmploymentSubType SubType { get; set; }

        public int Exp { get; set; }

        public string Code { get; set; }

        public long PersonId { get; set; }

        public Guid GroupId { get; set; }

        public int CompanyId { get; set; }
    }
}