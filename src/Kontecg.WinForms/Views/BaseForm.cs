using System;
using System.Globalization;
using Castle.Core.Logging;
using DevExpress.XtraEditors;
using Kontecg.Events.Bus;
using Kontecg.Extensions;
using Kontecg.Localization;
using Kontecg.Localization.Sources;
using Kontecg.Runtime;

namespace Kontecg.Views
{
    /// <summary>Represents a window or dialog box that makes up an application's user interface.</summary>
    public partial class BaseForm : XtraForm
    {
        private const string TitleId = "_Title";

        private ILocalizationSource _localizationSource;

        public BaseForm()
        {
            InitializeComponent();
            if (DesignMode) return;
            LocalizationSourceName = KontecgWinFormsConsts.LocalizationSourceName;
            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        /// <inheritdoc />
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                ScreenManager.SetFormToCurrentScreen(this);
                LocalizeIsolatedItems();
            }
            catch (Exception ex)
            {
                Logger?.Warn("Can´t setup localizable components!", ex);
            }
        }

        protected override FormShowMode ShowMode => FormShowMode.AfterInitialization;

        #region Localization

        protected virtual void LocalizeIsolatedItems()
        {
            Text = !Text.IsNullOrEmpty() ? $@"KONTECG - {L(Text.Replace(' ', '_') + TitleId)}" : "KONTECG";
        }

        /// <summary>
        ///     Gets/sets name of the localization source that is used in this application service.
        ///     It must be set in order to use <see cref="L(string)" /> and <see cref="L(string,CultureInfo)" /> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        ///     Gets localization source.
        ///     It's valid if <see cref="LocalizationSourceName" /> is set.
        /// </summary>
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (DesignMode)
                    LocalizationSourceName = KontecgWinFormsConsts.LocalizationSourceName;

                if (LocalizationSourceName == null && !DesignMode)
                    throw new KontecgException(
                        "Must set LocalizationSourceName before, in order to get LocalizationSource");

                if (_localizationSource == null || _localizationSource.Name != LocalizationSourceName)
                    _localizationSource = LocalizationManager.GetSource(LocalizationSourceName);

                return _localizationSource;
            }
        }

        /// <summary>
        ///     Reference to the localization manager.
        /// </summary>
        public ILocalizationManager LocalizationManager { protected get; set; }

        /// <summary>
        ///     Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationSource?.GetString(name);
        }

        /// <summary>
        ///     Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, params object[] args)
        {
            return LocalizationSource?.GetString(name, args);
        }

        /// <summary>
        ///     Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationSource?.GetString(name, culture);
        }

        /// <summary>
        ///     Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource?.GetString(name, culture, args);
        }

        #endregion

        /// <summary>
        ///     Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        ///     Reference to the event bus.
        /// </summary>
        public IEventBus EventBus { get; set; }
    }
}
