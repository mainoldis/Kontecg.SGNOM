using Kontecg.Domain.Entities.Auditing;
using Kontecg.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontecg.Organizations
{
    [Table("organization_units_center_costs", Schema = "est")]
    public class WorkPlaceUnitCenterCost : CreationAuditedEntity<long>, IMayHaveCompany, IMustHaveOrganizationUnit
    {
        /// <summary>
        ///     Id of the <see cref="OrganizationUnit" />.
        /// </summary>
        public virtual long OrganizationUnitId { get; set; }

        /// <summary>
        ///     CompanyId of this entity.
        /// </summary>
        public virtual int? CompanyId { get; set; }

        public virtual int CenterCostId { get; set; }
    }
}