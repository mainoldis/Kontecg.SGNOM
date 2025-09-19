using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Logging;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FluentStorage.Utils.Extensions;
using Kontecg.BlobStoring;
using Kontecg.Configuration;
using Kontecg.Domain;
using Kontecg.Storage.Blobs;
using Kontecg.Extensions;
using NMoneys;

namespace Kontecg.Currencies
{
    public class ExternalExchangeRateProviderWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day
        private readonly IRepository<ExchangeRateInfo> _exchangeRateRepository;
        private readonly IRepository<Bank> _bankRepository;
        private readonly IBlobContainer<AccountingContainer> _blobContainer;
        public const string Historigrama = "https://www.bc.gob.cu/historigrama/138";

        public ExternalExchangeRateProviderWorker(KontecgAsyncTimer timer,
            IBlobContainer<AccountingContainer> container,
            IRepository<ExchangeRateInfo> exchangeRateRepository,
            IRepository<Bank> bankRepository)
            : base(timer)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _blobContainer = container;
            _bankRepository = bankRepository;
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        protected override async Task DoWorkAsync()
        {
            try
            {
                using var uow = UnitOfWorkManager.Begin();
                using (CurrentUnitOfWork.SetCompanyId(null))
                {
                    var baseCurrency = await GetBaseCurrencyAsync();
                    var banks = await _bankRepository.GetAllListAsync();
                    foreach (var bank in banks)
                    {
                        var name = $"{AccountingContainer.ExchangeRates}\\{bank.Name}\\archivo.xml";
                        if (await _blobContainer.ExistsAsync(name))
                        {
                            var stream = (await _blobContainer.GetAsync(name)).ToString(Encoding.UTF8);
                            if (stream != null && !stream.IsNullOrEmpty())
                            {
                                var doc = XDocument.Parse(stream, LoadOptions.None);
                                var fecha = doc.Descendants("FECHA").Select(f =>
                                    new Fecha(
                                        ExtractDate(f.Attribute("DESDE").Value),
                                        ExtractDate(f.Attribute("HASTA").Value)))
                                    .FirstOrDefault();

                                var tcs = doc.Descendants("TC").Select(tc =>
                                                 new TasaCambio(
                                                     Enum.Parse<CurrencyIsoCode>(tc.Attribute("SIG_MONEDA").Value),
                                                     tc.Attribute("NOM_MONEDA").Value,
                                                     decimal.Parse(tc.Attribute("TC_1").Value, CultureInfo.InvariantCulture),
                                                     decimal.Parse(tc.Attribute("TC_2").Value, CultureInfo.InvariantCulture)
                                                 ))
                                             .ToList();

                                var exchangeRateInfos = (await _exchangeRateRepository.GetAllIncludingAsync(e => e.Bank))
                                    .Where(e => e.Bank.Name == bank.Name && e.Since == fecha.Desde && e.Until == fecha.Hasta).ToList();

                                foreach (var tasaCambio in tcs)
                                {
                                    if (!exchangeRateInfos.Exists(t => t.From == tasaCambio.CurrencyIsoCode))
                                    {
                                        await _exchangeRateRepository.InsertAsync(new ExchangeRateInfo(bank.Id,
                                            tasaCambio.CurrencyIsoCode, baseCurrency, tasaCambio.Tasa1, fecha.Desde,
                                            fecha.Hasta, ScopeData.Company));

                                        await _exchangeRateRepository.InsertAsync(new ExchangeRateInfo(bank.Id,
                                            tasaCambio.CurrencyIsoCode, baseCurrency, tasaCambio.Tasa2, fecha.Desde,
                                            fecha.Hasta, ScopeData.Personal));
                                    }
                                }
                            }
                        }
                    }
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                await uow.CompleteAsync();
            }
            catch (Exception e)
            {
                Logger.Log(LogSeverity.Error, "An error occurred while getting exchange rates on host database", e);
            }
        }

        private async Task<CurrencyIsoCode> GetBaseCurrencyAsync()
        {
            string currency = await SettingManager.GetSettingValueAsync(AppSettings.CurrencyManagement.BaseCurrency);
            return Enum.Parse<CurrencyIsoCode>(currency);
        }

        private DateTime ExtractDate(string toParse)
        {
            return DateTime.ParseExact(toParse, "yyyyMMdd", new DateTimeFormatInfo());
        }

        public record Fecha(DateTime Desde, DateTime Hasta);
        public record TasaCambio(CurrencyIsoCode CurrencyIsoCode, string Nombre, decimal Tasa1, decimal Tasa2);
    }
}
