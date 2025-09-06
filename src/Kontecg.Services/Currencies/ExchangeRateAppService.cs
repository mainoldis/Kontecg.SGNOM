using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Application.Features;
using Kontecg.Authorization;
using Kontecg.Currencies.Dtos;
using Kontecg.Domain;
using Kontecg.Domain.Repositories;
using Kontecg.EntityHistory;
using Kontecg.Features;
using Kontecg.Threading;
using NMoneys;
using NMoneys.Change;

namespace Kontecg.Currencies
{
    [KontecgAuthorize]
    [UseCase(Description = "Servicio de intercambio monetario")]
    public class ExchangeRateAppService : KontecgAppServiceBase, IExchangeRateAppService
    {
        private readonly IKontecgExchangeRateProvider _exchangeRateProvider;
        private readonly IRepository<BillDenomination> _billDenominationRepository;

        public ExchangeRateAppService(
            IKontecgExchangeRateProvider exchangeRateProvider,
            IRepository<BillDenomination> billDenominationRepository)
        {
            _exchangeRateProvider = exchangeRateProvider;
            _billDenominationRepository = billDenominationRepository;
        }

        [RequiresFeature(CoreFeatureNames.CurrencyExchangeRateFeature)]
        public Task<MoneyDto> ExchangeWithAsync(ExchangeInputDto input)
        {
            var from = Enum.Parse<CurrencyIsoCode>(input.From);
            var to = Enum.Parse<CurrencyIsoCode>(input.To);

            _exchangeRateProvider.Provider = input.Provider;
            _exchangeRateProvider.Since = input.Since;
            _exchangeRateProvider.Until = input.Until;
            _exchangeRateProvider.Scope = input.Scope ?? ScopeData.Company;

            var exchangeRate = _exchangeRateProvider.Get(from, to);

            var result = exchangeRate.Apply(new Money(input.Amount, from));
            return Task.FromResult(ObjectMapper.Map<MoneyDto>(result));
        }

        [RequiresFeature(CoreFeatureNames.CurrencyExchangeRateFeature)]
        public MoneyDto ExchangeWith(ExchangeInputDto input)
        {
            return AsyncHelper.RunSync(() => ExchangeWithAsync(input));
        }

        public async Task<BillDto[]> GetDenominationOfAsync(ExchangeBillInputDto input)
        {
            using (CurrentUnitOfWork.SetCompanyId(null))
            {
                var parsed = Enum.TryParse(input.From, true, out CurrencyIsoCode currencyIsoCode);
                var denominations = (await _billDenominationRepository
                                           .GetAllListAsync(b =>
                                               b.Currency == (parsed ? currencyIsoCode : CurrencyIsoCode.CUP) &&
                                               b.IsActive).ConfigureAwait(false))
                                    .Select(m => new Denomination(m.Bill)).ToArray();

                Money moneyBack = new Money(input.Amount, parsed ? currencyIsoCode : CurrencyIsoCode.CUP);
                OptimalChangeSolution change = moneyBack.MakeOptimalChange(denominations);
                return change.Count == 0 ? [] : change.Select(c => new BillDto((int) c.Quantity, c.Denomination.Value, input.From)).ToArray();
            }
        }

        public BillDto[] GetDenominationOf(ExchangeBillInputDto input)
        {
            return AsyncHelper.RunSync(() => GetDenominationOfAsync(input));
        }
    }
}
