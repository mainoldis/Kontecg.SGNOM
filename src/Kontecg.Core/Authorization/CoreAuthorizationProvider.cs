using Kontecg.Configuration.Startup;
using Kontecg.Localization;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization
{
    /// <summary>
    ///     Application's authorization provider.
    ///     Defines permissions for the application.
    ///     See <see cref="PermissionNames" /> for all permission names.
    /// </summary>
    public class CoreAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiCompanyEnabled;

        public CoreAuthorizationProvider(bool isMultiCompanyEnabled)
        {
            _isMultiCompanyEnabled = isMultiCompanyEnabled;
        }

        public CoreAuthorizationProvider(IMultiCompanyConfig multiCompanyConfig)
        {
            _isMultiCompanyEnabled = multiCompanyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var windows = context.GetPermissionOrNull(PermissionNames.Root) ??
                          context.CreatePermission(PermissionNames.Root, L("Kontecg"));

            var administration = windows.CreateChildPermission(PermissionNames.Administration, L("Administration"));

            var roles = administration.CreateChildPermission(PermissionNames.AdministrationRoles, L("Roles"));
            roles.CreateChildPermission(PermissionNames.AdministrationRolesCreate, L("CreatingNewRole"));
            roles.CreateChildPermission(PermissionNames.AdministrationRolesEdit, L("EditingRole"));
            roles.CreateChildPermission(PermissionNames.AdministrationRolesDelete, L("DeletingRole"));

            var users = administration.CreateChildPermission(PermissionNames.AdministrationUsers, L("Users"));
            users.CreateChildPermission(PermissionNames.AdministrationUsersCreate, L("CreatingNewUser"));
            users.CreateChildPermission(PermissionNames.AdministrationUsersEdit, L("EditingUser"));
            users.CreateChildPermission(PermissionNames.AdministrationUsersDelete, L("DeletingUser"));
            users.CreateChildPermission(PermissionNames.AdministrationUsersChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(PermissionNames.AdministrationUsersUnlock, L("Unlock"));
            users.CreateChildPermission(PermissionNames.AdministrationUsersImpersonation, L("LoginForUsers"));

            var languages = administration.CreateChildPermission(PermissionNames.AdministrationLanguages, L("Languages"));
            languages.CreateChildPermission(PermissionNames.AdministrationLanguagesCreate, L("CreatingNewLanguage"), multiCompanySides: _isMultiCompanyEnabled ? MultiCompanySides.Host : MultiCompanySides.Company);
            languages.CreateChildPermission(PermissionNames.AdministrationLanguagesEdit, L("EditingLanguage"), multiCompanySides: _isMultiCompanyEnabled ? MultiCompanySides.Host : MultiCompanySides.Company);
            languages.CreateChildPermission(PermissionNames.AdministrationLanguagesDelete, L("DeletingLanguages"), multiCompanySides: _isMultiCompanyEnabled ? MultiCompanySides.Host : MultiCompanySides.Company);
            languages.CreateChildPermission(PermissionNames.AdministrationLanguagesChangeTexts, L("ChangingTexts"));

            administration.CreateChildPermission(PermissionNames.AdministrationAuditLogs, L("AuditLogs"));

            var organizationUnits = administration.CreateChildPermission(PermissionNames.AdministrationOrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(PermissionNames.AdministrationOrganizationUnitsManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(PermissionNames.AdministrationOrganizationUnitsManageWorkPlacesTree, L("ManagingWorkPlaceTree"));
            organizationUnits.CreateChildPermission(PermissionNames.AdministrationOrganizationUnitsManageMembers, L("ManagingMembers"));
            organizationUnits.CreateChildPermission(PermissionNames.AdministrationOrganizationUnitsManageRoles, L("ManagingRoles"));

            var dynamicProperties = administration.CreateChildPermission(PermissionNames.AdministrationDynamicProperties, L("DynamicProperties"));
            dynamicProperties.CreateChildPermission(PermissionNames.AdministrationDynamicPropertiesCreate, L("CreatingDynamicProperties"));
            dynamicProperties.CreateChildPermission(PermissionNames.AdministrationDynamicPropertiesEdit, L("EditingDynamicProperties"));
            dynamicProperties.CreateChildPermission(PermissionNames.AdministrationDynamicPropertiesDelete, L("DeletingDynamicProperties"));

            var dynamicPropertyValues = dynamicProperties.CreateChildPermission(PermissionNames.AdministrationDynamicPropertyValue, L("DynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(PermissionNames.AdministrationDynamicPropertyValueCreate, L("CreatingDynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(PermissionNames.AdministrationDynamicPropertyValueEdit, L("EditingDynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(PermissionNames.AdministrationDynamicPropertyValueDelete, L("DeletingDynamicPropertyValue"));

            var dynamicEntityProperties = dynamicProperties.CreateChildPermission(PermissionNames.AdministrationDynamicEntityProperties, L("DynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(PermissionNames.AdministrationDynamicEntityPropertiesCreate, L("CreatingDynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(PermissionNames.AdministrationDynamicEntityPropertiesEdit, L("EditingDynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(PermissionNames.AdministrationDynamicEntityPropertiesDelete, L("DeletingDynamicEntityProperties"));

            var dynamicEntityPropertyValues = dynamicProperties.CreateChildPermission(PermissionNames.AdministrationDynamicEntityPropertyValue, L("EntityDynamicPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(PermissionNames.AdministrationDynamicEntityPropertyValueCreate, L("CreatingDynamicEntityPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(PermissionNames.AdministrationDynamicEntityPropertyValueEdit, L("EditingDynamicEntityPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(PermissionNames.AdministrationDynamicEntityPropertyValueDelete, L("DeletingDynamicEntityPropertyValue"));

            var massNotification = administration.CreateChildPermission(PermissionNames.AdministrationMassNotification, L("MassNotifications"));
            massNotification.CreateChildPermission(PermissionNames.AdministrationMassNotificationCreate, L("MassNotificationCreate"));

            var humanResources = windows.CreateChildPermission(PermissionNames.HumanResources, L("HumanResources"));
            var persons = humanResources.CreateChildPermission(PermissionNames.HumanResourcesPersons, L("ManagingPersons"));
            persons.CreateChildPermission(PermissionNames.HumanResourcesPersonsList, L("Persons"));
            persons.CreateChildPermission(PermissionNames.HumanResourcesPersonsCreate, L("CreatingNewPerson"));
            persons.CreateChildPermission(PermissionNames.HumanResourcesPersonsEdit, L("EditingPerson"));
            persons.CreateChildPermission(PermissionNames.HumanResourcesPersonsDelete, L("DeletingPerson"));

            var accounting = windows.CreateChildPermission(PermissionNames.Accounting, L("Accounting"));
            accounting.CreateChildPermission(PermissionNames.AccountingGetAll, L("AccountingGetAll"));
            accounting.CreateChildPermission(PermissionNames.AccountingGetBase, L("AccountingGetBase"));
            accounting.CreateChildPermission(PermissionNames.AccountingGetPersonal, L("AccountingGetPersonal"));
            accounting.CreateChildPermission(PermissionNames.AccountingGetPeriod, L("AccountingGetPeriod"));
            accounting.CreateChildPermission(PermissionNames.AccountingListDocuments, L("AccountingListDocuments"));
            accounting.CreateChildPermission(PermissionNames.AccountingListVouchers, L("AccountingListVouchers"));

            var accounts = accounting.CreateChildPermission(PermissionNames.AccountingAccounts, L("AccountingAccounts"));
            accounts.CreateChildPermission(PermissionNames.AccountingAccountsCreate, L("AccountingAccountsCreate"));
            accounts.CreateChildPermission(PermissionNames.AccountingAccountsUpdate, L("AccountingAccountsUpdate"));
            accounts.CreateChildPermission(PermissionNames.AccountingAccountsDelete, L("AccountingAccountsDelete"));

            var centerCosts = accounting.CreateChildPermission(PermissionNames.AccountingCenterCosts, L("AccountingCenterCosts"));
            centerCosts.CreateChildPermission(PermissionNames.AccountingCenterCostsCreate, L("AccountingCenterCostsCreate"));
            centerCosts.CreateChildPermission(PermissionNames.AccountingCenterCostsUpdate, L("AccountingCenterCostsUpdate"));
            centerCosts.CreateChildPermission(PermissionNames.AccountingCenterCostsDelete, L("AccountingCenterCostsDelete"));

            var expenseItems = accounting.CreateChildPermission(PermissionNames.AccountingExpenseItems, L("AccountingExpenseItems"));
            expenseItems.CreateChildPermission(PermissionNames.AccountingExpenseItemsCreate, L("AccountingExpenseItemsCreate"));
            expenseItems.CreateChildPermission(PermissionNames.AccountingExpenseItemsUpdate, L("AccountingExpenseItemsUpdate"));
            expenseItems.CreateChildPermission(PermissionNames.AccountingExpenseItemsDelete, L("AccountingExpenseItemsDelete"));

            var functions = accounting.CreateChildPermission(PermissionNames.AccountingFunctions, L("AccountingFunctions"));
            functions.CreateChildPermission(PermissionNames.AccountingFunctionsCreate, L("AccountingFunctionsCreate"));
            functions.CreateChildPermission(PermissionNames.AccountingFunctionsUpdate, L("AccountingFunctionsUpdate"));
            functions.CreateChildPermission(PermissionNames.AccountingFunctionsDelete, L("AccountingFunctionsDelete"));

            var documents = accounting.CreateChildPermission(PermissionNames.AccountingDocuments, L("AccountingDocuments"));
            documents.CreateChildPermission(PermissionNames.AccountingDocumentsCreate, L("AccountingDocumentsCreate"));
            documents.CreateChildPermission(PermissionNames.AccountingDocumentsUpdate, L("AccountingDocumentsUpdate"));
            documents.CreateChildPermission(PermissionNames.AccountingDocumentsDelete, L("AccountingDocumentsDelete"));

            var vouchers = accounting.CreateChildPermission(PermissionNames.AccountingVouchers, L("AccountingVouchers"));
            vouchers.CreateChildPermission(PermissionNames.AccountingVouchersCreate, L("AccountingVouchersCreate"));
            vouchers.CreateChildPermission(PermissionNames.AccountingVouchersUpdate, L("AccountingVouchersUpdate"));
            vouchers.CreateChildPermission(PermissionNames.AccountingVouchersDelete, L("AccountingVouchersDelete"));

            //TENANT-SPECIFIC PERMISSIONS
            windows.CreateChildPermission(PermissionNames.CompanyDashboard, L("Dashboard"), multiCompanySides: MultiCompanySides.Company);
            administration.CreateChildPermission(PermissionNames.AdministrationCompanySettings, L("Settings"), multiCompanySides: MultiCompanySides.Company);

            //HOST-SPECIFIC PERMISSIONS
            var companies = windows.CreateChildPermission(PermissionNames.Companies, L("Companies"), multiCompanySides: MultiCompanySides.Host);
            companies.CreateChildPermission(PermissionNames.CompaniesCreate, L("CreatingNewCompany"), multiCompanySides: MultiCompanySides.Host);
            companies.CreateChildPermission(PermissionNames.CompaniesEdit, L("EditingCompany"), multiCompanySides: MultiCompanySides.Host);
            companies.CreateChildPermission(PermissionNames.CompaniesChangeFeatures, L("ChangingFeatures"), multiCompanySides: MultiCompanySides.Host);
            companies.CreateChildPermission(PermissionNames.CompaniesDelete, L("DeletingCompany"), multiCompanySides: MultiCompanySides.Host);
            companies.CreateChildPermission(PermissionNames.CompaniesImpersonation, L("LoginForCompanies"), multiCompanySides: MultiCompanySides.Host);

            administration.CreateChildPermission(PermissionNames.AdministrationHostSettings, L("Settings"), multiCompanySides: MultiCompanySides.Host);
            var maintenance = administration.CreateChildPermission(PermissionNames.AdministrationHostMaintenance, L("Maintenance"),
                multiCompanySides: _isMultiCompanyEnabled ? MultiCompanySides.Host : MultiCompanySides.Company);
            maintenance.CreateChildPermission(PermissionNames.AdministrationNewVersionCreate, L("SendNewVersionNotification"));

            //Special Dates permissions
            var specialDates = administration.CreateChildPermission(PermissionNames.AdministrationSpecialDate,
                L(PermissionNames.AdministrationSpecialDate), multiCompanySides: MultiCompanySides.Host);
            specialDates.CreateChildPermission(PermissionNames.AdministrationSpecialDateList,
                L(PermissionNames.AdministrationSpecialDateList), multiCompanySides: MultiCompanySides.Host);
            specialDates.CreateChildPermission(PermissionNames.AdministrationSpecialDateCreate,
                L(PermissionNames.AdministrationSpecialDateCreate), multiCompanySides: MultiCompanySides.Host);
            specialDates.CreateChildPermission(PermissionNames.AdministrationSpecialDateUpdate,
                L(PermissionNames.AdministrationSpecialDateUpdate), multiCompanySides: MultiCompanySides.Host);
            specialDates.CreateChildPermission(PermissionNames.AdministrationSpecialDateDelete,
                L(PermissionNames.AdministrationSpecialDateDelete), multiCompanySides: MultiCompanySides.Host);

            administration.CreateChildPermission(PermissionNames.AdministrationHostDashboard,
                L("Dashboard"), multiCompanySides: MultiCompanySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, KontecgConsts.LocalizationSourceName);
        }
    }
}
