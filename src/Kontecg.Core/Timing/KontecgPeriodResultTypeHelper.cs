using Kontecg.Dependency;
using Kontecg.Localization;
using Kontecg.MultiCompany;

namespace Kontecg.Timing
{
    public class KontecgPeriodResultTypeHelper : KontecgCoreServiceBase, ITransientDependency
    {
        public KontecgInvalidPeriodException CreateExceptionForFailedOperationAttempt(KontecgPeriodResult<Company> operationResult, string companyName = null)
        {
            switch (operationResult.Result)
            {
                case KontecgPeriodResultType.Success:
                    return new KontecgInvalidPeriodException("Don't call this method with a success result!");
                case KontecgPeriodResultType.InvalidCompanyName:
                    return new KontecgInvalidPeriodException(L("ThereIsNoCompanyDefinedWithName{0}", operationResult.Company?.Name ?? companyName));
                case KontecgPeriodResultType.CompanyIsNotActive:
                    return new KontecgInvalidPeriodException(L("CompanyIsNotActive", operationResult.Company?.Name ?? companyName));
                case KontecgPeriodResultType.InvalidReferenceGroup:
                    return new KontecgInvalidPeriodException(L("InvalidReferenceGroup", operationResult.Period.ReferenceGroup));
                case KontecgPeriodResultType.PendingOperations:
                    return new KontecgInvalidPeriodException(L("PendingOperations", operationResult.Period.ToString(), operationResult.Period.ReferenceGroup));
                case KontecgPeriodResultType.FailedForOtherReason:
                    return new KontecgInvalidPeriodException(operationResult.FailReason.Localize(LocalizationManager));
                default:
                    //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled operation fail reason: " + operationResult);
                    return new KontecgInvalidPeriodException(L("OperationFailed"));
            }
        }

        public string CreateLocalizedMessageForFailedOperationAttempt(KontecgPeriodResult<Company> operationResult, string companyName = null)
        {
            switch (operationResult.Result)
            {
                case KontecgPeriodResultType.Success:
                    throw new KontecgInvalidPeriodException("Don't call this method with a success result!");
                case KontecgPeriodResultType.InvalidCompanyName:
                    return L("ThereIsNoCompanyDefinedWithName{0}", operationResult.Company?.Name ?? companyName);
                case KontecgPeriodResultType.CompanyIsNotActive:
                    return L("CompanyIsNotActive", operationResult.Company?.Name ?? companyName);
                case KontecgPeriodResultType.InvalidReferenceGroup:
                    return L("InvalidReferenceGroup", operationResult.Period.ReferenceGroup);
                case KontecgPeriodResultType.PendingOperations:
                    return L("PendingOperations", operationResult.Period.ReferenceGroup);
                case KontecgPeriodResultType.FailedForOtherReason:
                    return operationResult.FailReason.Localize(LocalizationManager);
                default:
                    //Can not fall to default, actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled operation fail reason: " + operationResult);
                    return L("OperationFailed");
            }
        }
    }
}
