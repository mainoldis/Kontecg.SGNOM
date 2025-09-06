namespace Kontecg.Services
{
    public interface IReportLocator
    {
        object GetReport(object reportKey);

        void ReleaseReport(object reportKey);
    }
}