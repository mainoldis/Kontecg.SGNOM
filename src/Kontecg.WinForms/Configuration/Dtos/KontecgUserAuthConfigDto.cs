using System.Collections.Generic;

namespace Kontecg.Configuration.Dtos
{
    public class KontecgUserAuthConfigDto
    {
        public Dictionary<string,string> AllPermissions { get; set; }

        public Dictionary<string, string> GrantedPermissions { get; set; }

    }
}
