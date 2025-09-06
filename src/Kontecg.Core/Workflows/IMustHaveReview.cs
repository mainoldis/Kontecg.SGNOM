using System;

namespace Kontecg.Workflows
{
    public interface IMustHaveReview
    {
        int DocumentDefinitionId { get; set; }

        public DateTime MadeOn { get; set; }

        public string Code { get; set; }

        ReviewStatus Review { get; set; }
    }
}
