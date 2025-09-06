using AutoMapper;
using Itenso.TimePeriod;
using Kontecg.Application.Features;
using Kontecg.Auditing;
using Kontecg.Auditing.Dto;
using Kontecg.Authorization;
using Kontecg.Authorization.Accounts.Dto;
using Kontecg.Authorization.Permissions.Dto;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Roles.Dto;
using Kontecg.Authorization.Users;
using Kontecg.Authorization.Users.Dto;
using Kontecg.Authorization.Users.Profile.Dto;
using Kontecg.Currencies.Dtos;
using Kontecg.DynamicEntityProperties.Dto;
using Kontecg.DynamicEntityProperties;
using Kontecg.EntityHistory;
using Kontecg.Extensions;
using Kontecg.Features.Dto;
using Kontecg.Localization;
using Kontecg.Localization.Dto;
using Kontecg.MultiCompany;
using Kontecg.MultiCompany.Dto;
using Kontecg.Notifications;
using Kontecg.Notifications.Dto;
using Kontecg.Organizations;
using Kontecg.Organizations.Dto;
using Kontecg.Sessions.Dto;
using Kontecg.UI.Inputs;
using NMoneys;
using Kontecg.Authorization.Delegation;
using Kontecg.Authorization.Users.Delegation.Dto;
using Kontecg.HumanResources.Dto;
using Kontecg.Accounting;
using Kontecg.Accounting.Dto;
using Kontecg.Currencies;
using Kontecg.HumanResources;
using Kontecg.Timing.Dto;
using Kontecg.Timing;
using Kontecg.Workflows;
using Kontecg.WorkRelations.Dto;
using Kontecg.WorkRelations;
using Kontecg.Identity;
using Kontecg.Identity.Dto;
using Elsa.Expressions.Helpers;

namespace Kontecg
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Role>().ReverseMap();
            configuration.CreateMap<Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Company
            //configuration.CreateMap<Company, RecentCompany>();
            configuration.CreateMap<Company, CompanyLoginInfoDto>();
            configuration.CreateMap<Company, CompanyListDto>();
            configuration.CreateMap<CompanyEditDto, Company>().ReverseMap();
            configuration.CreateMap<CurrentCompanyInfoDto, Company>().ReverseMap();
            configuration.CreateMap<Company, CompanyInfoDto>();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();
            configuration.CreateMap<WorkPlaceUnit, WorkPlaceUnitDto>()
                         .ForMember(d => d.Parent, o => o.MapFrom(m => m.Parent as WorkPlaceUnit))
                         .ForMember(d => d.Classification, o => o.MapFrom(m => m.Classification.Description))
                         .ForMember(d => d.Level, o => o.MapFrom(m => m.Classification.Level))
                         .ForMember(d => d.WorkPlacePaymentCode, o => o.MapFrom(m => m.WorkPlacePayment.Code));

            //Person
            configuration.CreateMap<Person, PersonDto>();
            

            //WorkRelationship
            configuration.CreateMap<WorkRelationship, WorkRelationshipDto>();

            //DynamicProperties
            configuration.CreateMap<DynamicProperty, DynamicPropertyDto>().ReverseMap();
            configuration.CreateMap<DynamicPropertyValue, DynamicPropertyValueDto>().ReverseMap();
            configuration.CreateMap<DynamicEntityProperty, DynamicEntityPropertyDto>()
                .ForMember(dto => dto.DynamicPropertyName,
                    options => options.MapFrom(entity =>
                        entity.DynamicProperty.DisplayName.IsNullOrEmpty()
                            ? entity.DynamicProperty.PropertyName
                            : entity.DynamicProperty.DisplayName));
            configuration.CreateMap<DynamicEntityPropertyDto, DynamicEntityProperty>();

            configuration.CreateMap<DynamicEntityPropertyValue, DynamicEntityPropertyValueDto>().ReverseMap();

            //User Delegations
            configuration.CreateMap<CreateUserDelegationDto, UserDelegation>();

            configuration.CreateMap<Sign, SignDto>();

            //Accounting & Currencies
            configuration.CreateMap<Money, MoneyDto>().ReverseMap();
            configuration.CreateMap<Money, string>().ConvertUsing(m => m.Format("{1}{0}"));
            configuration.CreateMap<PersonalAccountingInfo, PersonalAccountingInfoDto>();
            configuration.CreateMap<AccountingVoucherDocument, AccountingVoucherOutputDto>()
                .ForMember(p => p.Legal, o => o.MapFrom(m => m.DocumentDefinition.Legal));
            configuration.CreateMap<AccountingVoucherNote, AccountingVoucherNoteDto>();
            configuration.CreateMap<ViewName, ViewNameDto>();
            configuration.CreateMap<DocumentDefinition, DocumentDefinitionDto>();
            configuration.CreateMap<AccountingDocument, AccountingDocumentOutputDto>();
            configuration.CreateMap<BillDenomination, BillDto>().ReverseMap();
            configuration.CreateMap<AccountDefinition, AccountDefinitionDto>();
            configuration.CreateMap<CenterCostDefinition, CenterCostDefinitionDto>();
            configuration.CreateMap<ExpenseItemDefinition, ExpenseItemDefinitionDto>();
            configuration.CreateMap<AccountingFunctionDefinition, AccountingFunctionDefinitionDto>()
                         .ForMember(p => p.Formula, o => o.MapFrom(m => m.Storage.Script));
            configuration.CreateMap<AccountingClassifierDefinition, AccountingClassifierDefinitionDto>();

            //Periods
            configuration.CreateMap<Period, PeriodDto>()
                .ForMember(p => p.Start, o => o.MapFrom(m => m.Since))
                .ForMember(p => p.End, o => o.MapFrom(m => m.Until))
                .ForMember(p => p.Duration, o => o.MapFrom(m => m.ToTimePeriod().Duration));
            configuration.CreateMap<Period, PeriodInfoDto>()
                         .ForMember(p => p.Duration, o => o.MapFrom(m => m.ToTimePeriod().Duration));
            configuration.CreateMap<PeriodInfo, PeriodInfoDto>()
                         .ForMember(p => p.Duration, o => o.MapFrom(m => m.ToTimePeriod().Duration));

            configuration.CreateMap<ITimePeriod, PeriodDto>();
            configuration.CreateMap<ITimeRange, WorkingHoursDto>();
            configuration.CreateMap<WorkingHours, WorkingHoursDto>();
            configuration.CreateMap<CalendarTimeDecoration, CalendarTimeDecorationDto>()
                .ForMember(m => m.Order, o => o.MapFrom(m => m.Decorator.To<int>())); 
            configuration.CreateMap<SpecialDate, SpecialDateDto>();
        }
    }
}
