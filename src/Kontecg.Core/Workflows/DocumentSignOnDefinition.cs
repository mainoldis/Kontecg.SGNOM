using System.ComponentModel.DataAnnotations;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using Kontecg.MultiCompany;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontecg.Workflows
{
    [Table("document_sign_on_definitions", Schema = "gen")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class DocumentSignOnDefinition : AuditedEntity, IMustHaveCompany
    {
        public const int MaxCodeLength = 8;

        public const int MaxTagLength = 150;

        public virtual int DocumentDefinitionId { get; set; }

        [Required]
        [ForeignKey("DocumentDefinitionId")]
        public virtual DocumentDefinition DocumentDefinition { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        [StringLength(MaxTagLength)]
        public virtual string Tag { get; set; }

        public virtual int VisualOrder { get; set; }

        /// <inheritdoc />
        public virtual int CompanyId { get; set; }
    }
}