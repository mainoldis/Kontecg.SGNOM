using Kontecg.Currencies;
using Kontecg.Currencies.Dtos;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Currencies_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public void Money_operations_Test()
        {
            var use = KontecgSession.Use(1, 2);
            var exchangeService = LocalIocManager.Resolve<IExchangeRateAppService>();

            var moneyDto = exchangeService.ExchangeWith(new ExchangeInputDto {Amount = 100, From = "USD", To = "CUP"});
            moneyDto.Amount.ShouldBe(12000);
        }

        [Fact]
        public void DenominationOf_Test()
        {
            var use = KontecgSession.Use(1, 2);
            var exchangeService = LocalIocManager.Resolve<IExchangeRateAppService>();

            var billDto = exchangeService.GetDenominationOf(new ExchangeBillInputDto { Amount = 52626.25M, From = "CUP" });
            billDto.ShouldNotBeEmpty();
        }
    }
}
