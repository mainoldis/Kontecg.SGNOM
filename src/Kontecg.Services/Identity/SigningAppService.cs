using System.Threading.Tasks;
using Kontecg.Domain.Repositories;
using Kontecg.EntityHistory;
using Kontecg.Identity.Dto;

namespace Kontecg.Identity
{
    [UseCase(Description = "Servicio de gestión de firmas en documentos")]
    public class SigningAppService : KontecgAppServiceBase, ISigningAppService
    {
        private readonly IRepository<Sign> _signRepository;

        public SigningAppService(IRepository<Sign> signRepository)
        {
            _signRepository = signRepository;
        }

        public SignDto GetSignById(int id)
        {
            return ObjectMapper.Map<SignDto>(_signRepository.FirstOrDefault(s => s.Id == id));
        }

        public async Task<SignDto> GetSignByIdAsync(int id)
        {
            return ObjectMapper.Map<SignDto>(await _signRepository.FirstOrDefaultAsync(s => s.Id == id));
        }
    }
}
