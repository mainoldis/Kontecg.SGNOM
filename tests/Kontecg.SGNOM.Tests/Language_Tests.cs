using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kontecg.Dto;
using Kontecg.Localization;
using Kontecg.Localization.Dto;
using Kontecg.Localization.Exporting;
using Kontecg.Localization.Sources;
using Kontecg.Storage;
using Shouldly;
using Xunit;

namespace Kontecg.SGNOM.Tests
{
    public class Language_Tests : SGNOMModuleTestBase
    {
        [Fact]
        public async void Get_all_languages_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(1, 2);
            var languageAppService = LocalIocManager.Resolve<ILanguageAppService>();

            var languages = await languageAppService.GetLanguages();
            languages.DefaultLanguageName.ShouldBe("es");
            languages.Items.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Get_language_texts_TestAsync()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var languageAppService = LocalIocManager.Resolve<ILanguageAppService>();
            var languageExporter = LocalIocManager.Resolve<ILanguageTextsExcelExporter>();
            var tempFileCacheManager = Resolve<ITempFileCacheManager>();
            var sources = LocalIocManager.Resolve<ILocalizationManager>().GetAllSources();

            var texts = new List<LanguageTextListDto>();

            foreach (ILocalizationSource localizationSource in sources)
                texts.AddRange((await languageAppService.GetLanguageTexts(new GetLanguageTextsInput()
                    {SourceName = localizationSource.Name, TargetLanguageName = "en"})).Items);

            FileDto exportToFileAsync = await languageExporter.ExportToFileAsync("languages", texts);

            await using var file = File.Create(exportToFileAsync.FileName);
            var buffer = tempFileCacheManager.GetFile(exportToFileAsync.FileToken);
            await file.WriteAsync(buffer, 0, buffer.Length);
            await file.FlushAsync();
        }
    }
}
