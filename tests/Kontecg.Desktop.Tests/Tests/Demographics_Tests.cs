using System;
using System.Collections.Generic;
using System.IO;
using Kontecg.Common;
using Kontecg.Common.Dto;
using Kontecg.HumanResources;
using Kontecg.Json;
using Kontecg.Primitives;
using Shouldly;
using Xunit;

namespace Kontecg.Desktop.Tests
{
    public class Demographics_Tests : DesktopTestModuleTestBase
    {
        private readonly ICommonLookupAppService _commonLookupAppService;

        public Demographics_Tests()
        {
            var admin = GetDefaultCompanyAdmin();
            KontecgSession.CompanyId = admin.CompanyId;
            KontecgSession.UserId = admin.Id;
            _commonLookupAppService = Resolve<ICommonLookupAppService>();
        }

        [Fact]
        public async void Get_specific_state_Tests()
        {
            var states = await _commonLookupAppService.GetStatesForComboboxAsync();
            states.Items.ShouldContain(s => s.DisplayText == "LA HABANA");
        }

        [Fact]
        public async void Get_specific_city_Tests()
        {
            var cities =
                await _commonLookupAppService.GetCitiesByStateForComboboxAsync(new FindCitiesInput() {State = "27"});
            cities.Items.ShouldNotBeEmpty();
        }

        [Fact]
        public async void Serialize_some_persons()
        {
            var persons = new List<Person>();

            persons.Add(new Person("Mainoldis", "Fuentes", "Suárez", "82121726486", Gender.M, new DateTime(1982,12,17)));
            persons.Add(new Person("Milena", "Guilarte", "Santiesteban", "98120226486", Gender.F, new DateTime(1998, 12, 2)));

            string jsonString = persons.ToJsonString();

            string filePath = @"persons.json";
            await File.WriteAllTextAsync(filePath, jsonString);
        }
    }
}
