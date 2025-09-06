namespace Kontecg.Configuration.Dtos
{
    public class KontecgMultiCompanyConfigDto
    {
        public bool IsEnabled { get; set; }

        public bool IgnoreFeatureCheckForHostUsers { get; set; }

        public KontecgMultiCompanySidesConfigDto Sides { get; private set; }

        public KontecgMultiCompanyConfigDto()
        {
            Sides = new KontecgMultiCompanySidesConfigDto();
        }
    }
}
