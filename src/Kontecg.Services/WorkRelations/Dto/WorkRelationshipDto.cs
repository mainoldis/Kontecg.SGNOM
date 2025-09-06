using Kontecg.HumanResources.Dto;
using Kontecg.MultiCompany.Dto;
using System;
using Kontecg.Currencies.Dtos;
using Kontecg.Organizations.Dto;

namespace Kontecg.WorkRelations.Dto
{
    public class WorkRelationshipDto
    {
        public CompanyInfoDto Company { get; set; }

        public PersonDto Person { get; set; }

        public int? Exp { get; set; }

        public int? CenterCost { get; set; }

        public string OccupationCode { get; set; }

        public string OccupationDescription { get; set; }

        public string OccupationResponsibility { get; set; }

        public char? OccupationCategory { get; set; }

        public string ComplexityGroup { get; set; }

        public string Code { get; set; }

        public DateTime? MadeOn { get; set; }

        public DateTime? EffectiveSince { get; set; }

        public DateTime? EffectiveUntil { get; set; }

        public string Contract { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public WorkPlaceUnitDto WorkPlaceUnit { get; set; }

        public string FirstLevelDisplayName { get; set; }

        public string SecondLevelDisplayName { get; set; }

        public string ThirdLevelDisplayName { get; set; }

        public string WorkShiftDisplayName { get; set; }

        public string WorkRegimenDisplayName { get; set; }

        public MoneyDto Salary { get; set; }

        public MoneyDto Plus { get; set; }

        public MoneyDto TotalSalary { get; set; }

        public decimal? RatePerHour { get; set; }

        public string EmployeeSalaryForm { get; set; }
    }
}
