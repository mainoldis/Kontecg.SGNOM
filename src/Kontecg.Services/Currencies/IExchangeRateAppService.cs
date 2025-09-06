using System.Threading.Tasks;
using Kontecg.Currencies.Dtos;

namespace Kontecg.Currencies
{
    public interface IExchangeRateAppService
    {
        Task<MoneyDto> ExchangeWithAsync(ExchangeInputDto input);

        MoneyDto ExchangeWith(ExchangeInputDto input);

        Task<BillDto[]> GetDenominationOfAsync(ExchangeBillInputDto input);

        BillDto[] GetDenominationOf(ExchangeBillInputDto input);
    }
}
