using System.Threading.Tasks;
using Castle.Core.Logging;
using MassTransit;

namespace Kontecg
{
    public class PersonUpdatedConsumer : IConsumer<PersonUpdatedMessage>
    {
        private readonly ILogger _logger;

        public PersonUpdatedConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PersonUpdatedMessage> context)
        {
            _logger.Info($"Received a message with CorrelationId '{context.Message.CorrelationId}' corresponding to person identified by '{context.Message.IdentityCard}'");

            await Task.CompletedTask;
        }
    }
}
