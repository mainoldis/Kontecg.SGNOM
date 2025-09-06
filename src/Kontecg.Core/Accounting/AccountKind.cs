namespace Kontecg.Accounting
{
    /// <summary>
    /// Defines the classification types for accounting accounts based on their balance behavior
    /// and financial nature in double-entry bookkeeping systems.
    /// </summary>
    /// <remarks>
    /// This enumeration is fundamental to accounting principles and determines how account balances
    /// are calculated and displayed in financial statements. Each account kind has specific
    /// characteristics regarding normal balance direction and financial implications.
    /// </remarks>
    public enum AccountKind
    {
        /// <summary>
        /// Represents a credit account (Creditor Account) with right to collect and positive balance.
        /// </summary>
        /// <remarks>
        /// Credit accounts typically represent assets, expenses, and losses. They normally have
        /// positive balances and increase with debit entries and decrease with credit entries.
        /// Examples include cash, accounts receivable, and expense accounts.
        /// </remarks>
        Credit,

        /// <summary>
        /// Represents a debit account (Debtor Account) with payment obligation and negative balance.
        /// </summary>
        /// <remarks>
        /// Debit accounts typically represent liabilities, equity, and revenue. They normally have
        /// negative balances and increase with credit entries and decrease with debit entries.
        /// Examples include accounts payable, loans, and revenue accounts.
        /// </remarks>
        Debit,

        /// <summary>
        /// Represents a memo account (Temporary Account) used for transitional or temporary entries.
        /// </summary>
        /// <remarks>
        /// Memo accounts are used for temporary postings, adjustments, or entries that need to be
        /// cleared or transferred to permanent accounts. They are typically used in closing
        /// procedures or for temporary calculations and adjustments.
        /// </remarks>
        Memo
    }
}