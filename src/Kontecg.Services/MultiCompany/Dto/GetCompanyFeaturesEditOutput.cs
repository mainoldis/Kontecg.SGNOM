using System.Collections.Generic;
using Kontecg.Application.Services.Dto;
using Kontecg.Features.Dto;

namespace Kontecg.MultiCompany.Dto
{
    public class GetCompanyFeaturesEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}
