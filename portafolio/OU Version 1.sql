CREATE VIEW [est].[vw_job_positions]
AS
SELECT Kontecg.gen.companies.Organism AS Organism, Kontecg.gen.companies.CompanyName AS CompanyName, ou.Code AS Code, ou.DisplayName AS Area, ou.Description AS ShortName, wc.Description AS Kind, wc.[Level] AS [Level], 
       Kontecg.est.workplace_payments.Code AS PaymentGroupCode, Kontecg.est.workplace_payments.Description AS PaymentGroupName, positions.CenterCost, ao2.Code AS Code2, ao2.DisplayName AS Area2, ao2.[Level] AS Level2, ao2.Description AS Kind2, ao1.Code AS Code1, ao1.DisplayName AS Area1, ao1.[Level] AS Level1, ao1.Description AS Kind1, positions.Code AS JobPosition, jobs.Code AS OccupationCode, jobs.DisplayName AS Occupation, jobs_responsibility.DisplayName AS Responsibility, jobs_categories.Code AS CO, scholarships.Acronym AS RequireScholarship, jobs_groups.[Group], jobs_groups.BaseSalary, shifts.DisplayName AS WorkShift, employees.Code AS Employment, employees.[Exp], UPPER(persons.[Name] + ' ' + persons.Surname + ' ' + persons.Lastname) AS Employee, employees.TotalSalary
FROM   Kontecg.est.workplace_payments RIGHT OUTER JOIN Kontecg.gen.companies INNER JOIN
       Kontecg.est.organization_units ou ON Kontecg.gen.companies.Id = ou.CompanyId LEFT OUTER JOIN
       Kontecg.est.workplace_classifications AS wc ON ou.ClassificationId = wc.Id ON Kontecg.est.workplace_payments.Id = ou.WorkPlacePaymentId 
	   LEFT OUTER JOIN Sgnom.est.template_job_positions positions ON ou.Id = positions.OrganizationUnitId
	   LEFT JOIN Sgnom.est.occupations jobs ON positions.OccupationId = jobs.Id
	   LEFT JOIN Sgnom.est.complexity_groups jobs_groups ON jobs.GroupId = jobs_groups.Id
	   LEFT JOIN Sgnom.est.occupational_categories jobs_categories ON jobs.CategoryId = jobs_categories.Id
	   LEFT JOIN Sgnom.est.responsibility_levels jobs_responsibility ON jobs.ResponsibilityId = jobs_responsibility.Id
	   LEFT OUTER JOIN Sgnom.gen.scholarship_level_definitions scholarships ON positions.ScholarshipLevelId = scholarships.Id
	   LEFT OUTER JOIN Sgnom.gen.work_shifts shifts ON positions.WorkShiftId = shifts.Id
	   LEFT OUTER JOIN Sgnom.rel.employment_documents employees ON positions.DocumentId = employees.Id
	   LEFT OUTER JOIN Kontecg.docs.persons persons ON employees.PersonId = persons.Id
	   
	   CROSS APPLY Kontecg.dbo.GetOrganizationUnitAncestor(ou.Id, 2) ao2 
	   CROSS APPLY Kontecg.dbo.GetOrganizationUnitAncestor(ou.Id, 1) ao1
WHERE  (wc.[Level] = 3)

CREATE VIEW [est].[vw_job_template]
AS
SELECT Kontecg.gen.companies.Organism AS Organism, Kontecg.gen.companies.CompanyName AS CompanyName, ou.Code AS Code, ou.DisplayName AS Area, ou.Description AS ShortName, wc.Description AS Kind, wc.[Level] AS [Level], 
       Kontecg.est.workplace_payments.Code AS PaymentGroupCode, Kontecg.est.workplace_payments.Description AS PaymentGroupName, positions.CenterCost, ao2.Code AS Code2, ao2.DisplayName AS Area2, ao2.[Level] AS Level2, ao2.Description AS Kind2, ao1.Code AS Code1, ao1.DisplayName AS Area1, ao1.[Level] AS Level1, ao1.Description AS Kind1, jobs.Code AS OccupationCode, jobs.DisplayName AS Occupation, jobs_responsibility.DisplayName AS Responsibility, jobs_categories.Code AS CO, scholarships.Acronym AS RequireScholarship, jobs_groups.[Group], jobs_groups.BaseSalary, positions.WorkShift, positions.Proposals, positions.Approved
FROM   Kontecg.est.workplace_payments RIGHT OUTER JOIN Kontecg.gen.companies INNER JOIN
       Kontecg.est.organization_units ou ON Kontecg.gen.companies.Id = ou.CompanyId LEFT OUTER JOIN
       Kontecg.est.workplace_classifications AS wc ON ou.ClassificationId = wc.Id ON Kontecg.est.workplace_payments.Id = ou.WorkPlacePaymentId        
	   LEFT OUTER JOIN Sgnom.est.templates positions ON ou.Id = positions.OrganizationUnitId
	   LEFT JOIN Sgnom.est.occupations jobs ON positions.OccupationId = jobs.Id
	   LEFT JOIN Sgnom.est.complexity_groups jobs_groups ON jobs.GroupId = jobs_groups.Id
	   LEFT JOIN Sgnom.est.occupational_categories jobs_categories ON jobs.CategoryId = jobs_categories.Id
	   LEFT JOIN Sgnom.est.responsibility_levels jobs_responsibility ON jobs.ResponsibilityId = jobs_responsibility.Id
	   LEFT OUTER JOIN Sgnom.gen.scholarship_level_definitions scholarships ON positions.ScholarshipLevelId = scholarships.Id
	   	   
	   CROSS APPLY Kontecg.dbo.GetOrganizationUnitAncestor(ou.Id, 2) ao2 
	   CROSS APPLY Kontecg.dbo.GetOrganizationUnitAncestor(ou.Id, 1) ao1
WHERE  (wc.[Level] = 3)