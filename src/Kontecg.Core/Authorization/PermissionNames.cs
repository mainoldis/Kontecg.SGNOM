namespace Kontecg.Authorization
{
    /// <summary>
    ///     Defines string constants for application's permission names.
    ///     <see cref="CoreAuthorizationProvider" /> for permission definitions.
    /// </summary>
    public static class PermissionNames
    {
        //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

        public const string Root = "Kontecg";

        public const string Administration = "Kontecg.Administration";

        public const string AdministrationRoles = "Kontecg.Administration.Roles";
        public const string AdministrationRolesCreate = "Kontecg.Administration.Roles.Create";
        public const string AdministrationRolesEdit = "Kontecg.Administration.Roles.Edit";
        public const string AdministrationRolesDelete = "Kontecg.Administration.Roles.Delete";

        public const string AdministrationUsers = "Kontecg.Administration.Users";
        public const string AdministrationUsersCreate = "Kontecg.Administration.Users.Create";
        public const string AdministrationUsersEdit = "Kontecg.Administration.Users.Edit";
        public const string AdministrationUsersDelete = "Kontecg.Administration.Users.Delete";
        public const string AdministrationUsersChangePermissions = "Kontecg.Administration.Users.ChangePermissions";
        public const string AdministrationUsersImpersonation = "Kontecg.Administration.Users.Impersonation";
        public const string AdministrationUsersUnlock = "Kontecg.Administration.Users.Unlock";

        public const string AdministrationLanguages = "Kontecg.Administration.Languages";
        public const string AdministrationLanguagesCreate = "Kontecg.Administration.Languages.Create";
        public const string AdministrationLanguagesEdit = "Kontecg.Administration.Languages.Edit";
        public const string AdministrationLanguagesDelete = "Kontecg.Administration.Languages.Delete";
        public const string AdministrationLanguagesChangeTexts = "Kontecg.Administration.Languages.ChangeTexts";

        public const string AdministrationAuditLogs = "Kontecg.Administration.AuditLogs";

        public const string AdministrationOrganizationUnits = "Kontecg.Administration.OrganizationUnits";
        public const string AdministrationOrganizationUnitsManageOrganizationTree = "Kontecg.Administration.OrganizationUnits.ManageOrganizationTree";
        public const string AdministrationOrganizationUnitsManageWorkPlacesTree = "Kontecg.Administration.OrganizationUnits.ManageWorkPlaceTree";
        public const string AdministrationOrganizationUnitsManageMembers = "Kontecg.Administration.OrganizationUnits.ManageMembers";
        public const string AdministrationOrganizationUnitsManageRoles = "Kontecg.Administration.OrganizationUnits.ManageRoles";

        public const string AdministrationDynamicProperties = "Kontecg.Administration.DynamicProperties";
        public const string AdministrationDynamicPropertiesCreate = "Kontecg.Administration.DynamicProperties.Create";
        public const string AdministrationDynamicPropertiesEdit = "Kontecg.Administration.DynamicProperties.Edit";
        public const string AdministrationDynamicPropertiesDelete = "Kontecg.Administration.DynamicProperties.Delete";

        public const string AdministrationDynamicPropertyValue = "Kontecg.Administration.DynamicPropertyValue";
        public const string AdministrationDynamicPropertyValueCreate = "Kontecg.Administration.DynamicPropertyValue.Create";
        public const string AdministrationDynamicPropertyValueEdit = "Kontecg.Administration.DynamicPropertyValue.Edit";
        public const string AdministrationDynamicPropertyValueDelete = "Kontecg.Administration.DynamicPropertyValue.Delete";

        public const string AdministrationDynamicEntityProperties = "Kontecg.Administration.DynamicEntityProperties";
        public const string AdministrationDynamicEntityPropertiesCreate = "Kontecg.Administration.DynamicEntityProperties.Create";
        public const string AdministrationDynamicEntityPropertiesEdit = "Kontecg.Administration.DynamicEntityProperties.Edit";
        public const string AdministrationDynamicEntityPropertiesDelete = "Kontecg.Administration.DynamicEntityProperties.Delete";

        public const string AdministrationDynamicEntityPropertyValue = "Kontecg.Administration.DynamicEntityPropertyValue";
        public const string AdministrationDynamicEntityPropertyValueCreate = "Kontecg.Administration.DynamicEntityPropertyValue.Create";
        public const string AdministrationDynamicEntityPropertyValueEdit = "Kontecg.Administration.DynamicEntityPropertyValue.Edit";
        public const string AdministrationDynamicEntityPropertyValueDelete = "Kontecg.Administration.DynamicEntityPropertyValue.Delete";

        public const string AdministrationMassNotification = "Kontecg.Administration.MassNotification";
        public const string AdministrationMassNotificationCreate = "Kontecg.Administration.MassNotification.Create";

        public const string AdministrationNewVersionCreate = "Kontecg.Administration.NewVersion.Create";

        public const string Accounting = "Kontecg.Accounting";
        public const string AccountingGetAll = "Kontecg.Accounting.GetAll";
        public const string AccountingGetBase = "Kontecg.Accounting.GetBase";
        public const string AccountingGetPersonal = "Kontecg.Accounting.GetPersonal";
        public const string AccountingGetPeriod = "Kontecg.Accounting.GetPeriod";
        public const string AccountingListDocuments = "Kontecg.Accounting.ListDocuments";
        public const string AccountingListVouchers = "Kontecg.Accounting.ListVouchers";
        public const string AccountingAccounts = "Kontecg.Accounting.Accounts";
        public const string AccountingAccountsCreate = "Kontecg.Accounting.Accounts.Create";
        public const string AccountingAccountsUpdate = "Kontecg.Accounting.Accounts.Update";
        public const string AccountingAccountsDelete = "Kontecg.Accounting.Accounts.Delete";
        public const string AccountingCenterCosts = "Kontecg.Accounting.CenterCosts";
        public const string AccountingCenterCostsCreate = "Kontecg.Accounting.CenterCosts.Create";
        public const string AccountingCenterCostsUpdate = "Kontecg.Accounting.CenterCosts.Update";
        public const string AccountingCenterCostsDelete = "Kontecg.Accounting.CenterCosts.Delete";
        public const string AccountingExpenseItems = "Kontecg.Accounting.ExpenseItems";
        public const string AccountingExpenseItemsCreate = "Kontecg.Accounting.ExpenseItems.Create";
        public const string AccountingExpenseItemsUpdate = "Kontecg.Accounting.ExpenseItems.Update";
        public const string AccountingExpenseItemsDelete = "Kontecg.Accounting.ExpenseItems.Delete";
        public const string AccountingFunctions = "Kontecg.Accounting.Functions";
        public const string AccountingFunctionsCreate = "Kontecg.Accounting.Functions.Create";
        public const string AccountingFunctionsUpdate = "Kontecg.Accounting.Functions.Update";
        public const string AccountingFunctionsDelete = "Kontecg.Accounting.Functions.Delete";

        public const string AccountingDocuments = "Kontecg.Accounting.Documents";
        public const string AccountingDocumentsCreate = "Kontecg.Accounting.Documents.Create";
        public const string AccountingDocumentsUpdate = "Kontecg.Accounting.Documents.Update";
        public const string AccountingDocumentsDelete = "Kontecg.Accounting.Documents.Delete";

        public const string AccountingVouchers = "Kontecg.Accounting.Vouchers";
        public const string AccountingVouchersCreate = "Kontecg.Accounting.Vouchers.Create";
        public const string AccountingVouchersUpdate = "Kontecg.Accounting.Vouchers.Update";
        public const string AccountingVouchersDelete = "Kontecg.Accounting.Vouchers.Delete";


        //TENANT-SPECIFIC PERMISSIONS

        public const string CompanyDashboard = "Kontecg.Company.Dashboard";
        public const string AdministrationCompanySettings = "Kontecg.Administration.Company.Settings";

        //HOST-SPECIFIC PERMISSIONS

        public const string Companies = "Kontecg.Companies";
        public const string CompaniesCreate = "Kontecg.Companies.Create";
        public const string CompaniesEdit = "Kontecg.Companies.Edit";
        public const string CompaniesChangeFeatures = "Kontecg.Companies.ChangeFeatures";
        public const string CompaniesDelete = "Kontecg.Companies.Delete";
        public const string CompaniesImpersonation = "Kontecg.Companies.Impersonation";

        public const string AdministrationHostMaintenance = "Kontecg.Administration.Host.Maintenance";
        public const string AdministrationHostSettings = "Kontecg.Administration.Host.Settings";
        public const string AdministrationHostDashboard = "Kontecg.Administration.Host.Dashboard";

        public const string AdministrationSpecialDate = $"Kontecg.Administration.SpecialDates";
        public const string AdministrationSpecialDateList = $"Kontecg.Administration.SpecialDates.List";
        public const string AdministrationSpecialDateCreate = $"Kontecg.Administration.SpecialDates.Create";
        public const string AdministrationSpecialDateUpdate = $"Kontecg.Administration.SpecialDates.Update";
        public const string AdministrationSpecialDateDelete = $"Kontecg.Administration.SpecialDates.Delete";

        public const string HumanResources = "Kontecg.HumanResources";
        public const string HumanResourcesPersons = "Kontecg.HumanResources.Persons";
        public const string HumanResourcesPersonsList = "Kontecg.HumanResources.Persons.List";
        public const string HumanResourcesPersonsCreate = "Kontecg.HumanResources.Persons.Create";
        public const string HumanResourcesPersonsEdit = "Kontecg.HumanResources.Persons.Edit";
        public const string HumanResourcesPersonsDelete = "Kontecg.HumanResources.Persons.Delete";
    }
}
