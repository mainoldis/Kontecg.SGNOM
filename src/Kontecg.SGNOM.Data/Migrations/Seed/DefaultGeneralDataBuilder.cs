using System;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using EntityFramework.Exceptions.Common;
using Kontecg.Domain;
using Kontecg.EFCore;
using Kontecg.HumanResources;
using Kontecg.Identity;
using Kontecg.Json;
using Kontecg.Model;
using Kontecg.MultiCompany;
using Kontecg.WorkRelations;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Migrations.Seed
{
    public class DefaultGeneralDataBuilder
    {
        private readonly SGNOMDbContext _context;
        private readonly IContentFolders _contentFolders;
        private readonly ILogger _logger;

        public DefaultGeneralDataBuilder(SGNOMDbContext context, IContentFolders contentFolders, ILogger logger)
        {
            _context = context;
            _contentFolders = contentFolders;
            _logger = logger;
        }

        public void Create()
        {
            var companyId = KontecgCoreConsts.MultiCompanyEnabled ? MultiCompanyConsts.DefaultCompanyId : 1;

            CreateScholarshipLevels();
            CreateDegrees();
            CreateDriverLicenses();
            CreateMilitaryLocations();
            CreateLawPenalties();
            CreateSignReferences();
            CreateCommonDataForEmployments();

            ImportPersonalAccountsFromExternalDatabase(_contentFolders.DataFolder);
            ImportEmploymentDocumentsFromExternalDatabase(_contentFolders.DataFolder);
        }

        private void CreateDegrees()
        {
            AddDegreeDefinitionIfNotExists("Ing. Agronomía");
            AddDegreeDefinitionIfNotExists("Ing. Arquitectura");
            AddDegreeDefinitionIfNotExists("Ing. Automática");
            AddDegreeDefinitionIfNotExists("Ing. Bioinformática");
            AddDegreeDefinitionIfNotExists("Ing. Biomédica");
            AddDegreeDefinitionIfNotExists("Ing. Ciberseguridad");
            AddDegreeDefinitionIfNotExists("Ing. Ciencias Informáticas");
            AddDegreeDefinitionIfNotExists("Ing. Civil");
            AddDegreeDefinitionIfNotExists("Ing. Eléctrica");
            AddDegreeDefinitionIfNotExists("Ing. Geología");
            AddDegreeDefinitionIfNotExists("Ing. Industrial");
            AddDegreeDefinitionIfNotExists("Ing. Informática");
            AddDegreeDefinitionIfNotExists("Ing. Mecánica");
            AddDegreeDefinitionIfNotExists("Ing. Metalúrgica");
            AddDegreeDefinitionIfNotExists("Ing. Minas");
            AddDegreeDefinitionIfNotExists("Ing. Química");
            AddDegreeDefinitionIfNotExists("Ing. Telecomunicaciones");
            AddDegreeDefinitionIfNotExists("Lic. en Arte de los Medios de la Comunicación Audiovisual");
            AddDegreeDefinitionIfNotExists("Lic. en Bibliotecología y Ciencias de la Información");
            AddDegreeDefinitionIfNotExists("Lic. en Biología");
            AddDegreeDefinitionIfNotExists("Lic. en Ciencias de la Computación");
            AddDegreeDefinitionIfNotExists("Lic. en Ciencias Médicas");
            AddDegreeDefinitionIfNotExists("Lic. en Comunicación Social");
            AddDegreeDefinitionIfNotExists("Lic. en Contabilidad y Finanzas");
            AddDegreeDefinitionIfNotExists("Lic. en Cultura Física");
            AddDegreeDefinitionIfNotExists("Lic. en Derecho");
            AddDegreeDefinitionIfNotExists("Lic. en Diseño Gráfico");
            AddDegreeDefinitionIfNotExists("Lic. en Economía");
            AddDegreeDefinitionIfNotExists("Lic. en Educación Lengua extranjera inglés");
            AddDegreeDefinitionIfNotExists("Lic. en Educación Logopedia");
            AddDegreeDefinitionIfNotExists("Lic. en Educación Marxismo Leninismo e Historia");
            AddDegreeDefinitionIfNotExists("Lic. en Educación Pedagogía Psicología");
            AddDegreeDefinitionIfNotExists("Lic. en Educación");
            AddDegreeDefinitionIfNotExists("Lic. en Español y Literatura");
            AddDegreeDefinitionIfNotExists("Lic. en Farmacia");
            AddDegreeDefinitionIfNotExists("Lic. en Filología");
            AddDegreeDefinitionIfNotExists("Lic. en Física");
            AddDegreeDefinitionIfNotExists("Lic. en Gestión Sociocultural para el desarrollo");
            AddDegreeDefinitionIfNotExists("Lic. en Historia del Arte");
            AddDegreeDefinitionIfNotExists("Lic. en Historia");
            AddDegreeDefinitionIfNotExists("Lic. en Lengua Inglesa");
            AddDegreeDefinitionIfNotExists("Lic. en Matemática");
            AddDegreeDefinitionIfNotExists("Lic. en Periodismo");
            AddDegreeDefinitionIfNotExists("Lic. en Psicología");
            AddDegreeDefinitionIfNotExists("Lic. en Química");
            AddDegreeDefinitionIfNotExists("Lic. en Sociología");
            AddDegreeDefinitionIfNotExists("Lic. en Turismo");
            AddDegreeDefinitionIfNotExists("TM. en Administración");
            AddDegreeDefinitionIfNotExists("TM. en Construcción Civil");
            AddDegreeDefinitionIfNotExists("TM. en Contabilidad y Finanzas");
            AddDegreeDefinitionIfNotExists("TM. en Economía");
            AddDegreeDefinitionIfNotExists("TM. en Gastronomía");
            AddDegreeDefinitionIfNotExists("TM. en Informática y Comunicaciones");
            AddDegreeDefinitionIfNotExists("TM. en Metalurgia no Ferrosa");
            AddDegreeDefinitionIfNotExists("TM. en Soldadura");
            AddDegreeDefinitionIfNotExists("TM. en Telecomunicaciones");
            AddDegreeDefinitionIfNotExists("TM. en Turismo");
            AddDegreeDefinitionIfNotExists("TS. en Análisis Clínico");
            AddDegreeDefinitionIfNotExists("TS. en Asistencia Turística");
            AddDegreeDefinitionIfNotExists("TS. en Bioquímica");
            AddDegreeDefinitionIfNotExists("TS. en Citohispatología");
            AddDegreeDefinitionIfNotExists("TS. en Enfermería");
            AddDegreeDefinitionIfNotExists("TS. en especialidades pedagógicas para la Educación Técnica y Profesional");
            AddDegreeDefinitionIfNotExists("TS. en Estadística de la Salud");
            AddDegreeDefinitionIfNotExists("TS. en Higiene y Epidemiología");
            AddDegreeDefinitionIfNotExists("TS. en Meteorología");
            AddDegreeDefinitionIfNotExists("TS. en Vigilancia y Lucha Antivectorial");


            _logger.Info("Degree definitions was created.");
        }

        private void CreateLawPenalties()
        {
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Infracción del horario de trabajo o abandono del puesto de trabajo sin autorización del jefe inmediato o desaprovechamiento de la jornada",
                "Ley 116, Artículo 147, Inciso a.");
            AddLawPenaltyCauseDefinitionIfNotExists("Ausencia injustificada", "Ley 116, Artículo 147, Inciso b.");
            AddLawPenaltyCauseDefinitionIfNotExists("Desobediencia a las orientaciones de los superiores",
                "Ley 116, Artículo 147, Inciso c.");
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Falta de respeto a superiores, compañeros de trabajo o a otras personas en la entidad o en ocasión del desempeño del trabajo",
                "Ley 116, Artículo 147, Inciso d");
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Maltrato de obra o de palabra a superiores, compañeros de trabajo u otras personas en la entidad o en ocasión del desempeño del trabajo",
                "Ley 116, Artículo 147, Inciso e");
            AddLawPenaltyCauseDefinitionIfNotExists("Negligencia en el cumplimiento de sus deberes de trabajo",
                "Ley 116, Artículo 147, Inciso f");
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Violaciones de las disposiciones vigentes en la entidad sobre la seguridad y protección de la información oficial, el secreto técnico o comercial, la seguridad informática y para la seguridad y protección física",
                "Ley 116, Artículo 147, Inciso g.");
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Incumplimiento injustificado por parte del trabajador de los deberes que la legislación establece sobre seguridad y salud en el trabajo",
                "Ley 116, Artículo 147, Inciso h");
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Daño y pérdida de los bienes de la entidad o de terceras personas, en ocasión del desempeño del trabajo",
                "Ley 116, Artículo 147, Inciso i");
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Sustracción, desvío o apropiación de bienes o valores propiedad del centro de trabajo o de terceros",
                "Ley 116, Artículo 147, Inciso j");
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Cometer en la entidad o en ocasión del desempeño del trabajo, hechos que pueden ser constitutivos de delitos",
                "Ley 116, Artículo 147, Inciso k");
            AddLawPenaltyCauseDefinitionIfNotExists(
                "Modificar el expediente laboral o aportar documentos carentes de autenticidad para obtener beneficios laborales o de seguridad social mediante engaño",
                "Ley 116, Artículo 147, Inciso l.");
            AddLawPenaltyCauseDefinitionIfNotExists("Violación del reglamento disciplinario", "Ley 116, Artículo 148.");

            AddLawPenaltyDefinitionIfNotExists("Amonestación Pública", "");
            AddLawPenaltyDefinitionIfNotExists("Multa",
                "Multa de hasta el importe del 25% del salario de un mes, mediante descuentos de hasta un 10% del salario mensual");
            AddLawPenaltyDefinitionIfNotExists("InHabilitación para ser ascendido o promovido", "");
            AddLawPenaltyDefinitionIfNotExists("Suspensión del derecho al cobro de incentivos", "");
            AddLawPenaltyDefinitionIfNotExists("Suspensión del derecho escalafonario", "");
            AddLawPenaltyDefinitionIfNotExists("Suspensión del derecho a ser elegido o designado", "");
            AddLawPenaltyDefinitionIfNotExists("Pérdida de Honores", "");
            AddLawPenaltyDefinitionIfNotExists("Suspensión del vínculo laboral con un término de hasta 30 días", "");
            AddLawPenaltyDefinitionIfNotExists(
                "Traslado temporal a otra plaza de menor remuneración o condiciones laborales distintas", "");
            AddLawPenaltyDefinitionIfNotExists("Traslado definitivo a otra plaza con pérdida de la que ocupaba", "");
            AddLawPenaltyDefinitionIfNotExists("Separación definitiva de la entidad", "");
            AddLawPenaltyDefinitionIfNotExists("Separación del sector o actividad", "");

            _logger.Info("Law penalties definitions was created.");
        }

        private void CreateMilitaryLocations()
        {
            AddMilitaryLocationIfNotExists("BPD (En Lugar de Residencia)");
            AddMilitaryLocationIfNotExists("BPD (En Entidad Estatal)");
            AddMilitaryLocationIfNotExists("MTT");
            AddMilitaryLocationIfNotExists("UM");
            AddMilitaryLocationIfNotExists("DC");
            AddMilitaryLocationIfNotExists("CD Municipal");
            AddMilitaryLocationIfNotExists("Imprescindibles");
            AddMilitaryLocationIfNotExists("Brigada de evacuación");
            AddMilitaryLocationIfNotExists("UR");
            AddMilitaryLocationIfNotExists("FAR");
            AddMilitaryLocationIfNotExists("MININT");
            AddMilitaryLocationIfNotExists("FEN");
            AddMilitaryLocationIfNotExists("Asignado");
            AddMilitaryLocationIfNotExists("Incorporado");
            AddMilitaryLocationIfNotExists("No incorporado");
            AddMilitaryLocationIfNotExists("Otras");

            _logger.Info("Military location definitions was created.");
        }

        private void ImportPersonalAccountsFromExternalDatabase(string dataFolder)
        {
            try
            {
                var tempDataFile = Path.GetFullPath(Path.Combine(dataFolder, "seed", "personal_accounts.json"));

                if (File.Exists(tempDataFile))
                {
                    //var content = File.ReadAllText(tempDataFile, Encoding.UTF8);
                    //if (!string.IsNullOrEmpty(content))
                    //{
                    //    var personWithAccounts = content.FromJsonString<PersonWithAccount[]>();
                    //    var personWithAccountsToSave =
                    //        _context.ImportPersonAccounts.ToArray().Where(p => p.AccountNumber == null).ToArray()
                    //                .IntersectBy(personWithAccounts.Select(p => p.IdentityCard), p => p.IdentityCard).ToArray();

                    //    for (var index = 0; index < personWithAccountsToSave.Length; index++)
                    //    {
                    //        var import = personWithAccountsToSave[index];
                    //        var accounts = personWithAccounts.Where(d => d.IdentityCard == import.IdentityCard).SelectMany(n => n.PersonalAccounts).ToArray();
                    //        foreach (var account in accounts)
                    //        {
                    //            try
                    //            {
                    //                _context.PersonAccounts.Add(new PersonAccount(import.PersonId,
                    //                    account.AccountNumber, account.Currency.ToString()));
                    //                _context.SaveChanges();
                    //                _logger.Info(
                    //                    $"---> {account.AccountNumber} ({account.Currency}) for {import.Name} {import.Surname} {import.Lastname} Added.");
                    //            }
                    //            catch (UniqueConstraintException ex)
                    //            {
                    //                _logger.Warn(
                    //                    $"---> It seems that {account.AccountNumber} ({account.Currency}) for {import.Name} {import.Surname} {import.Lastname} is already added for another person.");
                    //            }
                    //        }
                    //    }
                    //}

                    _logger.Info("Personal accounts was populated from external data.");
                }
            }
            catch
            {
                _logger.Warn("Personal accounts couldn't be retrieved from external data.");
            }
        }

        private void ImportEmploymentDocumentsFromExternalDatabase(string dataFolder)
        {
            if (_context.Employments.IgnoreQueryFilters().Any())
                return;

            try
            {
                //var sql =
                //    "CREATE TABLE #movimientos(IdMov INT, DocumentDefinitionId INT, MadeOn DATETIME2(7), EffectiveSince DATETIME2(7), Exp INT, PersonId INT, Code NVARCHAR(11), GroupId UNIQUEIDENTIFIER, EmployeeSalaryForm TINYINT, CompanyId INT, Contract NVARCHAR(1), Type NVARCHAR(1), SubType NVARCHAR(3), PreviousId BIGINT, OrganizationUnitId BIGINT, [Level] INT, FirstLevelDisplayName NVARCHAR(128), SecondLevelDisplayName NVARCHAR(128), ThirdLevelDisplayName NVARCHAR(128), CenterCost INT, ComplexityGroup NVARCHAR(8), Occupation NVARCHAR(250), OccupationCode NVARCHAR(8), Responsibility NVARCHAR(150), OccupationCategory NVARCHAR(1), WorkShiftId INT, Salary DECIMAL(10,2), TotalSalary DECIMAL(10,2), RatePerHour DECIMAL(28,24), SummaryId INT, ExtraSummary NVARCHAR(500), ExpirationDate DATETIME2(7), HasSalary BIT, Review INT, CreationTime DATETIME2(7));\r\n\r\nINSERT INTO #movimientos\r\nSELECT RELMovimientos.IdMov, 0 AS DocumentDefinitionId, RELMovimientos.FechaConf AS MadeOn,  RELMovimientos.FechaEfectMov AS EffectiveSince, RELRelacionLaboral.NoExp AS Exp, Kontecg.Kontecg.Persons.Id AS PersonId, RELMovimientos.NumeroMov AS Code, RELMovimientos.IdEmpleo AS GroupId, \r\n       2 AS EmployeeSalaryForm, 1 AS CompanyId,\r\n\t   CASE RELMovimientosDGral.TipoCont WHEN 'I' THEN 'I' ELSE 'D' END AS Contract, RELMovimientos.TipoMov AS Type, RELMovimientos.SubTipoMov AS SubType, NULL AS PreviousId, ISNULL(Kontecg.Kontecg.OrganizationUnits.Id, 0) AS OrganizationUnitId, Kontecg.Kontecg.WorkPlaceClassifications.Level AS [Level], RELMovimientosHist.AreaN1 AS FirstLevelDisplayName, \r\n       '' AS SecondLevelDisplayName, RELMovimientosHist.Area AS ThirdLevelDisplayName, RELMovimientosHist.CCosto AS CenterCost, RTRIM(LTRIM(RELMovimientosHist.GrupoSalarial)) AS ComplexityGroup, RELMovimientosHist.Cargo AS Occupation, RELMovimientosHist.CodCargo AS OccupationCode,\r\n\t   CASE WHEN MGResponsabilidadCargo.Responsabilidad IN ('Jefe de Brigada', 'Especialista Principal', 'Jefe de Taller') THEN MGResponsabilidadCargo.Responsabilidad ELSE 'Del Cargo' END AS Responsibility,\r\n       RTRIM(LTRIM(RELMovimientosHist.CategOcup)) AS OccupationCategory, Sgnom.Sgnom.REL_WorkShifts.Id AS WorkShiftId, MGGrupoSalarial.SalarioEscala AS Salary, RELMovimientosDGral.SalarioTotal AS TotalSalary, (MGGrupoSalarial.SalarioEscala / 190.6) AS RatePerHour,\r\n       ISNULL(Sgnom.Sgnom.REL_EmploymentSummaries.Id, 20) AS SummaryId, \r\n       RELMovimientos.Observaciones AS ExtraSummary, \r\n       CASE WHEN RELMovimientos.SubTipoMov IN('MP', 'PMP') THEN RELMovProvisional.FechaVenc ELSE RELMovDeter.FechaVenc END AS ExpirationDate,\r\n\t   1 AS HasSalary, 2 AS Review, GETDATE() AS CreationTime\r\nFROM   ECGIARA.iAraGlobal_ECG.dbo.RELRelacionLaboral INNER JOIN\r\n       ECGIARA.iAraGlobal_ECG.dbo.PERPersonas ON RELRelacionLaboral.IdPersona = PERPersonas.IdPersona INNER JOIN\r\n       ECGIARA.iAraGlobal_ECG.dbo.RELMovimientosHist ON RELRelacionLaboral.IdMov = RELMovimientosHist.IdMov INNER JOIN\r\n       ECGIARA.iAraGlobal_ECG.dbo.RELMovimientosDGral ON RELMovimientosHist.IdMov = RELMovimientosDGral.IdMovimiento INNER JOIN\r\n       ECGIARA.iAraGlobal_ECG.dbo.MGTurnosLaborales ON RELMovimientosDGral.IdTurno = MGTurnosLaborales.IdTurno INNER JOIN\r\n       ECGIARA.iAraGlobal_ECG.dbo.RELMovimientos ON RELRelacionLaboral.IdMov = RELMovimientos.IdMov AND RELMovimientosDGral.IdMovimiento = RELMovimientos.IdMov INNER JOIN\r\n       ECGIARA.iAraGlobal_ECG.dbo.MGResponsabilidadCargo ON RELMovimientosDGral.IdResponsabilidad = MGResponsabilidadCargo.IdResponsabilidadCargo LEFT OUTER JOIN\r\n\t   ECGIARA.iAraGlobal_ECG.dbo.MGEspecifMotivosMvtos ON MGEspecifMotivosMvtos.IdEspecifMvto = RELMovimientos.IdEspecifMov INNER JOIN\r\n       ECGIARA.iAraGlobal_ECG.dbo.MGGrupoSalarial ON RELMovimientosDGral.IdGrupoSalarial = MGGrupoSalarial.IdGrupoSalarial LEFT OUTER JOIN\r\n       ECGIARA.iAraGlobal_ECG.dbo.RELMovDeter ON RELMovimientosDGral.IdMovimiento = RELMovDeter.IdMov LEFT OUTER JOIN\r\n\t   ECGIARA.iAraGlobal_ECG.dbo.RELMovProvisional ON RELMovimientosDGral.IdMovimiento = RELMovProvisional.IdMov LEFT OUTER JOIN\r\n\t   Kontecg.Kontecg.Persons ON Kontecg.Kontecg.Persons.IdentityCard = PERPersonas.CI COLLATE SQL_Latin1_General_CP1_CI_AS LEFT OUTER JOIN\r\n\t   Kontecg.Kontecg.OrganizationUnits ON Kontecg.Kontecg.OrganizationUnits.DisplayName = LTRIM(RTRIM(RELMovimientosHist.AreaN1)) COLLATE SQL_Latin1_General_CP1_CI_AS LEFT OUTER JOIN\r\n\t   Kontecg.Kontecg.WorkPlaceClassifications ON Kontecg.Kontecg.OrganizationUnits.ClassificationId = Kontecg.Kontecg.WorkPlaceClassifications.Id LEFT OUTER JOIN\r\n\t   Sgnom.Sgnom.REL_WorkShifts ON Sgnom.Sgnom.REL_WorkShifts.DisplayName = CASE WHEN MGTurnosLaborales.Descripcion NOT IN ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L') THEN 'N' ELSE LTRIM(RTRIM(MGTurnosLaborales.Descripcion)) END COLLATE SQL_Latin1_General_CP1_CI_AS LEFT OUTER JOIN\r\n       Sgnom.Sgnom.REL_EmploymentSummaries ON REL_EmploymentSummaries.DisplayName = LTRIM(RTRIM(MGEspecifMotivosMvtos.EspecificacionMvto)) COLLATE SQL_Latin1_General_CP1_CI_AS\r\nWHERE  RELMovimientos.Procesado = 1 AND (Kontecg.Kontecg.WorkPlaceClassifications.Level IS NULL OR Kontecg.Kontecg.WorkPlaceClassifications.Level = 3)\r\n\r\nINSERT INTO Sgnom.Sgnom.REL_EmploymentDocuments(DocumentDefinitionId, MadeOn, EffectiveSince, Exp, PersonId, Code, GroupId, EmployeeSalaryForm, CompanyId, Contract, Type, SubType, PreviousId, OrganizationUnitId, FirstLevelDisplayName, SecondLevelDisplayName, ThirdLevelDisplayName, CenterCost, ComplexityGroup, Occupation, OccupationCode, Responsibility, OccupationCategory, WorkShiftId, Salary, TotalSalary, RatePerHour, SummaryId, ExtraSummary, ExpirationDate, HasSalary, Review, CreationTime)\r\nSELECT DocumentDefinitionId, MadeOn, EffectiveSince, Exp, PersonId, Code, GroupId, EmployeeSalaryForm, CompanyId, Contract, Type, SubType, PreviousId, OrganizationUnitId, FirstLevelDisplayName, SecondLevelDisplayName, ThirdLevelDisplayName, CenterCost, ComplexityGroup, Occupation, OccupationCode, Responsibility, OccupationCategory, WorkShiftId, Salary, TotalSalary, RatePerHour, SummaryId, ExtraSummary, ExpirationDate, HasSalary, Review, CreationTime FROM #movimientos WHERE PersonId IS NOT NULL;\r\n\r\n\r\nINSERT INTO Sgnom.Sgnom.REL_EmploymentPluses(EmploymentId, PlusDefinitionId, Amount, RatePerHour, CompanyId, CreationTime)\r\nSELECT REL_EmploymentDocuments.Id AS EmploymentId, SAL_PlusDefinitions.Id AS PlusDefinitionId, RELMovAdiciones.Monto AS Amount, (RELMovAdiciones.Monto / 190.6) AS RatePerHour, 1 AS CompanyId, GETDATE() AS CreationTime\r\nFROM   ECGIARA.iAraGlobal_ECG.dbo.RELMovAdiciones INNER JOIN\r\n\t   ECGIARA.iAraGlobal_ECG.dbo.MGTiposAdiciones ON MGTiposAdiciones.IdAdiciones = RELMovAdiciones.IdAdiciones INNER JOIN\r\n\t   #movimientos ON #movimientos.IdMov = RELMovAdiciones.IdMov LEFT OUTER JOIN\r\n\t   Sgnom.Sgnom.REL_EmploymentDocuments ON REL_EmploymentDocuments.Exp = #movimientos.Exp AND #movimientos.PersonId = REL_EmploymentDocuments.PersonId LEFT OUTER JOIN \r\n\t   Sgnom.Sgnom.SAL_PlusDefinitions ON SAL_PlusDefinitions.Name = MGTiposAdiciones.Abrev COLLATE SQL_Latin1_General_CP1_CI_AS\r\nWHERE #movimientos.PersonId IS NOT NULL;\r\n\r\n \r\nUPDATE Sgnom.Sgnom.REL_EmploymentDocuments SET TotalSalary = Salary + t.Plus\r\nFROM\r\n(SELECT EmploymentId, CompanyId, SUM(REL_EmploymentPluses.Amount) AS Plus\r\nFROM Sgnom.Sgnom.REL_EmploymentPluses\r\nGROUP BY EmploymentId, CompanyId) t\r\nWHERE Sgnom.Sgnom.REL_EmploymentDocuments.Id = t.EmploymentId AND Sgnom.Sgnom.REL_EmploymentDocuments.CompanyId = t.CompanyId\r\n\r\nDROP TABLE #movimientos;";
                //_context.Database.ExecuteSqlRaw(sql);
                _logger.Info("Employments documents was populated from external database.");
            }
            catch (Exception ex)
            {
                _logger.Warn("Employments documents couldn't be retrieved from external database.", ex);
            }
        }

        private void CreateCommonDataForEmployments()
        {
            AddCriteriaIfNotExists("Certificados médicos");
            AddCriteriaIfNotExists("Peritados e Inválidos Parciales");
            AddCriteriaIfNotExists("Prestación Social (60%)");
            AddCriteriaIfNotExists("Ausencias Justificadas (Con Permiso y Licencias retribuidas)");
            AddCriteriaIfNotExists("Ausencias Injustificadas (Sin Permiso)");
            AddCriteriaIfNotExists("Licencias Deportivas y Culturales");
            AddCriteriaIfNotExists(
                "Licencias por Fallecimiento de familiares (Padre, Madre, Cónyugue, Hermanos o Hijos)");
            AddCriteriaIfNotExists("Licencias No Retribuida(Mes completo)");
            AddCriteriaIfNotExists("Licencias Deportivas y Culturales (Mes completo)");
            AddCriteriaIfNotExists("Citación Judicial del Tribunal o la Fiscalía");
            AddCriteriaIfNotExists("Estar Detenido o Sometido a Prisión Provisional");
            AddCriteriaIfNotExists("Sanción Laboral");
            AddCriteriaIfNotExists("Asistir a Consulta Médicas para trabajadores con el VIH-SIDA o Portadores");
            AddCriteriaIfNotExists("Aislado por COVID - 19");
            AddCriteriaIfNotExists("Otras causas");

            //NOTA: Altas
            var parentId =  AddEmploymentSummaryIfNotExists("Pluriempleo", EmploymentType.A, "PEMP");
            AddEmploymentSummaryIfNotExists("Proveniente de la propia entidad", EmploymentType.A, "PEMP", parentId, reference: "PEMP1");
            AddEmploymentSummaryIfNotExists("Proveniente de otra entidad de CUBANIQUEL", EmploymentType.A, "PEMP", parentId, reference: "PEMP2");
            AddEmploymentSummaryIfNotExists("Proveniente de otra entidad fuera de CUBANIQUEL", EmploymentType.A, "PEMP", parentId, reference: "PEMP3");
            AddEmploymentSummaryIfNotExists("Alta como disponible", EmploymentType.A, "AD");
            
            parentId = AddEmploymentSummaryIfNotExists("Movimiento Dirigido", EmploymentType.A);
            AddEmploymentSummaryIfNotExists("Nombramiento por resolución", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Traslado desde otra Entidad del Grupo Empresarial", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Traslado desde otra Entidad fuera del Grupo Empresarial", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Traspaso de Actividad desde otra Entidad", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Licenciamiento de la FAR o MININT", EmploymentType.A, parentId: parentId);
            
            parentId = AddEmploymentSummaryIfNotExists("Recién Graduados Asignados", EmploymentType.A, reference: "ADIESTRADO");
            AddEmploymentSummaryIfNotExists("Graduados de Nivel Superior", EmploymentType.A, parentId: parentId, reference: "ADIESTRADO_SUPERIOR");
            AddEmploymentSummaryIfNotExists("Graduados de Técnico Medio", EmploymentType.A, parentId: parentId, reference: "ADIESTRADO_TECNICO_MEDIO");
            AddEmploymentSummaryIfNotExists("Graduados de Obrero Calificado", EmploymentType.A, parentId: parentId, reference: "ADIESTRADO_OBRERO");
            
            AddEmploymentSummaryIfNotExists("Reserva Laboral de la DMT", EmploymentType.A);
            
            parentId = AddEmploymentSummaryIfNotExists("Reubicación de Disponibles", EmploymentType.A);
            AddEmploymentSummaryIfNotExists("Disponibles reubicados temporalmente", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Disponibles reubicados de forma permanente", EmploymentType.A, parentId: parentId);

            parentId = AddEmploymentSummaryIfNotExists("Contrato Determinado", EmploymentType.A, reference: "CONTRATO_DETERMINADO");
            AddEmploymentSummaryIfNotExists("Por necesidad de la producción", EmploymentType.A, parentId: parentId, reference: "CONTRATO_NECESIDAD");
            AddEmploymentSummaryIfNotExists("Jubilados re-contratados", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Por un Trabajador Enfermo", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Realizar Labores Emergentes", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Sustituir a un trabajador suspendido laboralmente", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Curso de capacitación a trabajadores de nueva incorporación", EmploymentType.A, parentId: parentId, reference: "CONTRATO_CURSO");
            AddEmploymentSummaryIfNotExists("Formación de Pre-grado", EmploymentType.A, parentId: parentId, reference: "CONTRATO_PREGRADO");

            parentId = AddEmploymentSummaryIfNotExists("Retorno a la Entidad Laboral", EmploymentType.A);
            AddEmploymentSummaryIfNotExists("Retorno de una suspensión laboral", EmploymentType.A, parentId: parentId, reference: "RETORNO_SANSIONADO");
            AddEmploymentSummaryIfNotExists("Modificación de medida disciplinaria", EmploymentType.A, parentId: parentId, reference: "RETORNO_MEDIDA");

            parentId = AddEmploymentSummaryIfNotExists("Desvinculados laboralmente", EmploymentType.A, "DL");
            AddEmploymentSummaryIfNotExists("Discapacitados", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Ex-reclusos", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Desmovilizado del Servicio Militar", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Retorno de una suspensión laboral", EmploymentType.A, parentId: parentId);

            parentId = AddEmploymentSummaryIfNotExists("Traslado por solicitud propia", EmploymentType.A, "TSP");
            AddEmploymentSummaryIfNotExists("Vínculo anterior con una Entidad fuera del Grupo Empresarial", EmploymentType.A, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Vínculo anterior con una Entidad del Grupo Empresarial", EmploymentType.A, parentId: parentId);

            parentId = AddEmploymentSummaryIfNotExists("Otras Causas de Altas", EmploymentType.A);
            AddEmploymentSummaryIfNotExists("Importación de Relación Laboral al nuevo Sistema de Nóminas", EmploymentType.A, parentId: parentId, reference: "A_SGNOM");

            //NOTA: Reubicaciones
            AddEmploymentSummaryIfNotExists("Cambio por ordenamiento monetario", EmploymentType.R, reference: "C2021");
            AddEmploymentSummaryIfNotExists("Cambio general salarial", EmploymentType.R, reference: "CS");
            AddEmploymentSummaryIfNotExists("Cambio de los elementos del cargo", EmploymentType.R, reference: "COCC");

            parentId = AddEmploymentSummaryIfNotExists("Cambios organizacionales de la Entidad", EmploymentType.R, reference: "C_ORG");
            AddEmploymentSummaryIfNotExists("Conversión de Plaza", EmploymentType.R, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Cambio de Cargo", EmploymentType.R, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Cambio de la Estructura de la Entidad", EmploymentType.R, parentId: parentId, reference: "C_ESTRUCTURA");
            AddEmploymentSummaryIfNotExists("Cambio de Area de Trabajo", EmploymentType.R, parentId: parentId);

            parentId = AddEmploymentSummaryIfNotExists("Cambio definitivo de Puesto de Trabajo", EmploymentType.R, reference: "CPT");
            AddEmploymentSummaryIfNotExists("Por Idoneidad", EmploymentType.R, parentId: parentId, reference: "CPT_IDONEIDAD");
            AddEmploymentSummaryIfNotExists("Para cumplir con Sanción Laboral", EmploymentType.R, parentId: parentId, reference: "CPT_SANCION_LABORAL");

            parentId = AddEmploymentSummaryIfNotExists("Cambio provisional de Puesto de Trabajo", EmploymentType.R, reference: "MP");
            AddEmploymentSummaryIfNotExists("Para cumplir con medida disciplinaria", EmploymentType.R, parentId: parentId, reference: "MP_MEDIDA");
            AddEmploymentSummaryIfNotExists("Por necesidad de la producción", EmploymentType.R, parentId: parentId, reference: "MP_NECESIDAD_PRODUCTIVA");
            AddEmploymentSummaryIfNotExists("Por Peritaje Médico", EmploymentType.R, parentId: parentId, reference: "MP_PERITAJE_MEDICO");

            parentId = AddEmploymentSummaryIfNotExists("Cambio de las adiciones salariales", EmploymentType.R, reference: "CPLUS");
            AddEmploymentSummaryIfNotExists("Pago de Categoría Científica", EmploymentType.R, parentId: parentId, reference: "C_GRADO");
            AddEmploymentSummaryIfNotExists("Pago de Homologación", EmploymentType.R, parentId: parentId, reference: "C_CERT");
            AddEmploymentSummaryIfNotExists("Pago de Horario Irregular", EmploymentType.R, parentId: parentId, reference: "C_HE");
            AddEmploymentSummaryIfNotExists("Pago de Diferencia Salarial", EmploymentType.R, parentId: parentId, reference: "C_DIF");

            AddEmploymentSummaryIfNotExists("Cambio de Turno y Régimen de Trabajo", EmploymentType.R, reference: "CTR");
            AddEmploymentSummaryIfNotExists("Prórroga del Movimiento Provisional", EmploymentType.R, reference: "PMP");
            AddEmploymentSummaryIfNotExists("Cancelación del Movimiento Provisional", EmploymentType.R, reference: "CMP");
            AddEmploymentSummaryIfNotExists("Culminación del Movimiento Provisional", EmploymentType.R, reference: "FMP");
            
            parentId = AddEmploymentSummaryIfNotExists("Culminación del período de Adiestramiento", EmploymentType.R, reference: "R_FIN_ADIESTRAMIENTO");
            AddEmploymentSummaryIfNotExists("Culminación del Servicio Social", EmploymentType.R, parentId: parentId);
            AddEmploymentSummaryIfNotExists("Continuación del Contrato Determinado para cumplir con el Servicio Social", EmploymentType.R, parentId: parentId);

            parentId = AddEmploymentSummaryIfNotExists("Otros Movimientos", EmploymentType.R, reference: "R_OTROS");
            AddEmploymentSummaryIfNotExists("Importación de Relación Laboral al nuevo Sistema de Nóminas", EmploymentType.R, parentId: parentId, reference: "R_SGNOM");
            AddEmploymentSummaryIfNotExists("Sub-sanación de errores en movimientos anteriores", EmploymentType.R, parentId: parentId, reference: "R_OTROS_SUBSANACION");
            AddEmploymentSummaryIfNotExists("Otras causas", EmploymentType.R, parentId: parentId, reference: "R_OTROS_GENERICO");

            //NOTA: Bajas
            //Clasificación: "Interés Personal", "Interés de la organización", "Fuerza Mayor", "Sin definir"
            parentId = AddEmploymentSummaryIfNotExists("Separación Definitiva", EmploymentType.B, acronym: "SD", classification: "Fuerza Mayor", fluctuation: false, reference: "SEPARACION_DEF");
            AddEmploymentSummaryIfNotExists("Por sentencia del Tribunal", EmploymentType.B, parentId: parentId, acronym: "SD", classification: "Fuerza Mayor", fluctuation: false, reference: "B_TRIBUNAL");
            
            AddEmploymentSummaryIfNotExists("Separación Definitiva del Sector", EmploymentType.B, acronym: "SS", classification: "Interés de la organización", fluctuation: false, reference: "SEPARACION_DEF_SECTOR");
            AddEmploymentSummaryIfNotExists("Salida del país", EmploymentType.B, acronym: "DSP", classification: "Interés Personal", fluctuation: false, reference: "DESERCIÓN");
            
            parentId = AddEmploymentSummaryIfNotExists("Solicitud Propia", EmploymentType.B, acronym: "SP", classification: "Interés Personal", fluctuation: true, reference: "B_SOLICITUD");
            AddEmploymentSummaryIfNotExists("Atención a familiares", EmploymentType.B, parentId: parentId, acronym: "SP", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Desacuerdos con métodos de dirección", EmploymentType.B, parentId: parentId, acronym: "SP", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Lejanía del Centro de Trabajo", EmploymentType.B, parentId: parentId, acronym: "SP", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Medidas disciplinarias aplicadas", EmploymentType.B, parentId: parentId, acronym: "SP", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Motivos salariales", EmploymentType.B, parentId: parentId, acronym: "SP", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Pocas perspectivas de superación", EmploymentType.B, parentId: parentId, acronym: "SP", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Sin Especificar", EmploymentType.B, parentId: parentId, acronym: "SP", classification: "Interés Personal", fluctuation: true);

            parentId = AddEmploymentSummaryIfNotExists("Sanciones Laborales", EmploymentType.B, acronym: "SL", classification: "Interés de la organización", fluctuation: true, reference: "SANCIÓN_LABORAL");
            AddEmploymentSummaryIfNotExists("Separación definitiva de la entidad", EmploymentType.B, parentId: parentId, acronym: "SL", classification: "Interés de la organización", fluctuation: true, reference: "SL_ENTIDAD");
            AddEmploymentSummaryIfNotExists("Separación definitiva del sector", EmploymentType.B, parentId: parentId, acronym: "SL", classification: "Interés de la organización", fluctuation: true, reference: "SL_SECTOR");

            parentId = AddEmploymentSummaryIfNotExists("Jubilación por Edad", EmploymentType.B, acronym: "JUB", classification: "Interés Personal", fluctuation: false, reference: "B_JUBILACION");
            AddEmploymentSummaryIfNotExists("Jubilación anticipada por edad", EmploymentType.B, parentId: parentId, acronym: "JUB", classification: "Interés Personal", fluctuation: false, reference: "B_JUBILACION_ANTICIPADA_EDAD");
            AddEmploymentSummaryIfNotExists("Arribó a la edad de jubilación (dentro del mismo año)", EmploymentType.B, parentId: parentId, acronym: "JUB", classification: "Interés Personal", fluctuation: false, reference: "B_JUBILACION_ANTICIPADA");
            AddEmploymentSummaryIfNotExists("Jubilación retrasada (posterior al año en que arribó)", EmploymentType.B, parentId: parentId, acronym: "JUB", classification: "Interés Personal", fluctuation: false, reference: "B_JUBILACION_RETRASADA");

            AddEmploymentSummaryIfNotExists("Jubilación por Enfermedad", EmploymentType.B, acronym: "JUB", classification: "Fuerza Mayor", fluctuation: false, reference: "B_JUBILACION_ENFERMEDAD");
            
            parentId = AddEmploymentSummaryIfNotExists("Movimientos Dirigidos", EmploymentType.B, acronym: "MD", classification: "Interés de la organización", fluctuation: false, reference: "B_DIRIGIDOS");
            AddEmploymentSummaryIfNotExists("Traslado a otra Entidad del Grupo Empresarial", EmploymentType.B, parentId: parentId, acronym: "MD", classification: "Interés de la organización", fluctuation: false, reference: "B_TRASPASO_ENTIDAD");
            AddEmploymentSummaryIfNotExists("Traslado a una Entidad fuera del Grupo Empresarial", EmploymentType.B, parentId: parentId, acronym: "MD", classification: "Interés de la organización", fluctuation: false, reference: "B_TRASPASO_FUERA");
            AddEmploymentSummaryIfNotExists("Traspaso de Actividades a otra Entidad", EmploymentType.B, parentId: parentId, acronym: "MD", classification: "Interés de la organización", fluctuation: false, reference: "B_TRASPASO");

            parentId = AddEmploymentSummaryIfNotExists("Ruptura anticipada de Contratos Determinados", EmploymentType.B, acronym: "RUP", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Solicitud propia del trabajador", EmploymentType.B, parentId: parentId, acronym: "RUP", classification: "Interés Personal", fluctuation: true);

            parentId = AddEmploymentSummaryIfNotExists("Cierre de Contratos Determinados", EmploymentType.B, acronym: "CCD", classification: "Interés de la organización", fluctuation: true, reference: "B_CIERRE_CONTRATOS");
            AddEmploymentSummaryIfNotExists("Conclusión de la labor pactada", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Pérdida de la Idoneidad Demostrada", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Incorporación del Trabajador al cual estaba sustituyendo", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Amortización de la Plaza que ocupaba en sustitución de un trabajador (antes de su incorporación)", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Sanción de privación de libertad por sentencia firme", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Falta de abastecimiento técnico material que impida cumplir la labor pactada", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Incumplimiento por el trabajador de las obligaciones contraÍdas en el contrato", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Vencimiento de los téminos pactados", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Otras causas", EmploymentType.B, parentId: parentId, acronym: "CCD", classification: "Interés de la organización", fluctuation: true);

            parentId = AddEmploymentSummaryIfNotExists("No aceptación de Curso de Re-calificación", EmploymentType.B, acronym: "NACR", classification: "Interés Personal", fluctuation: true, reference: "B_NO_ACEPTACION_CURSO");
            AddEmploymentSummaryIfNotExists("Trabajador Disponible", EmploymentType.B, parentId: parentId, acronym: "NACR", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Por Perdida de la Idoneidad", EmploymentType.B, parentId: parentId, acronym: "NACR", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Por Invalidez Parcial acreditada por peritaje médico", EmploymentType.B, parentId: parentId, acronym: "NACR", classification: "Interés Personal", fluctuation: true);

            parentId = AddEmploymentSummaryIfNotExists("No aceptación injustificada de Re-ubicación", EmploymentType.B, acronym: "NAR", classification: "Interés Personal", fluctuation: true, reference: "B_INJUSTIFICADA");
            AddEmploymentSummaryIfNotExists("Trabajador Disponible", EmploymentType.B, parentId: parentId, acronym: "NAR", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Por Perdida de la Idoneidad", EmploymentType.B, parentId: parentId, acronym: "NAR", classification: "Interés Personal", fluctuation: true);
            AddEmploymentSummaryIfNotExists("Por Invalidez Parcial acreditada por peritaje médico", EmploymentType.B, parentId: parentId, acronym: "NAR", classification: "Interés Personal", fluctuation: true);

            parentId = AddEmploymentSummaryIfNotExists("Baja temporal con derecho a retornar a la Entidad", EmploymentType.B, acronym: "TMP", classification: "Interés de la organización", fluctuation: false, reference: "B_TEMPORAL_CON_RETORNO");
            AddEmploymentSummaryIfNotExists("Salida para laborar en el Extranjero", EmploymentType.B, parentId: parentId, acronym: "TMP", classification: "Interés de la organización", fluctuation: false, reference: "B_TRABAJA_EXTRANJERO");
            AddEmploymentSummaryIfNotExists("Prestación de Servicios en otra Entidad", EmploymentType.B, parentId: parentId, acronym: "TMP", classification: "Interés de la organización", fluctuation: false, reference: "B_PRESTACION_SERVICIOS");

            parentId = AddEmploymentSummaryIfNotExists("Declaración de Disponibilidad del trabajador", EmploymentType.B, acronym: "DDT", classification: "Interés de la organización", fluctuation: true, reference: "B_DISPONIBLE");
            AddEmploymentSummaryIfNotExists("Re-ubicación definitiva fuera de la Entidad", EmploymentType.B, parentId: parentId, acronym: "DDT", classification: "Interés de la organización", fluctuation: true, reference: "B_DISPONIBLE_REUBICADO_FUERA");
            AddEmploymentSummaryIfNotExists("No existe la posibilidad de re-ubicarlo en otra plaza dentro de la entidad", EmploymentType.B, parentId: parentId, acronym: "DDT", classification: "Interés de la organización", fluctuation: true);
            AddEmploymentSummaryIfNotExists("No existe la posibilidad de envío a de cursos de re-calificación o rehabilitación", EmploymentType.B, parentId: parentId, acronym: "DDT", classification: "Interés de la organización", fluctuation: true);

            AddEmploymentSummaryIfNotExists("Extinción de la Entidad laboral", EmploymentType.B, acronym: "EXT", classification: "Fuerza Mayor", fluctuation: false, reference: "B_EXTINCION_ENTIDAD");
            
            parentId = AddEmploymentSummaryIfNotExists("Pérdida de la Idoneidad Demostrada", EmploymentType.B, acronym: "PID", classification: "Interés de la organización", fluctuation: true, reference: "B_IDONEIDAD");
            AddEmploymentSummaryIfNotExists("No existe la posibilidad de re-ubicarlo en otra plaza dentro de la entidad", EmploymentType.B, parentId: parentId, acronym: "PID", classification: "Interés de la organización", fluctuation: true, reference: "B_IDONEIDAD_NO_REUBICADO");
            AddEmploymentSummaryIfNotExists("No existe posibilidad de enviarlo a cursos de re-calificación", EmploymentType.B, parentId: parentId, acronym: "PID", classification: "Interés de la organización", fluctuation: true, reference: "B_IDONEIDAD_NO_RECALIFICACION");

            parentId = AddEmploymentSummaryIfNotExists("Invalidez Parcial acreditada por peritaje médico", EmploymentType.B, acronym: "INV", classification: "Interés de la organización", fluctuation: false, reference: "B_INVALIDEZ_PARCIAL");
            AddEmploymentSummaryIfNotExists("No existe la posibilidad de re-ubicarlo en otra plaza dentro de la entidad", EmploymentType.B, parentId: parentId, acronym: "INV", classification: "Interés de la organización", fluctuation: false, reference: "B_INV_PAR_NO_REUBICADO");

            parentId = AddEmploymentSummaryIfNotExists("No reintegración del trabajador al Trabajo", EmploymentType.B, acronym: "NRT", classification: "Interés de la organización", fluctuation: true, reference: "B_NO_REINTEGRACION");
            AddEmploymentSummaryIfNotExists("Cumplimiento del plazo de una licencia no retribuida", EmploymentType.B, parentId: parentId, acronym: "NRT", classification: "Interés de la organización", fluctuation: true, reference: "B_LICENCIA_NO_RETRIBUIDA");
            AddEmploymentSummaryIfNotExists("Término de una prestación social", EmploymentType.B, parentId: parentId, acronym: "NRT", classification: "Interés de la organización", fluctuation: true, reference: "B_PRESTACION_SOCIAL");

            parentId = AddEmploymentSummaryIfNotExists("Culminación del Período de Adiestramiento", EmploymentType.B, acronym: "ADI", classification: "Interés de la organización", fluctuation: false, reference: "B_ADIESTRAMIENTO");
            AddEmploymentSummaryIfNotExists("No está apto para desempeñar la labor pactada", EmploymentType.B, parentId: parentId, acronym: "ADI", classification: "Interés de la organización", fluctuation: false, reference: "B_NO_APTO");

            parentId = AddEmploymentSummaryIfNotExists("Otras causas", EmploymentType.B, acronym: "OM", classification: "Fuerza Mayor", fluctuation: false, reference: "B_OTRAS");
            AddEmploymentSummaryIfNotExists("Fallecimiento", EmploymentType.B, parentId: parentId, acronym: "OM", classification: "Fuerza Mayor", fluctuation: false, reference: "B_FALLECIMIENTO");
            AddEmploymentSummaryIfNotExists("Salida definitiva del país", EmploymentType.B, parentId: parentId, acronym: "OM", classification: "Fuerza Mayor", fluctuation: false, reference: "B_SALIDA_PAIS");
            AddEmploymentSummaryIfNotExists("Privación de libertad por más de 6 meses", EmploymentType.B, parentId: parentId, acronym: "OM", classification: "Fuerza Mayor", fluctuation: false, reference: "B_PRIVACION_LIBERTAD");
            AddEmploymentSummaryIfNotExists("Incorporación al SMG", EmploymentType.B, parentId: parentId, acronym: "OM", classification: "Fuerza Mayor", fluctuation: false, reference: "B_SMG");
            AddEmploymentSummaryIfNotExists("Deserción", EmploymentType.B, parentId: parentId, acronym: "OM", classification: "Interés Personal", fluctuation: false, reference: "B_DESERCION");
            AddEmploymentSummaryIfNotExists("Otras", EmploymentType.B, parentId: parentId, acronym: "OM", classification: "Interés Personal", fluctuation: false, reference: "B_OTRAS_PERSONAL");

            _logger.Info("Employment complementary related data created.");
        }

        private void CreateDriverLicenses()
        {
            AddDriverLicenseIfNotExists("Ciclomotor", "A1");
            AddDriverLicenseIfNotExists("Motocicleta", "A");
            AddDriverLicenseIfNotExists("Automóvil (hasta 3500kg)", "B");
            AddDriverLicenseIfNotExists("Camión (hasta 7500kg)", "C1");
            AddDriverLicenseIfNotExists("Camión (más de 7500kg)", "C");
            AddDriverLicenseIfNotExists("MicroBus (hasta 17 asientos)", "D1");
            AddDriverLicenseIfNotExists("Ómnibus (más de 17 asientos)", "D");
            AddDriverLicenseIfNotExists("Articulado", "E");
            AddDriverLicenseIfNotExists("Agro Industrial y de la Construcción", "F");

            _logger.Info("Driver Licenses related data created.");
        }

        private void CreateScholarshipLevels()
        {
            AddScholarshipLevelIfNotExists("Sin Especificar", "", ScopeData.Company);
            AddScholarshipLevelIfNotExists("Técnico Superior", "TS", ScopeData.Company, 0.75M);
            AddScholarshipLevelIfNotExists("Medio Superior", "MS", ScopeData.Company, 0.50M);
            AddScholarshipLevelIfNotExists("Nivel Medio", "NM", ScopeData.Company, 0.25M);
            AddScholarshipLevelIfNotExists("Ninguno", "", ScopeData.Personal);
            AddScholarshipLevelIfNotExists("Nivel Primario", "PRI", ScopeData.Personal, 0.10M);
            AddScholarshipLevelIfNotExists("Nivel Secundario", "SB", ScopeData.Personal, 0.20M);
            AddScholarshipLevelIfNotExists("Nivel PreUniversitario", "PU", ScopeData.Personal, 0.50M);
            AddScholarshipLevelIfNotExists("Nivel Medio", "TM", ScopeData.Personal, 0.25M);
            AddScholarshipLevelIfNotExists("Nivel Medio Superior", "NMS", ScopeData.Personal, 0.75M);
            AddScholarshipLevelIfNotExists("Nivel Superior", "NS", ScopeData.Personal, 1);
            AddScholarshipLevelIfNotExists("Máster en Ciencias", "MSC", ScopeData.Personal, 1.5M);
            AddScholarshipLevelIfNotExists("Doctor en Ciencias", "DRC", ScopeData.Personal, 2);

            _logger.Info("Scholarship Levels related data created.");
        }

        private void CreateSignReferences()
        {
            if (_context.SignOnDocuments.IgnoreQueryFilters().Any(s => s.Code == "-"))
                return;

            var companyId = KontecgCoreConsts.MultiCompanyEnabled ? MultiCompanyConsts.DefaultCompanyId : 1;
            _context.SignOnDocuments.Add(new SignOnDocument("-", "Firma no válida", "00000000000", "", 0)
            {
                CompanyId = companyId
            });

            _context.SaveChanges();
            _logger.Info("Signing documents details added.");
        }

        private void AddCriteriaIfNotExists(string description)
        {
            if (_context.PerformanceSummaries.IgnoreQueryFilters().Any(s => s.DisplayName == description))
                return;

            _context.PerformanceSummaries.Add(new PerformanceSummary(description));
            _context.SaveChanges();
        }

        private int? AddEmploymentSummaryIfNotExists(string description, EmploymentType type, string? acronym = null,
            int? parentId = null, string? classification = null, bool? fluctuation = null, string? reference = null)
        {
            if (_context.EmploymentSummaries.IgnoreQueryFilters().Any(s =>
                    s.DisplayName == description
                    && s.Acronym == acronym
                    && s.Type == type
                    && s.ParentId == parentId))

                return _context.EmploymentSummaries.IgnoreQueryFilters().First(s =>
                    s.DisplayName == description
                    && s.Acronym == acronym
                    && s.Type == type
                    && s.ParentId == parentId)?.Id;

            var employmentSummary = new EmploymentSummary(description, type, acronym, parentId, classification,
                fluctuation, reference);

            _context.EmploymentSummaries.Add(employmentSummary);
            _context.SaveChanges();
            return employmentSummary.Id;
        }

        private void AddScholarshipLevelIfNotExists(string description, string acronym, ScopeData ambient,
            decimal weight = 0)
        {
            if (_context.ScholarshipLevelDefinitions.IgnoreQueryFilters().Any(s =>
                    s.DisplayName == description
                    && s.Acronym == acronym
                    && s.Scope == ambient))
                return;

            _context.ScholarshipLevelDefinitions.Add(new ScholarshipLevelDefinition(description, acronym, ambient,
                weight));
            _context.SaveChanges();
        }

        private void AddDegreeDefinitionIfNotExists(string displayName)
        {
            if (_context.DegreeDefinitions.IgnoreQueryFilters().Any(s =>
                    s.DisplayName == displayName.ToUpperInvariant()))
                return;

            _context.DegreeDefinitions.Add(new DegreeDefinition(displayName));
            _context.SaveChanges();
        }

        private void AddDriverLicenseIfNotExists(string displayName, string category)
        {
            if (_context.DriverLicenseDefinitions.IgnoreQueryFilters().Any(s =>
                    s.DisplayName == displayName
                    && s.Category == category))
                return;

            _context.DriverLicenseDefinitions.Add(new DriverLicenseDefinition()
            {
                Category = category,
                DisplayName = displayName,
            });
            _context.SaveChanges();
        }

        private void AddMilitaryLocationIfNotExists(string displayName)
        {
            if (_context.MilitaryLocationDefinitions.IgnoreQueryFilters().Any(s =>
                    s.DisplayName == displayName.ToUpperInvariant()))
                return;

            _context.MilitaryLocationDefinitions.Add(new MilitaryLocationDefinition(displayName));
            _context.SaveChanges();
        }

        private void AddLawPenaltyDefinitionIfNotExists(string displayName, string description)
        {
            if (_context.LawPenaltyDefinitions.IgnoreQueryFilters().Any(s =>
                    s.DisplayName == displayName.ToUpperInvariant()))
                return;

            _context.LawPenaltyDefinitions.Add(new LawPenaltyDefinition(displayName, description));
            _context.SaveChanges();
        }

        private void AddLawPenaltyCauseDefinitionIfNotExists(string displayName, string legal)
        {
            if (_context.LawPenaltyCauseDefinitions.IgnoreQueryFilters().Any(s =>
                    s.DisplayName == displayName.ToUpperInvariant()))
                return;

            _context.LawPenaltyCauseDefinitions.Add(new LawPenaltyCauseDefinition(displayName, legal));
            _context.SaveChanges();
        }

        private class PersonWithAccount
        {
            public virtual string IdentityCard { get; set; }

            public virtual PersonAccount[] PersonalAccounts { get; set; }
        }
    }
}
