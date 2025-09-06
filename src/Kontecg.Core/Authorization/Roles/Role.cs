using System.ComponentModel.DataAnnotations;
using Kontecg.Authorization.Users;

namespace Kontecg.Authorization.Roles
{
    public class Role : KontecgRole<User>
    {
        public const int MaxDescriptionLength = 5000;

        public Role()
        {
        }

        public Role(int? companyId, string displayName)
            : base(companyId, displayName)
        {
        }

        public Role(int? companyId, string name, string displayName)
            : base(companyId, name, displayName)
        {
        }

        [StringLength(MaxDescriptionLength)] public string Description { get; set; }
    }
}
