namespace Kontecg.Authorization
{
    /// <summary>
    ///     Defines string constants for application's permission names.
    ///     <see cref="SGNOMAuthorizationProvider" /> for permission definitions.
    /// </summary>
    public static class SGNOMPermissions
    {
        public const string Modulo = "SGNOM";

        public const string Demographics = $"{Modulo}.{nameof(Demographics)}";

        public const string WorkRelations = $"{Modulo}.{nameof(WorkRelations)}";

        public const string Employments = $"{WorkRelations}.Employments";

        public const string EmploymentsList = $"{WorkRelations}.Employments.List";

        public const string EmploymentsExport = $"{WorkRelations}.Employments.Export";

        public const string WorkSchedule = $"{WorkRelations}.WorkSchedule";

        public const string WorkScheduleList = $"{WorkRelations}.WorkSchedule.List";

        public const string Salary = $"{Modulo}.{nameof(Salary)}";

        public const string Holidays = $"{Modulo}.{nameof(Holidays)}";

        public const string SocialSecurity = $"{Modulo}.{nameof(SocialSecurity)}";

        public const string Retentions = $"{Modulo}.{nameof(Retentions)}";

        public const string Claims = $"{Modulo}.{nameof(Claims)}";

        public const string Organizations = $"{Modulo}.{nameof(Organizations)}";
    }
}
