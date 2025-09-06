namespace Kontecg.Net.Mail
{
    public interface IEmailTemplateProvider
    {
        string GetDefaultTemplate(int? companyId);
    }
}
