using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Baseline;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Linq;
using Kontecg.MimeTypes;
using Kontecg.Organizations;
using Kontecg.Resources.Embedded;
using Kontecg.Runtime.Session;
using Kontecg.Storage;
using Kontecg.Timing;
using Kontecg.UI;

namespace Kontecg.HumanResources
{
    /// <summary>
    ///     Performs domain logic for persons.
    /// </summary>
    public class PersonManager : KontecgCoreDomainServiceBase, IPersonManager
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IMimeTypeMap _mimeTypeMap;
        private readonly IPersonSettings _settings;
        private readonly IPersonRepository _personRepository;
        private readonly IPersonOrganizationUnitRepository _personOrganizationUnitRepository;
        private readonly IEmbeddedResourceManager _embeddedResourceManager;

        private readonly IRepository<Country, long> _countryRepository;
        private readonly IRepository<State, long> _stateRepository;
        private readonly IRepository<City, long> _cityRepository;

        public PersonManager(
            IPersonRepository personRepository,
            IPersonOrganizationUnitRepository personOrganizationUnitRepository,
            IRepository<Country, long> countryRepository,
            IRepository<State, long> stateRepository,
            IRepository<City, long> cityRepository,
            IPersonSettings settings,
            IBinaryObjectManager binaryObjectManager,
            ITempFileCacheManager tempFileCacheManager,
            IMimeTypeMap mimeTypeMap,
            IEmbeddedResourceManager embeddedResourceManager)
        {
            _personRepository = personRepository;
            _personOrganizationUnitRepository = personOrganizationUnitRepository;
            _countryRepository = countryRepository;
            _stateRepository = stateRepository;
            _cityRepository = cityRepository;
            _binaryObjectManager = binaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;

            _mimeTypeMap = mimeTypeMap;
            _embeddedResourceManager = embeddedResourceManager;
            _settings = settings;

            LocalizationSourceName = KontecgBaselineConsts.LocalizationSourceName;
            KontecgSession = NullKontecgSession.Instance;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public IKontecgSession KontecgSession { get; set; }

        public virtual IQueryable<Person> Persons => _personRepository.GetAll();

        #region General methods

        public virtual async Task<Person> FindPersonByIdAsync(long id)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _personRepository.FirstOrDefaultAsync(id));
        }

        public virtual Person FindPersonById(long id)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
                _personRepository.FirstOrDefault(id));
        }

        public virtual async Task<Person> GetByIdAsync(long id)
        {
            var person = await FindPersonByIdAsync(id);
            if (person == null) throw new KontecgException("There is no person with id: " + id);

            return person;
        }

        public virtual Person GetById(long id)
        {
            var person = FindPersonById(id);
            if (person == null) throw new KontecgException("There is no person with id: " + id);

            return person;
        }

        public virtual T GetExtraData<T>(long id, string propertyName)
        {
            return GetById(id).GetData<T>(propertyName);
        }

        public virtual async Task<T> GetExtraDataAsync<T>(long id, string propertyName)
        {
            var person = await GetByIdAsync(id);
            return await Task.FromResult(person.GetData<T>(propertyName));
        }

        public void SetExtraData<T>(long id, string propertyName, T value)
        {
            var person = FindPersonById(id);
            if (person == null) throw new KontecgException("There is no person with id: " + id);
            person.SetData<T>(propertyName, value);
            UpdatePerson(person);
        }

        public async Task SetExtraDataAsync<T>(long id, string propertyName, T value)
        {
            var person = await FindPersonByIdAsync(id);
            if (person == null) throw new KontecgException("There is no person with id: " + id);
            person.SetData<T>(propertyName, value);
            await UpdatePersonAsync(person);
        }

        public virtual async Task<Person> FindPersonByIdentityCardAsync(string identityCard)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(() =>
                _personRepository.FirstOrDefaultAsync(p => p.IdentityCard == identityCard));
        }

        public virtual Person FindPersonByIdentityCard(string identityCard)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
                _personRepository.FirstOrDefault(p => p.IdentityCard == identityCard));
        }

        public virtual async Task<TempFileInfo> GetPhotoPersonByIdAsync(long personId)
        {
            var person = await FindPersonByIdAsync(personId);
            var defaultPhoto = _embeddedResourceManager.GetResource("Unknown.jpg");
            if (person == null || !person.HasPhoto())
            {
                TempFileInfo defaultFile = new("Unknown.jpg", "image/jpeg", defaultPhoto.Content);
                _tempFileCacheManager.SetFile(defaultFile.FileName, defaultFile.File);
                return defaultFile;
            }

            var extension = _mimeTypeMap.GetExtension(person.PhotoFileType);
            var binaryObject = await _binaryObjectManager.GetOrNullAsync(person.PhotoId.Value);
            if (binaryObject == null)
            {
                TempFileInfo defaultFile = new("Unknown.jpg", "image/jpeg", defaultPhoto.Content);
                _tempFileCacheManager.SetFile(defaultFile.FileName, defaultFile.File);
                return defaultFile;
            }

            TempFileInfo file = new(person.IdentityCard + extension, person.PhotoFileType, binaryObject.Bytes);
            _tempFileCacheManager.SetFile(person.PhotoId.Value.ToString(), file);
            return file;
        }

        public async Task<Person> UpdatePhotoPersonByIdAsync(long personId, byte[] stream, string mimeType)
        {
            if (stream == null || stream.Length == 0) return null;

            Person result = null;
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var person = await FindPersonByIdAsync(personId);
                if (person == null) return;

                if (person.HasPhoto())
                {
                    await _binaryObjectManager.DeleteAsync(person.PhotoId.Value);
                    person.ClearPhoto();
                }

                var storedFile = new BinaryObject(null, stream, $"Photo {person.IdentityCard}. {Clock.Now}");
                await _binaryObjectManager.SaveAsync(storedFile);

                person.PhotoId = storedFile.Id;
                person.PhotoFileType = _mimeTypeMap.GetMimeType(mimeType);

                result = await _personRepository.UpdateAsync(person);
            });

            return result;
        }

        public virtual async Task<Person> CreatePersonAsync(Person person)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (await _personRepository.FirstOrDefaultAsync(p => p.IdentityCard == person.IdentityCard) != null)
                    throw new UserFriendlyException(string.Format(L("PersonIsAlreadyExists"), person.FullName));

                return await _personRepository.InsertAsync(person);
            });
        }

        public virtual Person CreatePerson(Person person)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                if (_personRepository.FirstOrDefault(p => p.IdentityCard == person.IdentityCard) != null)
                    throw new UserFriendlyException(string.Format(L("PersonIsAlreadyExists"), person.FullName));

                return _personRepository.Insert(person);
            });
        }

        public virtual async Task<Person> UpdatePersonAsync(Person person)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await ValidatePersonAsync(person);

                if (await _personRepository.FirstOrDefaultAsync(p =>
                        p.IdentityCard == person.IdentityCard && p.Id != person.Id) != null)
                    throw new UserFriendlyException(string.Format(L("PersonIsAlreadyExists"), person.FullName));

                return await _personRepository.UpdateAsync(person);
            });
        }

        public virtual Person UpdatePerson(Person person)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                ValidatePerson(person);

                if (_personRepository.FirstOrDefault(p => p.IdentityCard == person.IdentityCard && p.Id != person.Id) !=
                    null)
                    throw new UserFriendlyException(string.Format(L("PersonIsAlreadyExists"), person.FullName));

                return _personRepository.Update(person);
            });
        }

        public virtual async Task DeletePersonAsync(Person person)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () => await _personRepository.DeleteAsync(person));
        }

        public virtual void DeletePerson(Person person)
        {
            UnitOfWorkManager.WithUnitOfWork(() => _personRepository.Delete(person));
        }

        #endregion

        #region Countries, States and Cities

        public virtual async Task<IReadOnlyList<Country>> GetCountriesAsync()
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(() => _countryRepository.GetAllListAsync());
        }

        public virtual IReadOnlyList<Country> GetCountries()
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
                _countryRepository.GetAllList().ToImmutableList());
        }

        public virtual async Task<IReadOnlyList<State>> GetStatesByCountryCodeAsync(
            string countryCode = KontecgCoreConsts.DefaultCountryCode)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _stateRepository.GetAllListAsync(s => s.Country.InternationalCode == countryCode));
        }

        public virtual IReadOnlyList<State> GetStatesByCountryCode(
            string countryCode = KontecgCoreConsts.DefaultCountryCode)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
                _stateRepository.GetAllList(s => s.Country.InternationalCode == countryCode).ToImmutableList());
        }

        public virtual async Task<IReadOnlyList<City>> GetCitiesByStateCodeAsync(string stateCode)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _cityRepository.GetAllListAsync(s => s.State.Code == stateCode));
        }

        public virtual IReadOnlyList<City> GetCitiesByStateCode(string stateCode)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
                _cityRepository.GetAllList(s => s.State.Code == stateCode).ToImmutableList());
        }

        #endregion

        #region Validations

        protected virtual async Task ValidatePersonAsync(Person person)
        {
            if ((await _settings.MustHavePhotoAsync(KontecgSession.CompanyId)) && !person.HasPhoto())
                throw new UserFriendlyException(L("PersonMustHavePhotoWarning", person.FullName));

            if ((await _settings.MustHaveAddressAsync(KontecgSession.CompanyId)) && person.OfficialAddress == null)
                throw new UserFriendlyException(L("PersonMustHaveAddressWarning", person.FullName));

            if (await _settings.MustHaveEtniaAsync(KontecgSession.CompanyId) && (person.Etnia == null))
                throw new UserFriendlyException(L("PersonMustHaveEtniaWarning", person.FullName));

            if (await _settings.MustHaveClothingSizesAsync(KontecgSession.CompanyId) && person.ClothingSize == null)
                throw new UserFriendlyException(L("PersonMustHaveClothingSizesWarning", person.FullName));

            if (person.Age < 18)
                throw new UserFriendlyException(L("PersonAgeMustBeGeaterThan18Warning", person.FullName));
        }

        protected virtual void ValidatePerson(Person person)
        {
            if ((_settings.MustHavePhoto(KontecgSession.CompanyId)) && !person.HasPhoto())
                throw new UserFriendlyException(L("PersonMustHavePhotoWarning", person.FullName));

            if ((_settings.MustHaveAddress(KontecgSession.CompanyId)) && person.OfficialAddress == null)
                throw new UserFriendlyException(L("PersonMustHaveAddressWarning", person.FullName));

            if (_settings.MustHaveEtnia(KontecgSession.CompanyId) && (person.Etnia == null))
                throw new UserFriendlyException(L("PersonMustHaveEtniaWarning", person.FullName));

            if (_settings.MustHaveClothingSizes(KontecgSession.CompanyId) && person.ClothingSize == null)
                throw new UserFriendlyException(L("PersonMustHaveClothingSizesWarning", person.FullName));

            if (person.Age < 18)
                throw new UserFriendlyException(L("PersonAgeMustBeGeaterThan18Warning", person.FullName));
        }

        #endregion
    }
}
