using Castle.Core.Logging;
using Kontecg.EFCore;

namespace Kontecg.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly ILogger _logger;

        public InitialHostDbBuilder(KontecgCoreDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Create()
        {
            new DefaultFeaturesBuilder(_context, _logger).Create();
            new DefaultLanguagesBuilder(_context, _logger).Create();
            new HostRoleAndUserBuilder(_context, _logger).Create();
            new DefaultSettingsBuilder(_context, _logger).Create();
            _context.SaveChanges();
        }
    }
}
