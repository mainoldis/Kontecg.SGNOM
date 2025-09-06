using System;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Salary;

namespace Kontecg.WorkRelations
{
    [Table("employment_documents_to_generate", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class EmploymentDocumentToGenerate : AuditedEntity<long>, IMustHaveCompany
    {
        public virtual long EmploymentDocumentId { get; set; }

        [ForeignKey("EmploymentDocumentId")]
        public virtual EmploymentDocument EmploymentDocument { get; set; }

        public virtual long? NextEmploymentDocumentId { get; set; }

        [ForeignKey("NextEmploymentDocumentId")]
        public virtual EmploymentDocument NextEmploymentDocument { get; set; }

        public virtual EmploymentDocumentLegalChangeType LegalChangeType { get; set; }

        public virtual DateTime? EffectiveSince { get; set; }

        public virtual DateTime? ExpirationDate { get; set; }

        public virtual EmployeeSalaryForm? EmployeeSalaryForm { get; set; }

        public virtual EmploymentType? Type { get; set; }

        public virtual EmploymentSubType? SubType { get; set; }

        public virtual long? OrganizationUnitId { get; set; }

        public virtual int? CenterCost { get; set; }

        public virtual string OccupationCode { get; set; }

        public virtual int? WorkShiftId { get; set; }

        public virtual int? SummaryId { get; set; }

        public virtual string ExtraSummary { get; set; }

        public virtual bool Confirmed { get; set; }

        /// <inheritdoc />
        public virtual int CompanyId { get; set; }

        /// <inheritdoc />
        public EmploymentDocumentToGenerate(long employmentDocumentId, EmploymentDocumentLegalChangeType legalChangeType = EmploymentDocumentLegalChangeType.AllWithoutEmployeeSalaryForm)
        {
            EmploymentDocumentId = employmentDocumentId;
            LegalChangeType = legalChangeType;
            Confirmed = false;
        }

        /// <inheritdoc />
        public EmploymentDocumentToGenerate(EmploymentDocument employmentDocument, EmploymentDocumentLegalChangeType legalChangeType = EmploymentDocumentLegalChangeType.AllWithoutEmployeeSalaryForm)
        {
            EmploymentDocument = employmentDocument;
            LegalChangeType = legalChangeType;
            Confirmed = false;
        }
    }
}