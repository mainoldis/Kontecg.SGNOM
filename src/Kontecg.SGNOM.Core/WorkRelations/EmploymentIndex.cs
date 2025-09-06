using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;

namespace Kontecg.WorkRelations
{
    [Table("employment_indexes", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class EmploymentIndex : CreationAuditedEntity<Guid>, IMustHaveCompany
    {
        public const int MaxNoteLength = Person.MaxNameLength + (Person.MaxSurnameLength * 2) + 3;

        [Required]
        [Range(1, Int32.MaxValue)]
        public virtual int Exp { get; set; }

        [Required]
        public virtual ContractType Contract { get; set; }

        [Required]
        public virtual ContractSubType Group { get; set; }

        [StringLength(MaxNoteLength)]
        public virtual string Note { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public EmploymentIndex(int companyId, ContractType contract, ContractSubType group, string note)
        {
            Contract = contract;
            Group = group;
            Note = note;
            CompanyId = companyId;
        }

        public EmploymentIndex(int companyId, ContractType contract, ContractSubType group, string note, int exp)
            :this(companyId, contract, group, note)
        {
            Exp = exp;
        }
    }
}
