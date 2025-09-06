namespace Kontecg.RealTime
{
    public interface IOnlineClientInfoProvider
    {
        IOnlineClient CreateClientForCurrentConnection();
    }
}
