namespace Kontecg.Timing
{
    public enum KontecgPeriodResultType : byte
    {
        Success = 1,

        InvalidCompanyName,

        CompanyIsNotActive,

        InvalidReferenceGroup,

        PendingOperations,

        FailedForOtherReason
    }
}
