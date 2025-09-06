using Kontecg.Localization;
using Kontecg.MultiCompany;
using System;

namespace Kontecg.Timing
{
    public class KontecgPeriodResult<TCompany>
        where TCompany : KontecgCompanyBase
    {
        public KontecgPeriodResult(KontecgPeriodResultType result, TCompany company = null, PeriodInfo period = null)
        {
            Result = result;
            Company = company;
            Period = period;
        }

        public KontecgPeriodResult(TCompany company, PeriodInfo period)
            :this(KontecgPeriodResultType.Success, company, period)
        {
        }

        public KontecgPeriodResultType Result { get; }

        public PeriodInfo Period { get; }

        public ILocalizableString FailReason { get; private set; }

        public TCompany Company { get; }

        /// <summary>
        ///     This method can be used only when <see cref="Result" /> is
        ///     <see cref="KontecgPeriodResultType.FailedForOtherReason" />.
        /// </summary>
        /// <param name="failReason">Localizable fail reason message</param>
        public void SetFailReason(ILocalizableString failReason)
        {
            if (Result != KontecgPeriodResultType.FailedForOtherReason)
            {
                throw new Exception(
                    $"Can not set fail reason for result type {Result}, use this method only for KontecgPeriodResultType.FailedForOtherReason result type!");
            }

            FailReason = failReason;
        }
    }
}
