SELECT Resumen.WorkPlace, ISNULL([C], 0) AS C, ISNULL([T],0) AS T, ISNULL([A],0) AS A, ISNULL([S],0) AS S, ISNULL([O],0) AS O, ISNULL([C], 0) + ISNULL([T],0) + ISNULL([A],0) + ISNULL([S],0) + ISNULL([O],0) AS Total FROM
(SELECT REL_OccupationalCategories.Code AS Category, Kontecg.dbo.KontecgOrganizationUnits.DisplayName AS WorkPlace, REL_Templates.Approved
FROM   REL_Templates INNER JOIN
	   REL_TemplateDocuments ON REL_TemplateDocuments.Id = REL_Templates.DocumentId INNER JOIN
	   Kontecg.dbo.KontecgCompanies ON Kontecg.dbo.KontecgCompanies.Id = REL_TemplateDocuments.CompanyId INNER JOIN
	   Kontecg.dbo.KontecgOrganizationUnits ON Kontecg.dbo.KontecgOrganizationUnits.Id = REL_Templates.OrganizationUnitId INNER JOIN
	   REL_Occupations ON REL_Occupations.Id = REL_Templates.OccupationId INNER JOIN
	   REL_OccupationalCategories ON REL_OccupationalCategories.Id = REL_Templates.CategoryId INNER JOIN
	   SAL_ComplexityGroups ON SAL_ComplexityGroups.Id = REL_Templates.GroupId INNER JOIN
	   REL_Responsibilities ON REL_Responsibilities.Id = REL_Templates.ResponsibilityId LEFT OUTER JOIN
	   PER_ScholarshipLevelDefinitions ON PER_ScholarshipLevelDefinitions.Id = REL_Templates.ScholarshipLevelId INNER JOIN
	   REL_WorkShifts ON REL_WorkShifts.Id = REL_Templates.WorkShiftId) t
PIVOT (
	SUM(t.Approved)
	FOR t.Category IN ([C], [T], [A], [S], [O])
) AS Resumen