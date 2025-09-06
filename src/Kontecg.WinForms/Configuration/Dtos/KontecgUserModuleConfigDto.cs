using System.Collections.Generic;
using Kontecg.Views;

namespace Kontecg.Configuration.Dtos
{
    public class KontecgUserModuleConfigDto
    {
        public Dictionary<string, Module> Modules { get; set; }
    }
}