SELECT REL_Templates.Id AS EntryId, REL_TemplateDocuments.Id AS DocId, Kontecg.dbo.KontecgCompanies.Organism, Kontecg.dbo.KontecgCompanies.Name AS Company, Kontecg.dbo.KontecgCompanies.Reup, Kontecg.dbo.KontecgOrganizationUnits.Code AS WorkPlaceCode, Kontecg.dbo.KontecgOrganizationUnits.DisplayName AS WorkPlace, REL_Templates.CenterCost, REL_Templates.EmployeeSalaryForm, REL_Occupations.Code AS OccupationCode, CASE REL_Responsibilities.NormalizedDescription WHEN 'DEL CARGO' THEN REL_Occupations.DisplayName ELSE (REL_Occupations.DisplayName + ' (' + REL_Responsibilities.DisplayName + ')') END AS Occupation, REL_OccupationalCategories.Code AS Category, SAL_ComplexityGroups.[Group], SAL_ComplexityGroups.[BaseSalary], PER_ScholarshipLevelDefinitions.Acronym AS Requirement, REL_WorkShifts.DisplayName AS WorkShift, REL_Templates.Approved
FROM   REL_Templates INNER JOIN
	   REL_TemplateDocuments ON REL_TemplateDocuments.Id = REL_Templates.DocumentId INNER JOIN
	   Kontecg.dbo.KontecgCompanies ON Kontecg.dbo.KontecgCompanies.Id = REL_TemplateDocuments.CompanyId INNER JOIN
	   Kontecg.dbo.KontecgOrganizationUnits ON Kontecg.dbo.KontecgOrganizationUnits.Id = REL_Templates.OrganizationUnitId INNER JOIN
	   REL_Occupations ON REL_Occupations.Id = REL_Templates.OccupationId INNER JOIN
	   REL_OccupationalCategories ON REL_OccupationalCategories.Id = REL_Templates.CategoryId INNER JOIN
	   SAL_ComplexityGroups ON SAL_ComplexityGroups.Id = REL_Templates.GroupId INNER JOIN
	   REL_Responsibilities ON REL_Responsibilities.Id = REL_Templates.ResponsibilityId LEFT OUTER JOIN
	   PER_ScholarshipLevelDefinitions ON PER_ScholarshipLevelDefinitions.Id = REL_Templates.ScholarshipLevelId INNER JOIN
	   REL_WorkShifts ON REL_WorkShifts.Id = REL_Templates.WorkShiftId


ORDER BY Kontecg.dbo.KontecgOrganizationUnits.[Order], SAL_ComplexityGroups.Id DESC
	   