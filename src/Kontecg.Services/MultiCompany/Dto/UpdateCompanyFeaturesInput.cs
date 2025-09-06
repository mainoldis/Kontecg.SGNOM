using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kontecg.Application.Services.Dto;

namespace Kontecg.MultiCompany.Dto
{
    public class UpdateCompanyFeaturesInput
    {
        [Range(1, int.MaxValue)] public int Id { get; set; }

        [Required] public List<NameValueDto> FeatureValues { get; set; }
    }
}
