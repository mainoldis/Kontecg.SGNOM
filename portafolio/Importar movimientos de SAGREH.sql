CREATE TABLE #movimientos(IdMov INT, IdMovPredecesor INT, DocumentDefinitionId INT, MadeOn DATETIME2(7), EffectiveSince DATETIME2(7), Exp INT, PersonId INT, Code NVARCHAR(11), GroupId UNIQUEIDENTIFIER, EmployeeSalaryForm TINYINT, CompanyId INT, Contract NVARCHAR(1), ContractSubType NVARCHAR(1),Type NVARCHAR(1), SubType NVARCHAR(3), PreviousId BIGINT, OrganizationUnitId BIGINT, [Level] INT, WorkPlacePaymentCode NVARCHAR(8), FirstLevelCode NVARCHAR(12),  FirstLevelDisplayName NVARCHAR(128), SecondLevelCode NVARCHAR(12), SecondLevelDisplayName NVARCHAR(128), ThirdLevelCode NVARCHAR(12), ThirdLevelDisplayName NVARCHAR(128), CenterCost INT, ComplexityGroup NVARCHAR(8), Occupation NVARCHAR(250), OccupationCode NVARCHAR(8), Responsibility NVARCHAR(150), OccupationCategory NVARCHAR(1), WorkShiftId INT, Salary DECIMAL(10,2), TotalSalary DECIMAL(10,2), RatePerHour DECIMAL(28,24), SummaryId INT, ExtraSummary NVARCHAR(500), ExpirationDate DATETIME2(7), HasSalary BIT, ByAssignment BIT, ByOfficial BIT, Review INT, CreationTime DATETIME2(7));  

DECLARE @DocumentDefinitionId INT;
DECLARE @MOTIVO_A INT, @MOTIVO_R INT;
SELECT @DocumentDefinitionId = Id FROM Kontecg.gen.document_definitions WHERE Reference = 'DOCNOM';
SELECT @MOTIVO_A = Id FROM SGNOM.rel.employment_summaries WHERE Reference = 'A_SGNOM';
SELECT @MOTIVO_R = Id FROM SGNOM.rel.employment_summaries WHERE Reference = 'R_SGNOM';

INSERT INTO #movimientos 
SELECT DISTINCT RELMovimientos.IdMov,
	   CASE WHEN RELMovimientos.SubTipoMov IN ('MP', 'PMP') THEN RELMovPredecesor.IdMovPredecesor ELSE NULL END AS IdMovPredecesor,
       RELMovimientos.IdMov AS DocumentDefinitionId, 
       RELMovimientos.FechaConf AS MadeOn,  
       RELMovimientos.FechaEfectMov AS EffectiveSince, 
       RELMovimientosHist.NoExp AS Exp, 
       ISNULL(Kontecg.docs.persons.Id, 0) AS PersonId, 
       RELMovimientos.NumeroMov AS Code, 
       RELMovimientos.IdEmpleo AS GroupId,         
       2 AS EmployeeSalaryForm, 
	   1 AS CompanyId,    
       CASE RELMovimientosDGral.TipoCont WHEN 'I' THEN 'I' ELSE 'D' END AS Contract, 
	   CASE RELMovimientosDGral.TipoCont WHEN 'I' THEN 'I' WHEN 'A' THEN 'A' WHEN 'P' THEN 'P' ELSE 'D' END AS ContractSubType, 
       RELMovimientos.TipoMov AS Type, 
       RELMovimientos.SubTipoMov AS SubType, 
       NULL AS PreviousId, 
       --ISNULL(Kontecg.est.organization_units.Id, 1) AS OrganizationUnitId,
	   1 AS OrganizationUnitId,
	   Kontecg.est.workplace_classifications.Level AS [Level],
       RTRIM(LTRIM(GEN_AREASPAGO.Codigo)) AS WorkPlacePaymentCode,
	   n1.Codigo AS FirstLevelCode,
       RELMovimientosHist.AreaN1 AS FirstLevelDisplayName,    
	   RTRIM(LTRIM(GEN_AREASPAGO.Codigo)) AS SecondLevelCode, 
       RTRIM(LTRIM(GEN_AREASPAGO.Descripcion)) AS SecondLevelDisplayName,
	   RELMovimientosHist.CodArea AS ThirdLevelCode,
       RELMovimientosHist.Area AS ThirdLevelDisplayName, 
       RELMovimientosHist.CCosto AS CenterCost, 
       RTRIM(LTRIM(REPLACE(RELMovimientosHist.GrupoSalarial,'.', ''))) AS ComplexityGroup, 
	   --RELMovimientosHist.GrupoSalarial AS ComplexityGroup, 
       RELMovimientosHist.Cargo AS Occupation, 
       RELMovimientosHist.CodCargo AS OccupationCode,    
       CASE WHEN MGResponsabilidadCargo.Responsabilidad IN ('Jefe de Brigada', 'Especialista Principal', 'Jefe de Taller') THEN MGResponsabilidadCargo.Responsabilidad ELSE 'Del Cargo' END AS Responsibility,        
       RTRIM(LTRIM(RELMovimientosHist.CategOcup)) AS OccupationCategory, 
       Sgnom.gen.work_shifts.Id AS WorkShiftId, 
       MGGrupoSalarial.SalarioEscala AS Salary, 
       RELMovimientosDGral.SalarioTotal AS TotalSalary, 
       (MGGrupoSalarial.SalarioEscala / 190.6) AS RatePerHour,        
       CASE RELMovimientos.TipoMov WHEN 'A' THEN @MOTIVO_A WHEN 'R' THEN @MOTIVO_R END AS SummaryId, 
       RELMovimientos.Observaciones AS ExtraSummary,         
       CASE WHEN RELMovimientos.SubTipoMov IN('MP', 'PMP') THEN RELMovProvisional.FechaVenc ELSE RELMovDeter.FechaVenc END AS ExpirationDate,    
       1 AS HasSalary, 
       0 AS ByAssignment, 
       CASE RTRIM(LTRIM(RELMovimientosHist.CategOcup)) WHEN 'C' THEN 1 ELSE 0 END AS ByOfficial, 
       2 AS Review, 
       GETDATE() AS CreationTime 
FROM   iAraGlobal_ECG.dbo.RELMovimientos INNER JOIN        
       iAraGlobal_ECG.dbo.PERPersonas ON RELMovimientos.IdPersona = PERPersonas.IdPersona INNER JOIN        
       iAraGlobal_ECG.dbo.RELMovimientosHist ON RELMovimientos.IdMov = RELMovimientosHist.IdMov INNER JOIN   
       iAraGlobal_ECG.dbo.RELMovimientosDGral ON RELMovimientosHist.IdMov = RELMovimientosDGral.IdMovimiento INNER JOIN        
	   iAraGlobal_ECG.dbo.ESTAreasTrabajo ON RELMovimientosHist.IdArea = ESTAreasTrabajo.IdAreasTrabajo INNER JOIN
	   iAraGlobal_ECG.dbo.ESTAreasTrabajo n1 ON RELMovimientosHist.IdAreaN1 = n1.IdAreasTrabajo INNER JOIN
	   iAraGlobal_ECG.dbo.GEN_AREASPAGO ON GEN_AREASPAGO.IdAreasPago = ESTAreasTrabajo.IdAreaPago INNER JOIN
       iAraGlobal_ECG.dbo.MGTurnosLaborales ON RELMovimientosDGral.IdTurno = MGTurnosLaborales.IdTurno INNER JOIN        
       iAraGlobal_ECG.dbo.MGEspecifMotivosMvtos ON MGEspecifMotivosMvtos.IdEspecifMvto = RELMovimientos.IdEspecifMov INNER JOIN 
	   iAraGlobal_ECG.dbo.MGGrupoSalarial ON RELMovimientosDGral.IdGrupoSalarial = MGGrupoSalarial.IdGrupoSalarial INNER JOIN 
       iAraGlobal_ECG.dbo.MGResponsabilidadCargo ON RELMovimientosDGral.IdResponsabilidad = MGResponsabilidadCargo.IdResponsabilidadCargo LEFT OUTER JOIN    
	   iAraGlobal_ECG.dbo.RELMovPredecesor ON RELMovimientos.IdMov = RELMovPredecesor.IdMov LEFT OUTER JOIN 
       iAraGlobal_ECG.dbo.RELMovDeter ON RELMovimientosDGral.IdMovimiento = RELMovDeter.IdMov LEFT OUTER JOIN    
       iAraGlobal_ECG.dbo.RELMovProvisional ON RELMovimientosDGral.IdMovimiento = RELMovProvisional.IdMov LEFT OUTER JOIN    
       Kontecg.docs.persons ON Kontecg.docs.persons.IdentityCard = PERPersonas.CI COLLATE SQL_Latin1_General_CP1_CI_AS LEFT OUTER JOIN    
       Kontecg.est.organization_units ON Kontecg.est.organization_units.DisplayName = LTRIM(RTRIM(RELMovimientosHist.AreaN1)) COLLATE SQL_Latin1_General_CP1_CI_AS LEFT OUTER JOIN    
       Kontecg.est.workplace_classifications ON Kontecg.est.organization_units.ClassificationId = Kontecg.est.workplace_classifications.Id LEFT OUTER JOIN    
       Sgnom.gen.work_shifts ON Sgnom.gen.work_shifts.IsActive = 1 AND Sgnom.gen.work_shifts.DisplayName = CASE WHEN MGTurnosLaborales.Descripcion NOT IN ('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L') THEN 'N' ELSE LTRIM(RTRIM(MGTurnosLaborales.Descripcion)) END COLLATE SQL_Latin1_General_CP1_CI_AS LEFT OUTER JOIN        
       Sgnom.rel.employment_summaries ON Sgnom.rel.employment_summaries.DisplayName = LTRIM(RTRIM(MGEspecifMotivosMvtos.EspecificacionMvto)) COLLATE SQL_Latin1_General_CP1_CI_AS 
WHERE  RELMovimientos.Procesado = 1 AND (Kontecg.est.workplace_classifications.Level IS NULL OR Kontecg.est.workplace_classifications.Level = 3) AND RELMovimientos.FechaEfectMov >= '20210101'
ORDER BY PersonId, EffectiveSince;

--SELECT * FROM #movimientos WHERE IdMov = 101034867

SELECT #movimientos.*, 1 AS [Index], 1 AS rn INTO #indeterminados 
FROM #movimientos RIGHT OUTER JOIN iAraGlobal_ECG.dbo.RELRelacionLaboral r ON r.IdMov = #movimientos.IdMov 
WHERE SubType NOT IN ('MP', 'PMP') ORDER BY #movimientos.IdMov;

WITH prov AS (
	SELECT #movimientos.*, 1 AS [Index] 
	FROM #movimientos 
	INNER JOIN iAraGlobal_ECG.dbo.RELRelacionLaboral r WITH (NOLOCK) ON r.IdMov = #movimientos.IdMov WHERE SubType IN ('MP', 'PMP')
	UNION ALL 
	SELECT #movimientos.*, [Index] + 1 
	FROM #movimientos 
	INNER JOIN prov ON prov.IdMovPredecesor = #movimientos.IdMov
)

SELECT t.IdMov, t.IdMovPredecesor, t.DocumentDefinitionId, t.MadeOn, t.EffectiveSince, t.[Exp], t.PersonId, t.Code, t.GroupId, t.EmployeeSalaryForm, t.CompanyId, t.[Contract], t.ContractSubType, t.[Type], t.SubType, t.PreviousId, t.OrganizationUnitId, t.[Level], t.WorkPlacePaymentCode, t.FirstLevelCode, t.FirstLevelDisplayName, t.SecondLevelCode,t. SecondLevelDisplayName, t.ThirdLevelCode, t.ThirdLevelDisplayName, t.CenterCost, t.ComplexityGroup, t.Occupation, t.OccupationCode, t.Responsibility, t.OccupationCategory, t.WorkShiftId, t.Salary, t.TotalSalary, t.RatePerHour, t.SummaryId, t.ExtraSummary, t.ExpirationDate, t.HasSalary, t.ByAssignment, t.ByOfficial, t.Review, t.CreationTime, [Index], rn 
INTO #provisionales
FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY GroupId ORDER BY [Index] DESC) as rn FROM prov) t LEFT OUTER JOIN #movimientos ON #movimientos.IdMov = t.IdMovPredecesor;

SELECT * INTO #relacion_laboral FROM #provisionales UNION SELECT * FROM #indeterminados;

SELECT * FROM #relacion_laboral;

INSERT INTO SGNOM.rel.employment_documents(DocumentDefinitionId, MadeOn, EffectiveSince, [Exp], PersonId, Code, GroupId, EmployeeSalaryForm, CompanyId, [Contract], ContractSubType, [Type], SubType, PreviousId, OrganizationUnitId, WorkPlacePaymentCode, FirstLevelCode, FirstLevelDisplayName, SecondLevelCode, SecondLevelDisplayName, ThirdLevelCode, ThirdLevelDisplayName, CenterCost, ComplexityGroup, Occupation, OccupationCode, Responsibility, OccupationCategory, WorkShiftId, Salary, TotalSalary, RatePerHour, SummaryId, ExtraSummary, ExpirationDate, HasSalary, ByAssignment, ByOfficial, Review, CreationTime) 
SELECT DocumentDefinitionId, MadeOn, EffectiveSince, [Exp], PersonId, Code, GroupId, EmployeeSalaryForm, CompanyId, [Contract], ContractSubType, [Type], SubType, PreviousId, OrganizationUnitId, WorkPlacePaymentCode, FirstLevelCode, FirstLevelDisplayName, SecondLevelCode, SecondLevelDisplayName, ThirdLevelCode, ThirdLevelDisplayName, CenterCost, ComplexityGroup, Occupation, OccupationCode, Responsibility, OccupationCategory, WorkShiftId, Salary, TotalSalary, RatePerHour, SummaryId, ExtraSummary, ExpirationDate, HasSalary, ByAssignment, ByOfficial, Review, CreationTime
FROM #relacion_laboral;

UPDATE SGNOM.rel.employment_documents SET PreviousId = t.PreviousId
FROM
(SELECT r.Id, p.Id AS PreviousId FROM SGNOM.rel.employment_documents r INNER JOIN #relacion_laboral ON r.DocumentDefinitionId = #relacion_laboral.IdMov LEFT OUTER JOIN SGNOM.rel.employment_documents p ON p.DocumentDefinitionId = #relacion_laboral.IdMovPredecesor) t
WHERE t.Id = SGNOM.rel.employment_documents.Id;

--DELETE FROM Sgnom.rel.employment_pluses;

INSERT INTO Sgnom.rel.employment_pluses(EmploymentId, PlusDefinitionId, Amount, RatePerHour, CompanyId, CreationTime) 
SELECT employment_documents.Id AS EmploymentId, 
	   plus_definitions.Id AS PlusDefinitionId, 
	   RELMovAdiciones.Monto AS Amount,  
	   (RELMovAdiciones.Monto / 190.6) AS RatePerHour, 
	   1 AS CompanyId, 
	   GETDATE() AS CreationTime 
FROM   iAraGlobal_ECG.dbo.RELMovAdiciones INNER JOIN    
	   iAraGlobal_ECG.dbo.MGTiposAdiciones ON MGTiposAdiciones.IdAdiciones = RELMovAdiciones.IdAdiciones INNER JOIN    
	   #relacion_laboral ON #relacion_laboral.IdMov = RELMovAdiciones.IdMov LEFT OUTER JOIN    
	   Sgnom.rel.employment_documents ON employment_documents.DocumentDefinitionId = #relacion_laboral.IdMov LEFT OUTER JOIN     
	   Sgnom.sal.plus_definitions ON plus_definitions.[Name] = MGTiposAdiciones.Abrev COLLATE SQL_Latin1_General_CP1_CI_AS;    
   
UPDATE Sgnom.rel.employment_documents SET TotalSalary = Salary + t.Plus 
FROM (SELECT EmploymentId, CompanyId, SUM(employment_pluses.Amount) AS Plus FROM Sgnom.rel.employment_pluses GROUP BY EmploymentId, CompanyId) t 
WHERE Sgnom.rel.employment_documents.Id = t.EmploymentId AND Sgnom.rel.employment_documents.CompanyId = t.CompanyId  

--UPDATE SGNOM.rel.employment_documents SET DocumentDefinitionId = @DocumentDefinitionId;

DROP TABLE #movimientos;
DROP TABLE #provisionales;
DROP TABLE #indeterminados;
DROP TABLE #relacion_laboral;