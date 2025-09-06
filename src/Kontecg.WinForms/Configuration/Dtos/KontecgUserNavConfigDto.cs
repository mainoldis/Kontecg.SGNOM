using System.Collections.Generic;
using Kontecg.Application.Navigation;

namespace Kontecg.Configuration.Dtos
{
    public class KontecgUserNavConfigDto
    {
        public Dictionary<string, UserMenu> Menus { get; set; }
    }
}
