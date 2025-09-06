using System.Collections.ObjectModel;

namespace Kontecg.Features.Dto
{
    //Mapped in CustomDtoMapper
    public class LocalizableComboboxItemSourceDto
    {
        public Collection<LocalizableComboboxItemDto> Items { get; set; }
    }
}
