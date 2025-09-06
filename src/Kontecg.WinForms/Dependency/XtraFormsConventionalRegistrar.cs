using System.Reflection;
using Castle.MicroKernel.Registration;
using Kontecg.Views;

namespace Kontecg.Dependency
{
    /// <summary>
    ///     This class is used to register basic dependency implementations such as general forms />.
    /// </summary>
    public class XtraFormsConventionalRegistrar : IConventionalDependencyRegistrar
    {
        /// <inheritdoc />
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<BaseForm>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition && !type.IsAbstract)
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
            );

            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<BaseRibbonForm>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition && !type.IsAbstract)
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
            );

            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<BaseDirectXForm>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition && !type.IsAbstract)
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
            );

            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                       .IncludeNonPublicTypes()
                       .BasedOn<BaseUserControl>()
                       .If(type => !type.GetTypeInfo().IsGenericTypeDefinition && !type.IsAbstract)
                       .WithService.Self()
                       .WithService.DefaultInterfaces()
                       .LifestyleTransient()
            );

            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<BaseReport>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition && !type.IsAbstract)
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
            );
        }
    }
}
