using System.Threading.Tasks;

namespace Kontecg.Net.Mail
{
    public interface IEmailSettingsChecker
    {
        bool EmailSettingsValid();

        Task<bool> EmailSettingsValidAsync();
    }
}
