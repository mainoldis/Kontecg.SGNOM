using Kontecg.Currencies.Dtos;

namespace Kontecg.WorkRelations.Dto
{
    public class EmploymentPlusInfoDto
    {
        public PlusDefinitionInfoDto PlusDefinition { get; set; }

        public MoneyDto Amount { get; set; }

        public decimal RatePerHour { get; set; }
    }
}
