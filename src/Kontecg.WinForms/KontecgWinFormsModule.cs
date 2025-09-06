using System;
using System.Globalization;
using DevExpress.Utils.Localization;
using DevExpress.Utils.MVVM;
using Kontecg.Authorization;
using Kontecg.Configuration;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.EmbeddedResources;
using Kontecg.ExceptionHandling;
using Kontecg.Features;
using Kontecg.Localization;
using Kontecg.Modules;
using Kontecg.Navigation;
using Kontecg.Reflection.Extensions;
using Kontecg.Threading;
using Kontecg.Views;

namespace Kontecg
{
    [DependsOn(typeof(KontecgAppModule))]
    public class KontecgWinFormsModule : KontecgModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IKontecgWinFormsConfiguration, KontecgWinFormsConfiguration>();
            IocManager.RegisterIfNot<IKontecgExceptionHandlingDefaultOptions, KontecgExceptionHandlingDefaultOptions>();
            IocManager.AddConventionalRegistrar(new XtraFormsConventionalRegistrar());

            Configuration.ReplaceService<ICancellationTokenProvider, WinFormsCancellationTokenProvider>();
            Configuration.Modules.UseWinForms().Providers.Add<DefaultModuleRegistrationProvider>();

            //Adding localization sources
            KontecgWinFormsLocalizationConfigurer.Configure(Configuration.Localization);
            //Adding feature providers
            Configuration.Features.Providers.Add<WinFormsFeatureProvider>();
            //Adding setting providers
            Configuration.Settings.Providers.Add<WinFormsSettingProvider>();
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<WinFormsAuthorizationProvider>();
            //Adding EmbeddedResources from current assembly
            KontecgWinformsResourcesConfigurer.Configure(Configuration.EmbeddedResources);

            MVVMContextCompositionRoot.ViewModelCreate += MVVMContextCompositionRootOnViewModelCreate;
            MVVMContextCompositionRoot.ViewModelRelease += MVVMContextCompositionRootOnViewModelRelease;

            XtraLocalizer.QueryLocalizedString += XtraLocalizer_OnQueryLocalizedString;
            XtraLocalizer.QueryLocalizedStringNonTranslated += XtraLocalizer_OnQueryLocalizedStringNonTranslated;
            XtraLocalizer.QueryLocalizedStringContainerResource += XtraLocalizer_QueryLocalizedStringContainerResource;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgWinFormsModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ModuleManager>().Initialize();
        }

        /// <inheritdoc />
        public override void Shutdown()
        {
            XtraLocalizer.QueryLocalizedString -= XtraLocalizer_OnQueryLocalizedString;
            XtraLocalizer.QueryLocalizedStringNonTranslated -= XtraLocalizer_OnQueryLocalizedStringNonTranslated;
            XtraLocalizer.QueryLocalizedStringContainerResource -= XtraLocalizer_QueryLocalizedStringContainerResource;

            MVVMContextCompositionRoot.ViewModelCreate -= MVVMContextCompositionRootOnViewModelCreate;
            MVVMContextCompositionRoot.ViewModelRelease -= MVVMContextCompositionRootOnViewModelRelease;
        }

        private void MVVMContextCompositionRootOnViewModelCreate(object sender, ViewModelCreateEventArgs e)
        {
            try
            {
                IocManager.RegisterIfNot(e.ViewModelType, e.RuntimeViewModelType, DependencyLifeStyle.Transient);
                e.ViewModel = IocManager.Resolve(e.ViewModelType);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);

                var viewModel = e.ViewModelSource.Create(e.ViewModelType, e.ConstructorParameters);
                e.ViewModel = viewModel;
            }
        }

        private void MVVMContextCompositionRootOnViewModelRelease(object sender, ViewModelReleaseEventArgs e)
        {
            try
            {
                IocManager.Release(e.ViewModel);
            }
            catch
            {
            }
        }

        private void XtraLocalizer_OnQueryLocalizedString(object sender, XtraLocalizer.QueryLocalizedStringEventArgs e)
        {
            e.Value = LocalizationHelper.GetString(KontecgWinFormsConsts.LocalizationSourceName, e.StringID.ToString(),
                CultureHelper.GetCultureInfoByChecking(e.Culture));
        }

        private void XtraLocalizer_OnQueryLocalizedStringNonTranslated(object sender, XtraLocalizer.QueryLocalizedStringEventArgs e)
        {
        }

        private void XtraLocalizer_QueryLocalizedStringContainerResource(object sender, XtraLocalizer.QueryLocalizedStringEventArgs e)
        {
        }
    }
}
