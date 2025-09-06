using System.Collections.Generic;
using System.Threading.Tasks;
using Kontecg.Storage;

namespace Kontecg.HumanResources
{
    public interface IPersonManager
    {
        Task<Person> FindPersonByIdAsync(long id);

        Person FindPersonById(long id);

        Task<Person> GetByIdAsync(long id);

        Person GetById(long id);

        T GetExtraData<T>(long id, string propertyName);

        Task<T> GetExtraDataAsync<T>(long id, string propertyName);

        void SetExtraData<T>(long id, string propertyName, T value);

        Task SetExtraDataAsync<T>(long id, string propertyName, T value);

        Task<Person> FindPersonByIdentityCardAsync(string identityCard);

        Person FindPersonByIdentityCard(string identityCard);

        Task<TempFileInfo> GetPhotoPersonByIdAsync(long personId);

        Task<Person> UpdatePhotoPersonByIdAsync(long personId, byte[] stream, string mimeType);

        Task<Person> CreatePersonAsync(Person person);

        Person CreatePerson(Person person);

        Task<Person> UpdatePersonAsync(Person person);

        Person UpdatePerson(Person person);

        Task DeletePersonAsync(Person person);

        void DeletePerson(Person person);

        Task<IReadOnlyList<Country>> GetCountriesAsync();

        IReadOnlyList<Country> GetCountries();

        Task<IReadOnlyList<State>> GetStatesByCountryCodeAsync(string countryCode = KontecgCoreConsts.DefaultCountryCode);

        IReadOnlyList<State> GetStatesByCountryCode(string countryCode = KontecgCoreConsts.DefaultCountryCode);

        Task<IReadOnlyList<City>> GetCitiesByStateCodeAsync(string stateCode);

        IReadOnlyList<City> GetCitiesByStateCode(string stateCode);
    }
}
