update Sgnom.rel.employment_documents set OrganizationUnitId = ou.Id
from Sgnom.rel.employment_documents e
	inner join Migration.dbo.organization_units_migration mig ON e.ThirdLevelCode = mig.Origen
	inner join Kontecg.est.organization_units ou ON mig.Destino = ou.Code

update SGNOM.est.templates set OrganizationUnitId = ou.Id
from Kontecg.est.organization_units ou
where SGNOM.est.templates.OrganizationUnitCode = ou.Code

update SGNOM.est.template_job_positions set OrganizationUnitId = ou.Id
from Kontecg.est.organization_units ou
where SGNOM.est.template_job_positions.OrganizationUnitCode = ou.Code

--actualiza el indice de chapas usadas
merge SGNOM.rel.employment_indexes AS t using (select distinct convert(int, d.NoExp) AS [Exp], UPPER(p.Nombre + ' ' + p.Apellido1 + ' ' + p.Apellido2) AS Person, CASE WHEN convert(int, d.NoExp) < 40000 THEN 'I' ELSE 'D' END AS [Contract], CASE WHEN convert(int, d.NoExp) < 40000 THEN 'I' WHEN convert(int, d.NoExp) >= 40000 AND convert(int, d.NoExp) < 60000 THEN 'D' WHEN convert(int, d.NoExp) >= 60000 AND convert(int, d.NoExp) < 70000 THEN 'A' ELSE 'P' END AS ContractSubType from iAraGlobal_ECG.dbo.RELMovimientosHist d inner join iAraGlobal_ECG.dbo.RELMovimientos m on d.IdMov = m.IdMov inner join iAraGlobal_ECG.dbo.PERPersonas p on m.IdPersona = p.IdPersona where m.Procesado = 1 and d.IdArea <> 101078) AS o
on o.[Exp] = t.[Exp] and t.CompanyId = 1
when matched then update set t.Note = o.Person, t.[Contract] = o.[Contract], t.[Group] = o.ContractSubType
when not matched by target then insert (Id, [Exp], [Contract], [Group], Note, CompanyId, CreationTime) values(newid(), o.[Exp], o.[Contract], o.ContractSubType, o.Person, 1, GETDATE())
output deleted.*, $action, inserted.*;


merge SGNOM.rel.employment_indexes AS t using (select distinct d.[Exp], d.PersonId, d.CompanyId, UPPER(p.[Name] + ' '+ p.Surname + ' ' + p.Lastname) as Person, CASE WHEN convert(int, d.[Exp]) < 40000 THEN 'I' ELSE 'D' END AS [Contract], CASE WHEN convert(int, d.[Exp]) < 40000 THEN 'I' WHEN convert(int, d.[Exp]) >= 40000 AND convert(int, d.[Exp]) < 60000 THEN 'D' WHEN convert(int, d.[Exp]) >= 60000 AND convert(int, d.[Exp]) < 70000 THEN 'A' ELSE 'P' END AS ContractSubType from Sgnom.rel.employment_documents d inner join Kontecg.docs.persons p on d.PersonId = p.Id) AS o
on o.[Exp] = t.[Exp] and t.CompanyId = o.CompanyId
when matched then update set t.Note = o.Person, t.[Contract] = o.[Contract], t.[Group] = o.ContractSubType, t.CreationTime = GETDATE()
when not matched by target then insert (Id, [Exp], [Contract], [Group], Note, CompanyId, CreationTime) values(newid(), o.[Exp], o.[Contract], o.ContractSubType, o.Person, o.CompanyId, GETDATE())
output deleted.*, $action, inserted.*;

select distinct max(i.[Exp]) over (partition by i.[Group]) as [Exp], i.[Contract], i.[Group] from SGNOM.rel.employment_indexes i


--actualiza la ubicación de las personas a partir del movimiento de nómina y su nivel de responsabilidad
merge Kontecg.est.organization_units_persons AS t using (select PersonId, OrganizationUnitId, CompanyId from Sgnom.rel.employment_documents where employment_documents.PersonId <> 0) AS o
on o.PersonId = t.PersonId and t.CompanyId = o.CompanyId
when matched then update set t.OrganizationUnitId = o.OrganizationUnitId
when not matched by target then insert (PersonId, OrganizationUnitId, CompanyId, ResponsibilityLevel, CreationTime) values(o.PersonId, o.OrganizationUnitId, o.CompanyId, NULL, GETDATE())
output deleted.*, $action, inserted.*;

--actualiza la estructura y la asociación con sus centros de costos
merge Kontecg.est.organization_units_center_costs AS t using (select distinct e.OrganizationUnitId, e.CompanyId, cc.Id AS CenterCostId, cc.Code from Sgnom.rel.employment_documents e inner join Kontecg.cnt.center_cost_definitions cc on e.CenterCost = cc.Code where e.OrganizationUnitId <> 1) AS o
on o.OrganizationUnitId = t.OrganizationUnitId and t.CompanyId = o.CompanyId and o.CenterCostId = t.CenterCostId
when matched then update set t.CreationTime = GETDATE()
when not matched by target then insert (OrganizationUnitId, CompanyId, CenterCostId, CreationTime) values(o.OrganizationUnitId, o.CompanyId, o.CenterCostId, GETDATE())
output deleted.*, $action, inserted.*;

--Generar plan de vacaciones para el 2025
merge SGNOM.docs.person_holiday_schedules as t
using (
SELECT DISTINCT SGNOM.rel.employment_documents.PersonId, 2025 AS [Year], 0 AS JanuaryFirstFortnightCalendarDays, 0 AS JanuarySecondFortnightCalendarDays,0 AS FebruaryFirstFortnightCalendarDays,0 AS FebruarySecondFortnightCalendarDays,0 AS MarchFirstFortnightCalendarDays,0 AS MarchSecondFortnightCalendarDays,0 AS AprilFirstFortnightCalendarDays,0 AS AprilSecondFortnightCalendarDays,0 AS MayFirstFortnightCalendarDays,0 AS MaySecondFortnightCalendarDays,0 AS JuneFirstFortnightCalendarDays,0 AS JuneSecondFortnightCalendarDays,0 AS JulyFirstFortnightCalendarDays,0 AS JulySecondFortnightCalendarDays,0 AS AugustFirstFortnightCalendarDays,0 AS AugustSecondFortnightCalendarDays,0 AS SeptemberFirstFortnightCalendarDays,0 AS SeptemberSecondFortnightCalendarDays,0 AS OctoberFirstFortnightCalendarDays,0 AS OctoberSecondFortnightCalendarDays,0 AS NovemberFirstFortnightCalendarDays,0 AS NovemberSecondFortnightCalendarDays,0 AS DecemberFirstFortnightCalendarDays,0 AS DecemberSecondFortnightCalendarDays, GETDATE() AS CreationTime
FROM   SGNOM.rel.employment_documents
WHERE  (SGNOM.rel.employment_documents.PersonId <> 0)) as o
ON t.PersonId = o.PersonId and t.[Year] = o.[Year]
when matched then update set t.LastModificationTime = getdate()
when not matched by target then insert (PersonId, [Year], JanuaryFirstFortnightCalendarDays, JanuarySecondFortnightCalendarDays, FebruaryFirstFortnightCalendarDays, FebruarySecondFortnightCalendarDays, MarchFirstFortnightCalendarDays, MarchSecondFortnightCalendarDays, AprilFirstFortnightCalendarDays, AprilSecondFortnightCalendarDays, MayFirstFortnightCalendarDays, MaySecondFortnightCalendarDays, JuneFirstFortnightCalendarDays, JuneSecondFortnightCalendarDays, JulyFirstFortnightCalendarDays, JulySecondFortnightCalendarDays, AugustFirstFortnightCalendarDays, AugustSecondFortnightCalendarDays, SeptemberFirstFortnightCalendarDays, SeptemberSecondFortnightCalendarDays, OctoberFirstFortnightCalendarDays, OctoberSecondFortnightCalendarDays, NovemberFirstFortnightCalendarDays, NovemberSecondFortnightCalendarDays, DecemberFirstFortnightCalendarDays, DecemberSecondFortnightCalendarDays, CreationTime)
values(o.PersonId, o.[Year], o.JanuaryFirstFortnightCalendarDays, o.JanuarySecondFortnightCalendarDays,o.FebruaryFirstFortnightCalendarDays,o.FebruarySecondFortnightCalendarDays,o.MarchFirstFortnightCalendarDays,o.MarchSecondFortnightCalendarDays,o.AprilFirstFortnightCalendarDays,o.AprilSecondFortnightCalendarDays,o.MayFirstFortnightCalendarDays,o.MaySecondFortnightCalendarDays,o.JuneFirstFortnightCalendarDays,o.JuneSecondFortnightCalendarDays,o.JulyFirstFortnightCalendarDays,o.JulySecondFortnightCalendarDays,o.AugustFirstFortnightCalendarDays,o.AugustSecondFortnightCalendarDays,o.SeptemberFirstFortnightCalendarDays,o.SeptemberSecondFortnightCalendarDays,o.OctoberFirstFortnightCalendarDays,o.OctoberSecondFortnightCalendarDays,o.NovemberFirstFortnightCalendarDays,o.NovemberSecondFortnightCalendarDays,o.DecemberFirstFortnightCalendarDays,o.DecemberSecondFortnightCalendarDays, getdate())
output deleted.*, $action, inserted.*;