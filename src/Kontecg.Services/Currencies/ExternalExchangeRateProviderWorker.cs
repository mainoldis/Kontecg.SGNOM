using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Logging;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;
using Kontecg.Timing;
using System;
using Kontecg.Net.Http;
using System.Threading.Tasks;
using Kontecg.Extensions;

namespace Kontecg.Currencies
{
    public class ExternalExchangeRateProviderWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly TimeSpan _outdatedTime = TimeSpan.FromDays(1);
        private readonly IRepository<ExchangeRateInfo> _exchangeRateRepository;
        private readonly IContentFolders _contentFolders;
        private readonly IPreconfiguredHttpClientProvider _preconfiguredHttpClientProvider;

        private string _tempFolder;
        private DateTime _lastCheckOnRuntime;

        public const string Historigrama = "https://www.bc.gob.cu/historigrama/138";

        public ExternalExchangeRateProviderWorker(KontecgAsyncTimer timer,
            IContentFolders contentFolders,
            IRepository<ExchangeRateInfo> exchangeRateRepository,
            IPreconfiguredHttpClientProvider preconfiguredHttpClientProvider)
            : base(timer)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _contentFolders = contentFolders;
            _preconfiguredHttpClientProvider = preconfiguredHttpClientProvider;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;

            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        protected override async Task DoWorkAsync()
        {
            try
            {
                if (_tempFolder.IsNullOrWhiteSpace()) _tempFolder = _contentFolders.TempFolder;

                var expireDate = Clock.Now - _outdatedTime;

       //         var xml = @"
       //<TC002011.23>
       //    <FECHA DESDE=""20231120"" HASTA=""20231121""/>
       //    <TC SIG_MONEDA=""AUD"" NOM_MONEDA=""DOLAR AUSTRALIANO"" TC_1=""15.73920"" TC_2=""78.69600""/>
       //    <TC SIG_MONEDA=""MXN"" NOM_MONEDA=""NUEVO PESO MEXICANO"" TC_1=""1.40312"" TC_2=""7.01562""/>
       //</TC002011.23>";

       //         var doc = XDocument.Parse(xml);

       //         var tcs = doc.Descendants("TC").Select(tc => new TC
       //         {
       //             SIG_MONEDA = tc.Attribute("SIG_MONEDA").Value,
       //             NOM_MONEDA = tc.Attribute("NOM_MONEDA").Value,
       //             TC_1 = tc.Attribute("TC_1").Value,
       //             TC_2 = tc.Attribute("TC_2").Value
       //         }).ToList();

                //using var uow = UnitOfWorkManager.Begin();
                //using (CurrentUnitOfWork.SetCompanyId(null))
                //{
                //    using (CurrentUnitOfWork.DisableFilter(KontecgDataFilters.MayHaveCompany))
                //    {
                //        //Code goes here
                //        Logger.Log(LogSeverity.Info, "Just for testing for now");
                //        uow.Complete();
                //    }
                //}

                _lastCheckOnRuntime = Clock.Now;
            }
            catch (Exception e)
            {
                Logger.Log(LogSeverity.Error, "An error occurred while getting exchange rates on host database", e);
            }
        }
    }
}
