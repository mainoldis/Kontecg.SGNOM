using System;

namespace Kontecg
{
    public class PersonUpdatedMessage
    {
        public PersonUpdatedMessage(Guid correlationId, string identityCard)
        {
            CorrelationId = correlationId;
            IdentityCard = identityCard;
        }

        public Guid CorrelationId { get; }

        public string IdentityCard { get; }
    }
}
