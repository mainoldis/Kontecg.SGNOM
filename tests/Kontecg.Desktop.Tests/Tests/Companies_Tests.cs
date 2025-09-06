using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Kontecg.Application.Services.Dto;
using Kontecg.Configuration;
using Kontecg.Extensions;
using Kontecg.HumanResources;
using Kontecg.MimeTypes;
using Kontecg.MultiCompany;
using Kontecg.MultiCompany.Dto;
using Kontecg.Notifications;
using Kontecg.Notifications.Dto;
using Kontecg.Storage;
using Kontecg.Text;
using Shouldly;
using Xunit;
using static Kontecg.Text.FormattedStringValueExtracter;

namespace Kontecg.Desktop.Tests
{
    public class Companies_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public async void Register_a_new_company_Test()
        {
            KontecgSession.Use(null, 1);

            var companyAppService = LocalIocManager.Resolve<ICompanyAppService>();
            var companyRegistrationAppService = LocalIocManager.Resolve<ICompanyRegistrationAppService>();

            var registeredCompany = await companyRegistrationAppService.RegisterCompanyAsync(new RegisterCompanyInput
            {
                CompanyName = "CEPRONI",
                Name = "Empresa de Servicios y Proyectos del Níquel",
                Reup = "xxx.x.xxxxx",
                Organism = "MINEM",
                AdminEmailAddress = "postmaster@ceproni.moa.minem.cu",
                AdminPassword = "postmaster",
                Address = new Address()
            });

            registeredCompany.CompanyId.ShouldBeGreaterThan(0);

            var featuresEditOutput =
                await companyAppService.GetCompanyFeaturesForEditAsync(new EntityDto(registeredCompany.CompanyId));

            featuresEditOutput.ShouldNotBeNull();

            featuresEditOutput.FeatureValues.ForEach(f => f.Value = true.ToString().ToLowerInvariant());

            await companyAppService.UpdateCompanyFeaturesAsync(new UpdateCompanyFeaturesInput()
            {
                Id = registeredCompany.CompanyId,
                FeatureValues = featuresEditOutput.FeatureValues
            });
        }

        [Fact]
        public async void Send_mass_notification_Test()
        {
            KontecgSession.Use(null, 1);

            var notificationAppService = LocalIocManager.Resolve<INotificationAppService>();

            var lookupTable = await notificationAppService.GetAllUserForLookupTableAsync(new GetAllForLookupTableInput());

            await notificationAppService.CreateMassNotificationAsync(new CreateMassNotificationInput
            {
                Message = "Probando sistema de notificaciones",
                UserIds = lookupTable.Items.Select(u => u.Id).ToArray(),
                TargetNotifiers = notificationAppService.GetAllNotifiers().ToArray()
            });
        }

        [Fact]
        public async void Try_set_logo_and_paper_legal_Test()
        {
            KontecgSession.Use(null, 1);

            var companyManager = LocalIocManager.Resolve<CompanyManager>();
            var mimetypeResolver = LocalIocManager.Resolve<IMimeTypeMap>();
            var binaryObjectManager = LocalIocManager.Resolve<IBinaryObjectManager>();
            byte[] byteArray;

            await WithUnitOfWorkAsync(async () =>
            {
                var company = await companyManager.FindByCompanyNameAsync(KontecgCompanyBase.DefaultCompanyName);
                var image = Image.FromFile("Membrete.png");
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    byteArray = ms.ToArray();
                }

                if (company.HasLetterHead())
                {
                    await binaryObjectManager.DeleteAsync(company.LetterHeadId.Value);
                    company.ClearLetterHead();
                }

                var storedFile = new BinaryObject(company.Id, byteArray,
                    $"Membrete para informes de la empresa {company.CompanyName}. {DateTime.UtcNow}");
                await binaryObjectManager.SaveAsync(storedFile);

                company.LetterHeadId = storedFile.Id;
                company.LetterHeadFileType = mimetypeResolver.GetMimeType("Membrete.png");

                await companyManager.UpdateAsync(company);
            });

        }


        [Fact]
        public void Try_to_get_company_from_domain_Test()
        {
            Func<string[], string, ExtractionResult> predicate = (formats, domain) =>
            {
                foreach (var item in formats)
                {
                    var domainFormat = item.RemovePreFix("http://", "https://").Split(':')[0].RemovePostFix("/");
                    var result = new FormattedStringValueExtracter().Extract(domain, domainFormat, true, '.');

                    if (result.IsMatch && result.Matches.Any())
                    {
                        return result;
                    }
                }

                return null;
            };

            using var disposable = KontecgSession.Use(1, 1);
            var configuration = LocalIocManager.Resolve<IKontecgCoreConfiguration>();
            var companyStore = LocalIocManager.Resolve<ICompanyStore>();

            configuration.DomainFormat.ShouldNotBeNull();

            var domainName = "ecg.moa.minbas.cu";
            var computerName = "UBST-TIC09.ecg.moa.minbas.cu";

            domainName.ShouldNotBe(computerName);

            var domainFormats = configuration.DomainFormat.Split(";");
            var result = predicate(domainFormats, domainName);

            result.ShouldNotBeNull();

            string companyName = result.Matches[0].Value.RemovePostFix(".");

            var companyInfo = companyStore.Find(companyName);
            companyInfo.ShouldNotBeNull();
        }
    }
}
