using Kontecg.Application.Services.Dto;

namespace Kontecg.Authorization.Users.Dto
{
    public class LinkedUserDto : EntityDto<long>
    {
        public int? CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string Username { get; set; }

        public object GetShownLoginName(bool multiCompanyEnabled)
        {
            if (!multiCompanyEnabled)
            {
                return Username;
            }

            return string.IsNullOrEmpty(CompanyName)
                ? ".\\" + Username
                : CompanyName + "\\" + Username;
        }
    }
}
