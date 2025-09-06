using Kontecg.Calendar.Dto;
using Kontecg.Currencies.Dtos;
using Kontecg.Domain.Entities;
using Kontecg.Organizations.Dto;

namespace Kontecg.WorkRelations.Dto
{
    public class InnerPartEmploymentDocumentDto : Entity<long>
    {
        public string EmployeeSalaryForm { get; set; }

        public WorkPlaceUnitDto WorkPlaceUnit { get; set; }

        public string WorkPlacePaymentCode { get; set; }

        public string FirstLevelDisplayName { get; set; }

        public string SecondLevelDisplayName { get; set; }

        public string ThirdLevelCode { get; set; }

        public string ThirdLevelDisplayName { get; set; }

        public int? CenterCost { get; set; }

        public string ComplexityGroup { get; set; }

        public string OccupationCode { get; set; }

        public string FullOccupationDescription { get; set; }

        public char OccupationCategory { get; set; }

        public WorkShiftDto WorkShift { get; set; }

        public MoneyDto Salary { get; set; }

        public PlusListInfoDto Plus { get; set; }

        public MoneyDto TotalSalary { get; set; }

        public decimal? RatePerHour { get; set; }
    }
}
