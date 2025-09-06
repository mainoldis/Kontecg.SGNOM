using System;
using Kontecg.Application.Services.Dto;
using Kontecg.Calendar.Dto;
using Kontecg.Currencies.Dtos;
using Kontecg.HumanResources.Dto;
using Kontecg.Timing.Dto;

namespace Kontecg.WorkRelations.Dto
{
    public class EmploymentDocumentInfoDto : EntityDto<long>
    {
        public string CompanyName { get; set; }

        public string Organism { get; set; }

        public PersonDto Person { get; set; }

        public int Exp { get; set; }

        public long OrganizationUnitId { get; set; }

        public int CenterCost { get; set; }

        public string OccupationCode { get; set; }

        public char OccupationCategory { get; set; }

        public string ComplexityGroup { get; set; }

        public string FullOccupationDescription { get; set; }

        public string Code { get; set; }

        public DateTime MadeOn { get; set; }

        public DateTime EffectiveSince { get; set; }

        public DateTime EffectiveUntil { get; set; }

        public string Contract { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public string WorkPlacePaymentCode { get; set; }

        public string WorkPlacePaymentDescription { get; set; }

        public string FirstLevelCode { get; set; }

        public string FirstLevelDisplayName { get; set; }

        public string SecondLevelCode { get; set; }

        public string SecondLevelDisplayName { get; set; }

        public string ThirdLevelCode { get; set; }

        public string ThirdLevelDisplayName { get; set; }

        public WorkShiftDto WorkShift { get; set; }

        public MoneyDto Salary { get; set; }

        public PlusListInfoDto Plus { get; set; }

        public MoneyDto TotalSalary { get; set; }

        public decimal RatePerHour { get; set; }

        public string EmployeeSalaryForm { get; set; }

        public string DisplaySummary { get; set; }

        public EmploymentDocumentInfoDto Previous { get; set; }

        public bool ByAssignment { get; set; }

        public bool ByOfficial { get; set; }

        public PeriodDto Period { get; set; }

        public string Review { get; set; }
    }
}
