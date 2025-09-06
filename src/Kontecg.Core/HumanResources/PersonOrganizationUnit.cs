using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Organizations;

namespace Kontecg.HumanResources
{
    [Table("organization_units_persons", Schema = "est")]
    public class PersonOrganizationUnit : CreationAuditedEntity<long>, IMayHaveCompany, IMustHaveOrganizationUnit
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PersonOrganizationUnit" /> class.
        /// </summary>
        public PersonOrganizationUnit()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PersonOrganizationUnit" /> class.
        /// </summary>
        /// <param name="companyId">Id of the <see cref="Company" /></param>
        /// <param name="personId">Id of the <see cref="Person" />.</param>
        /// <param name="organizationUnitId">Id of the <see cref="OrganizationUnit" />.</param>
        /// <param name="responsibilityLevel">Specifies if the <see cref="OrganizationUnit" /> is owned or not and his level.</param>
        public PersonOrganizationUnit(int? companyId, long personId, long organizationUnitId, int? responsibilityLevel = null)
        {
            CompanyId = companyId;
            PersonId = personId;
            OrganizationUnitId = organizationUnitId;
            ResponsibilityLevel = responsibilityLevel;
        }

        /// <summary>
        ///     Id of the Person.
        /// </summary>
        public virtual long PersonId { get; set; }

        /// <summary>
        ///     Id of the <see cref="OrganizationUnit" />.
        /// </summary>
        public virtual long OrganizationUnitId { get; set; }

        /// <summary>
        ///     CompanyId of this entity.
        /// </summary>
        public virtual int? CompanyId { get; set; }

        /// <summary>
        ///     Specifies if the organization is owned or not.
        /// </summary>
        public virtual int? ResponsibilityLevel { get; set; }
    }
}
