using System.Linq;
using Kontecg.Currencies;
using Kontecg.Domain.Repositories;
using NMoneys;
using NMoneys.Allocations;
using NMoneys.Change;
using Shouldly;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class Money_Tests : SGNOMModuleTestBase
    {
        private readonly IKontecgExchangeRateProvider _exchangeRateProvider;
        private readonly IRepository<BillDenomination> _billDenominationRepository;

        public Money_Tests()
        {
            var admin = GetDefaultCompanyAdmin();
            KontecgSession.UserId = admin.Id;
            KontecgSession.CompanyId = admin.CompanyId;

            _exchangeRateProvider = LocalIocManager.Resolve<IKontecgExchangeRateProvider>();
            _billDenominationRepository = LocalIocManager.Resolve<IRepository<BillDenomination>>();
        }

        [Fact]
        public void Try_to_make_change_Test()
        {
            WithUnitOfWork(KontecgSession.CompanyId, () =>
            {
                decimal amount = 15435.56M;

                var denominations = _billDenominationRepository.GetAll()
                                                               .Where(b => b.Currency == CurrencyIsoCode.CUP && b.IsActive)
                                                               .Select(m => new Denomination(m.Bill)).ToArray();

                Money moneyBack = amount.Cup();
                OptimalChangeSolution change = moneyBack.MakeOptimalChange(denominations);
                change.ShouldNotBeNull();
            });
        }

        [Fact]
        public void Try_to_distribute_a_big_amount_Test()
        {
            WithUnitOfWork(KontecgSession.CompanyId, () =>
            {
                decimal amount = 100000M;
                Money moneyToDistribute = amount.Cup();

                var ratio1 = new Ratio(0.0377909534442471M);
                var ratio2 = new Ratio(0.0818517539012047M);
                var ratio3 = new Ratio(0.0618397536425012M);

                RatioCollection ratios = new RatioCollection(ratio1, ratio2, ratio2, ratio2, ratio2, ratio2, ratio2, ratio2, ratio2, ratio2, ratio2, ratio2, ratio3);

                var allocation = moneyToDistribute.Allocate(ratios);
            });
        }
    }
}