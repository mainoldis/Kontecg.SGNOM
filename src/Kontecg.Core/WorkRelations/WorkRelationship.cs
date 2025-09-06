using Kontecg.Organizations;
using NMoneys;
using System;
using Kontecg.HumanResources;
using Kontecg.MultiCompany;

namespace Kontecg.WorkRelations
{
    /// <summary>
    ///     This information are collected for all persons.
    /// </summary>
    public class WorkRelationship
    {
        /// <summary>
        ///     CompanyId.
        /// </summary>
        public int? CompanyId { get; set; }

        public Company Company { get; set; }

        /// <summary>
        ///     PersonId.
        /// </summary>
        public long PersonId { get; set; }

        public Person Person { get; set; }

        /// <summary>
        ///     Exp.
        /// </summary>
        public int? Exp { get; set; }

        /// <summary>
        ///     OrganizationUnitId.
        /// </summary>
        public long? OrganizationUnitId { get; set; }

        /// <summary>
        ///     CenterCost.
        /// </summary>
        public int? CenterCost { get; set; }

        /// <summary>
        ///     OccupationCode.
        /// </summary>
        public string OccupationCode { get; set; }

        /// <summary>
        ///     OccupationDescription.
        /// </summary>
        public string OccupationDescription { get; set; }

        public string OccupationResponsibility { get; set; }

        /// <summary>
        ///     OccupationCategory.
        /// </summary>
        public char? OccupationCategory { get; set; }

        /// <summary>
        ///     ComplexityGroup.
        /// </summary>
        public string ComplexityGroup { get; set; }

        /// <summary>
        ///     LastDocumentId.
        /// </summary>
        public long? LastDocumentId { get; set; }

        public string Code { get; set; }

        public DateTime? MadeOn { get; set; }

        public DateTime? EffectiveSince { get; set; }

        public DateTime? EffectiveUntil { get; set; }

        public string Contract { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public WorkPlaceUnit WorkPlaceUnit { get; set; }

        public string FirstLevelDisplayName { get; set; }

        public string SecondLevelDisplayName { get; set; }

        public string ThirdLevelDisplayName { get; set; }

        public string WorkShiftDisplayName { get; set; }

        public string WorkRegimenDisplayName { get; set; }

        public Money? Salary { get; set; }

        public Money? Plus { get; set; }

        public Money? TotalSalary { get; set; }

        public decimal? RatePerHour { get; set; }

        public string EmployeeSalaryForm { get; set; }
    }
}
