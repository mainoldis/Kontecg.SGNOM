using Kontecg.Sessions.Dto;
using Kontecg.Sessions;
using System.Threading.Tasks;
using Kontecg.Dependency;

namespace Kontecg.Runtime.Session
{
    internal class PerRequestSessionCache : IPerRequestSessionCache, ITransientDependency
    {
        private readonly ISessionAppService _sessionAppService;

        public PerRequestSessionCache(ISessionAppService sessionAppService)
        {
            _sessionAppService = sessionAppService;
        }

        public async Task<GetCurrentLoginInformationOutput> GetCurrentLoginInformationAsync()
        {
            var hasCacheKey = WinFormsRuntimeContext.Items.TryGetValue("__PerRequestSessionCache", out object? value);
            if (!hasCacheKey)
            {
                return await _sessionAppService.GetCurrentLoginInformationAsync();
            }

            var cachedValue = value as GetCurrentLoginInformationOutput;
            if (cachedValue == null)
            {
                cachedValue = await _sessionAppService.GetCurrentLoginInformationAsync();
                WinFormsRuntimeContext.Items["__PerRequestSessionCache"] = cachedValue;
            }

            return cachedValue;
        }
    }
}
