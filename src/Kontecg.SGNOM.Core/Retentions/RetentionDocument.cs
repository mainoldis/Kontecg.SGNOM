using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using Kontecg.Workflows;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using NMoneys;

namespace Kontecg.Retentions
{
    [Table("retention_documents", Schema = "ret")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class RetentionDocument : AuditedEntity<long>, IMustHaveCompany, IPassivable, IMustHaveReview
    {
        public const int MaxCodeLength = 20;

        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        public virtual DateTime MadeOn { get; set; }

        public virtual int RetentionDefinitionId { get; set; }

        [Required]
        [ForeignKey("RetentionDefinitionId")]
        public virtual RetentionDefinition RetentionDefinition { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        /// <inheritdoc />
        [Required]
        public virtual bool IsActive { get; set; } //Suspendido

        [Required]
        public virtual bool Finished { get; set; } //Terminado

        [Required]
        public virtual Money Amortization { get; set; } // Descuento acumulado

        [Required]
        public virtual int PaymentTerms { get; set; } //Número de plazos

        [Required]
        public virtual Money LastAmortization { get; set; } //Último descuento

        [Required]
        public virtual Money Surcharge { get; set; } //Recargo

        [Required]
        public virtual Money Total { get; set; } //Total a descontar

        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        [Required]
        public ReviewStatus Review { get; set; }
    }
}
