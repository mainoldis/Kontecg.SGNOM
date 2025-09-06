using System;
using System.Linq;
using Kontecg.Configuration;
using Kontecg.Data;
using Kontecg.Dependency;
using Kontecg.Domain;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.Json;
using Kontecg.Linq.Extensions;
using Kontecg.Timing;
using NMoneys;
using NMoneys.Exchange;

namespace Kontecg.Currencies
{
    public class KontecgExchangeRateProvider : IKontecgExchangeRateProvider, ISingletonDependency
    {
        private readonly IExchangeRateFactory _factory;
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<ExchangeRateInfo> _repositoryExchangeRate;

        public KontecgExchangeRateProvider(
            IExchangeRateFactory factory,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<ExchangeRateInfo> repositoryExchangeRate, ISettingManager settingManager)
        {
            _factory = factory;
            _unitOfWorkManager = unitOfWorkManager;
            _repositoryExchangeRate = repositoryExchangeRate;
            _settingManager = settingManager;
        }

        public string Provider { get; set; } = KontecgCoreConsts.DefaultExchangeRateProvider;

        public DateTime? Since { get; set; }

        public DateTime? Until { get; set; }

        public ScopeData Scope { get; set; } = ScopeData.Company;

        public ExchangeRate Get(CurrencyIsoCode from, CurrencyIsoCode to)
        {
            return FindExchangeRate(Provider, from, to, Since, Until, Scope);
        }

        public bool TryGet(CurrencyIsoCode from, CurrencyIsoCode to, out ExchangeRate rate)
        {
            rate = FindExchangeRate(Provider, from, to, Since, Until, Scope);
            return true;
        }

        protected virtual ExchangeRate FindExchangeRate(string provider, CurrencyIsoCode from, CurrencyIsoCode to, DateTime? since, DateTime? until, ScopeData scope = ScopeData.Company)
        {
            if(!IsAllowedExchangeRate(from, to))
                throw new KontecgException("Requested exchange rate is not allowed!");

            if (scope == ScopeData.Company) scope = GetScopeToApply(from);

            var rate = _unitOfWorkManager.WithUnitOfWork((() => _repositoryExchangeRate.GetAllIncluding(ex => ex.Bank)
                .Where(p => p.Bank.Name == provider && p.Scope == scope && p.From == from && p.To == to)
                .WhereIf(since.HasValue, p => p.Since >= since)
                .WhereIf(until.HasValue, p => p.Until <= until)
                .OrderByDescending(o => o.Until)
                .FirstOrDefault()), new UnitOfWorkOptions() {IsTransactional = false});

            return rate != null
                ? _factory.CreateExchangeRate(provider, rate.From, rate.To, rate.Rate, rate.Since, rate.Until, rate.Scope)
                : _factory.CreateExchangeRate(provider, from, to, 1M, Clock.Now, Clock.Now);
        }

        private bool IsAllowedExchangeRate(CurrencyIsoCode from, CurrencyIsoCode to)
        {
            var currencies = _settingManager.GetSettingValue(AppSettings.CurrencyManagement.AllowedCurrencies).FromJsonString<string[]>();
            return currencies.Contains(from.ToString()) && currencies.Contains(to.ToString());
        } 

        private ScopeData GetScopeToApply(CurrencyIsoCode from)
        {
            var scoped = _settingManager.GetSettingValue(AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate + "." + from.ToString()) ??
                         _settingManager.GetSettingValue(AppSettings.CurrencyManagement.ScopeForCompanyOnExchangeRate);

            return scoped.To<ScopeData>();
        }
    }
}
