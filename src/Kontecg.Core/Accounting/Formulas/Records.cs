using NMoneys;

namespace Kontecg.Accounting.Formulas
{
    public record AccountValue(int Account, int SubAccount, int SubControl, int Analysis, string Reference);

    public record ExpenseItemValue(int Code, string Reference);

    public record ClassifierValue(int Id, string Description);

    public record NoteValue(
        int ScopeId,
        int Account,
        int SubAccount,
        int SubControl,
        int Analysis,
        CurrencyIsoCode Currency,
        AccountOperation Operation)
    {
        public decimal Amount { get; set; }
    }
}