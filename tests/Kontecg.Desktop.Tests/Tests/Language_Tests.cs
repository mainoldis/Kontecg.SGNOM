using Kontecg.Application.Services.Dto;
using Kontecg.Localization;
using Kontecg.Localization.Dto;
using Kontecg.Localization.Sources;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Language_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public async void Get_all_languages_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var languageAppService = LocalIocManager.Resolve<ILanguageAppService>();

            var languages = await languageAppService.GetLanguages();
            languages.DefaultLanguageName.ShouldBe("es");
            languages.Items.ShouldNotBeEmpty();
        }

        [Fact]
        public async void Create_brazilian_language_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var languageAppService = LocalIocManager.Resolve<ILanguageAppService>();

            var languageForEdit = await languageAppService.GetLanguageForEdit(new NullableIdDto());

            languageForEdit.Language.Name = "pt-BR";

            await languageAppService.CreateOrUpdateLanguage(new CreateOrUpdateLanguageInput()
                {Language = languageForEdit.Language});
        }

        [Fact]
        public async void Get_language_texts_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var languageAppService = LocalIocManager.Resolve<ILanguageAppService>();
            var sources = LocalIocManager.Resolve<ILocalizationManager>().GetAllSources();

            foreach (ILocalizationSource localizationSource in sources)
            {
                var languageTexts = await languageAppService.GetLanguageTexts(new GetLanguageTextsInput() { SourceName = localizationSource.Name, TargetLanguageName = "pt-BR" });
            }
        }
    }
}
