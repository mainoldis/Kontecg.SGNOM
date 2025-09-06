using Kontecg.Localization;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization
{
    /// <inheritdoc />
    public class SGNOMAuthorizationProvider : AuthorizationProvider
    {
        /// <inheritdoc />
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var modulo = context.GetPermissionOrNull(SGNOMPermissions.Modulo) ??
                         context.CreatePermission(SGNOMPermissions.Modulo, L(SGNOMPermissions.Modulo),
                             multiCompanySides: MultiCompanySides.Company);

            var demographics =
                modulo.CreateChildPermission(SGNOMPermissions.Demographics, L(SGNOMPermissions.Demographics),
                    multiCompanySides: MultiCompanySides.Company);

            var workRelations =
                modulo.CreateChildPermission(SGNOMPermissions.WorkRelations, L(SGNOMPermissions.WorkRelations),
                    multiCompanySides: MultiCompanySides.Company);

            //Employments permissions
            var employments = workRelations.CreateChildPermission(SGNOMPermissions.Employments,
                L(SGNOMPermissions.Employments), multiCompanySides: MultiCompanySides.Company);
            employments.CreateChildPermission(SGNOMPermissions.EmploymentsList, L(SGNOMPermissions.EmploymentsList),
                multiCompanySides: MultiCompanySides.Company);
            employments.CreateChildPermission(SGNOMPermissions.EmploymentsExport, L(SGNOMPermissions.EmploymentsExport),
                multiCompanySides: MultiCompanySides.Company);

            //WorkSchedule permissions
            var workSchedule = workRelations.CreateChildPermission(SGNOMPermissions.WorkSchedule,
                L(SGNOMPermissions.WorkSchedule), multiCompanySides: MultiCompanySides.Company);
            workSchedule.CreateChildPermission(SGNOMPermissions.WorkScheduleList, L(SGNOMPermissions.WorkScheduleList),
                multiCompanySides: MultiCompanySides.Company);

            var salary =
                modulo.CreateChildPermission(SGNOMPermissions.Salary, L(SGNOMPermissions.Salary),
                    multiCompanySides: MultiCompanySides.Company);

            var holidays =
                modulo.CreateChildPermission(SGNOMPermissions.Holidays, L(SGNOMPermissions.Holidays),
                    multiCompanySides: MultiCompanySides.Company);

            var socialSecurity =
                modulo.CreateChildPermission(SGNOMPermissions.SocialSecurity, L(SGNOMPermissions.SocialSecurity),
                    multiCompanySides: MultiCompanySides.Company);

            var retentions =
                modulo.CreateChildPermission(SGNOMPermissions.Retentions, L(SGNOMPermissions.Retentions),
                    multiCompanySides: MultiCompanySides.Company);

            var claims =
                modulo.CreateChildPermission(SGNOMPermissions.Claims, L(SGNOMPermissions.Claims),
                    multiCompanySides: MultiCompanySides.Company);

            var organizations =
                modulo.CreateChildPermission(SGNOMPermissions.Organizations, L(SGNOMPermissions.Organizations),
                    multiCompanySides: MultiCompanySides.Company);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, SGNOMConsts.LocalizationSourceName);
        }
    }
}
