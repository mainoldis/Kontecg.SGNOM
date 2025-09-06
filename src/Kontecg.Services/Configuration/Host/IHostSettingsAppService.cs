using System.Threading.Tasks;
using Kontecg.Application.Services;
using Kontecg.Configuration.Host.Dto;

namespace Kontecg.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettingsAsync();

        Task UpdateAllSettingsAsync(HostSettingsEditDto input);

        Task SendTestEmailAsync(SendTestEmailInput input);
    }
}
