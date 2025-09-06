using Kontecg.Sessions.Dto;
using System.Threading.Tasks;

namespace Kontecg.Runtime.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationOutput> GetCurrentLoginInformationAsync();
    }
}
