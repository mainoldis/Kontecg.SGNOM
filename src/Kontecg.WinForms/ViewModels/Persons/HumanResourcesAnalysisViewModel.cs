using Kontecg.HumanResources;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using Kontecg.Domain.Uow;

namespace Kontecg.ViewModels.Persons
{
    [POCOViewModel]
    public class HumanResourcesAnalysisViewModel : DocumentContentViewModelBase
    {
        private readonly IHumanResourcesAppService _humanResourcesAppService;

        /// <inheritdoc />
        public HumanResourcesAnalysisViewModel(IHumanResourcesAppService humanResourcesAppService)
        {
            _humanResourcesAppService = humanResourcesAppService;
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        public IList<Item> GetOldPeople()
        {
            var femaleString = L("Gender.F"); 
            var oldPeople = UnitOfWorkManager.WithUnitOfWork(() => _humanResourcesAppService.GetAgedPeople().Items);
            var groupBy = oldPeople.GroupBy(k => GetRangeForAge(k.Age))
                                   .Select(x => new Item{ Range = x.Key, Count = x.Count(), Female = x.Count(s => s.Gender == femaleString) });
            return groupBy.ToReadOnlyCollection();
        }
        
        public class Item
        {
            public string Range { get; set; }

            public int Count { get; set; }

            public int Female { get; set; }
        }

        private string GetRangeForAge(int age)
        {
            return age switch
                   {
                       >= 0 and <= 25 => "<=25",
                       > 25 and <= 35 => "26-35",
                       > 35 and <= 45 => "36-45",
                       > 45 and <= 50 => "46-50",
                       > 50 and <= 55 => "51-55",
                       > 55 and <= 65 => "56-65",
                       _ => ">65"
                   };
        }
    }
}