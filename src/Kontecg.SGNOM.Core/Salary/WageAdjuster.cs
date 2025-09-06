using System;

namespace Kontecg.Salary
{
    [Flags]
    public enum WageAdjuster
    {
        None = 0,
        Salary = 1,
        Plus = 2,
        Bonus = 4,
        Holiday = 8,
        Subsidy = 16,
        
        Average = Salary | Plus | Holiday | Subsidy,
        All = Salary | Plus | Bonus | Holiday | Subsidy,
    }
}