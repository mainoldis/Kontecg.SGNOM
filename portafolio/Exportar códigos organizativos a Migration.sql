INSERT INTO Migration.dbo.organization_units_migration(Origen, Destino)
SELECT DISTINCT docs.ThirdLevelCode AS Origen, ou.Code AS Destino
FROM   SGNOM.rel.employment_documents docs INNER JOIN Kontecg.est.organization_units ou ON ou.Id = docs.OrganizationUnitId
WHERE  docs.OrganizationUnitId <> 1