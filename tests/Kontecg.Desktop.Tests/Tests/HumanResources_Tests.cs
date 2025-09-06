using System;
using System.Threading.Tasks;
using Kontecg.Common.Dto;
using Kontecg.HumanResources;
using Kontecg.HumanResources.Dto;
using Kontecg.IO;
using Kontecg.ObjectMapping;
using Kontecg.Primitives;
using Kontecg.Resources.Embedded;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class HumanResources_Tests : DesktopTestModuleTestBase
    {
        [Fact]
        public void Get_persons_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var pm = Resolve<IHumanResourcesAppService>();

            var pagedResultDto = pm.GetAll(new FindPersonsInput());

            pagedResultDto.Items.ShouldNotBeEmpty();
        }

        [Fact]
        public void Get_one_people_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var pm = Resolve<IHumanResourcesAppService>();

            var resultDto = pm.GetAll(new FindPersonsInput {IdentityCard = "82121726486"});
            resultDto.ShouldNotBeNull();
        }

        [Fact]
        public async Task Get_photo_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var pm = Resolve<IPersonManager>();

            var person = await pm.FindPersonByIdentityCardAsync("82121726486");
            if (person != null && person.HasPhoto())
            {
                var fileDto = await pm.GetPhotoPersonByIdAsync(person.Id);
                fileDto.ShouldNotBeNull();

                AppFileHelper.SaveToFile(fileDto.FileName, fileDto.File);
            }
        }

        [Theory]
        [InlineData("Mainoldis", "Fuentes", "Suárez", "82121726486", Gender.M, "1982/12/17")]
        public async Task Add_new_person_Test(string name, string surname, string lastname, string identityCard, Gender gender, string birthDate)
        {
            DateTime birthDateAsDateTime = DateTime.Parse(birthDate);

            var pm = Resolve<PersonManager>();
            var mapper = Resolve<IObjectMapper>();
            var rm = LocalIocManager.Resolve<IEmbeddedResourceManager>();
            var defaultPhoto = rm.GetResource("Unknown.jpg");

            using var shouldBeDisposable = KontecgSession.Use(null, 1);
            var input = new PersonDto
            {
                Name = name,
                Surname = surname,
                Lastname = lastname,
                IdentityCard = identityCard,
                Gender = gender.ToString(),
                BirthDate = birthDateAsDateTime
            };

            Person person = mapper.Map<Person>(input);
            person = await pm.CreatePersonAsync(person);
            person = await pm.UpdatePhotoPersonByIdAsync(person.Id, defaultPhoto.Content, defaultPhoto.FileExtension);
        }

        [Fact]
        public void Get_old_people_Test()
        {
            using var shouldBeDisposable = KontecgSession.Use(1, 2);
            var pm = Resolve<IHumanResourcesAppService>();

            var agedPeople = pm.GetAgedPeople();
            agedPeople.Items.ShouldNotBeEmpty();
        }
    }
}
