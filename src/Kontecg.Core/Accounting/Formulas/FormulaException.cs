using System;

namespace Kontecg.Accounting.Formulas
{
    public class FormulaException : KontecgException
    {
        public FormulaException(string message, Exception inner = null)
            : base(message, inner) { }
    }
}