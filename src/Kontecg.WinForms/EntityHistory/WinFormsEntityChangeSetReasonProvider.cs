using System.Diagnostics;
using JetBrains.Annotations;
using Kontecg.Dependency;
using Kontecg.Runtime;

namespace Kontecg.EntityHistory
{
    /// <summary>
    /// Implements <see cref="IEntityChangeSetReasonProvider"/> to get reason.
    /// </summary>
    [DebuggerStepThrough]
    public class WinFormsEntityChangeSetReasonProvider : EntityChangeSetReasonProviderBase, ISingletonDependency
    {
        /// <inheritdoc />
        public WinFormsEntityChangeSetReasonProvider(IAmbientScopeProvider<ReasonOverride> reasonOverrideScopeProvider) :
            base(reasonOverrideScopeProvider)
        {
        }

        /// <inheritdoc />
        [CanBeNull]
        public override string Reason
        {
            get
            {
                if(OverridedValue != null)
                    return OverridedValue.Reason;

                try
                {
                    var hasCacheKey = WinFormsRuntimeContext.Items.TryGetValue("__CurrentWindowOrForm", out object? value);
                    if (!hasCacheKey)
                        return "KONTECG";

                    var cachedValue = value as string;
                    return cachedValue ?? "KONTECG";
                }
                catch (KontecgException ex)
                {
                    Logger.Warn("WinFormsRuntimeContext access when it is not suppose to", ex);
                    return null;
                }
            }
        }
    }
}
