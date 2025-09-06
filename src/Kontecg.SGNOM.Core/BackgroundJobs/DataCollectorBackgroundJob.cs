using System.Threading.Tasks;
using Kontecg.HumanResources;

namespace Kontecg.BackgroundJobs
{
    public class DataCollectorBackgroundJob : IAsyncBackgroundJob<Person>
    {
        public async Task ExecuteAsync(Person args)
        {
            throw new System.NotImplementedException();
        }
    }
}
