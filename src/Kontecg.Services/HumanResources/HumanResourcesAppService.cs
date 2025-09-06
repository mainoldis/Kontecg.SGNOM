using System.Linq;
using Kontecg.Application.Services;
using Kontecg.Application.Services.Dto;
using Kontecg.Authorization;
using Kontecg.Common.Dto;
using Kontecg.EntityHistory;
using Kontecg.Extensions;
using Kontecg.HumanResources.Dto;
using Kontecg.Linq.Extensions;

namespace Kontecg.HumanResources
{
    [KontecgAuthorize(PermissionNames.HumanResources)]
    [UseCase(Description = "Servicio de Gestión de Recursos Humanos")]
    public class HumanResourcesAppService : CrudAppService<Person, PersonDto, long, FindPersonsInput, PersonDto, PersonDto, EntityDto<long>, EntityDto<long>>, IHumanResourcesAppService
    {
        private readonly PersonManager _personManager;
        private readonly IPersonRepository _personRepository;

        /// <inheritdoc />
        public HumanResourcesAppService(
            PersonManager personManager,
            IPersonRepository repository) 
            : base(repository)
        {
            _personManager = personManager;
            _personRepository = repository;
            LocalizationSourceName = KontecgCoreConsts.LocalizationSourceName;
        }

        protected override string GetPermissionName => PermissionNames.HumanResourcesPersons;

        protected override string GetAllPermissionName => PermissionNames.HumanResourcesPersonsList;

        protected override string CreatePermissionName => PermissionNames.HumanResourcesPersonsCreate;

        protected override string UpdatePermissionName => PermissionNames.HumanResourcesPersonsEdit;

        protected override string DeletePermissionName => PermissionNames.HumanResourcesPersonsDelete;

        /// <inheritdoc />
        protected override IQueryable<Person> CreateFilteredQuery(FindPersonsInput input)
        {
            var query = _personManager.Persons
                                      .WhereIf(
                                          !input.IdentityCard.IsNullOrWhiteSpace(),
                                          p => p.IdentityCard.StartsWith(input.IdentityCard))
                                      .WhereIf(
                                          input.Gender.HasValue,
                                          p => p.Gender == input.Gender)
                                      .WhereIf(
                                          !input.Filter.IsNullOrWhiteSpace(),
                                          p =>
                                              p.Name.Contains(input.Filter) ||
                                              p.Surname.Contains(input.Filter) ||
                                              p.Lastname.Contains(input.Filter));
            return query;
        }

        /// <inheritdoc />
        public override PersonDto Create(PersonDto input)
        {
            CheckCreatePermission();

            Person entity = MapToEntity(input);

            entity = _personManager.CreatePerson(entity);

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(entity);
        }

        /// <inheritdoc />
        public override PersonDto Update(PersonDto input)
        {
            CheckUpdatePermission();

            Person entity = GetEntityById(input.Id);

            MapToEntity(input, entity);

            entity = _personManager.UpdatePerson(entity);

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(entity);
        }

        /// <inheritdoc />
        public PagedResultDto<PersonDto> GetAgedPeople(bool nextYear = false)
        {
            CheckGetAllPermission();

            var persons = _personRepository.GetAgedPeople(nextYear);

            return new PagedResultDto<PersonDto>(
                persons.Count,
                persons.Select(MapToEntityDto).ToList()
            );
        }
    }
}
