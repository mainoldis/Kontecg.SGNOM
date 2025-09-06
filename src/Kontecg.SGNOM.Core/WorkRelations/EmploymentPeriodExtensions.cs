using Kontecg.Timing;

namespace Kontecg.WorkRelations
{
    public static class EmploymentPeriodExtensions
    {
        public static WorkingHours ToWorkingPeriod(this EmploymentDocument employment)
        {
            return new WorkingHours(employment.EffectiveSince, employment.EffectiveUntil);
        }
    }
}
