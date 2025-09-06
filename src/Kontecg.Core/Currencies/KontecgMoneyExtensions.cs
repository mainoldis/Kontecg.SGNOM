using NMoneys;

namespace Kontecg.Currencies
{
    /// <summary>
    ///     Extensions methods related to monetary quantities.
    /// </summary>
    public static class KontecgMoneyExtensions
    {
        /// <summary>
        ///     Creates an <see cref="Money" /> instance with the specified Currency.
        /// </summary>
        /// <param name="amount">The <see cref="Money.Amount" /> of the monetary quantity.</param>
        /// <returns>A <see cref="Money" /> with the specified <paramref name="amount" /> and <see cref="CurrencyIsoCode.CUP" />.</returns>
        public static Money Cup(this decimal amount)
        {
            return new(amount, CurrencyIsoCode.CUP);
        }
    }
}
