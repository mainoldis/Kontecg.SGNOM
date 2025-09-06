namespace Kontecg.WorkRelations
{
    public record EmploymentIndexSettingRecord(
        int StartIndexForLifeTimeContracts,
        int EndIndexForLifeTimeContracts,
        int StartIndexForInitialLifeTimeContracts,
        int EndIndexForInitialLifeTimeContracts,
        int StartIndexForShortContracts,
        int EndIndexForShortContracts,
        int StartIndexForTemporallyContracts,
        int EndIndexForTemporallyContracts);
}
