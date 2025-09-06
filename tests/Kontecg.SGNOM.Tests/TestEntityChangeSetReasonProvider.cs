using JetBrains.Annotations;
using Kontecg.Dependency;
using Kontecg.EntityHistory;
using Kontecg.Runtime;

namespace Kontecg.SGNOM.Tests
{
    public class TestEntityChangeSetReasonProvider : EntityChangeSetReasonProviderBase, ISingletonDependency
    {
        /// <inheritdoc />
        public TestEntityChangeSetReasonProvider(IAmbientScopeProvider<ReasonOverride> reasonOverrideScopeProvider) :
            base(reasonOverrideScopeProvider)
        {
        }

        /// <inheritdoc />
        [CanBeNull]
        public override string Reason => OverridedValue != null ? OverridedValue.Reason : "Testing";
    }
}