SELECT Kontecg.gen.companies.Organism AS Organism, Kontecg.gen.companies.CompanyName AS CompanyName, ou.Id, ou.Code AS Code, ou.DisplayName AS Area, ou.Description AS ShortName, wc.Description AS Kind, wc.[Level] AS [Level],
       Kontecg.est.workplace_payments.Code AS PaymentGroupCode, Kontecg.est.workplace_payments.Description AS PaymentGroupName, ao2.Code AS Code2, ao2.DisplayName AS Area2, ao2.[Level] AS Level2, ao2.Kind AS Kind2, ao1.Code AS Code1, ao1.DisplayName AS Area1, ao1.[Level] AS Level1, ao1.Kind AS Kind1
INTO #estructura
FROM   Kontecg.est.workplace_payments RIGHT OUTER JOIN Kontecg.gen.companies INNER JOIN
       Kontecg.est.organization_units ou ON Kontecg.gen.companies.Id = ou.CompanyId LEFT OUTER JOIN
       Kontecg.est.workplace_classifications AS wc ON ou.ClassificationId = wc.Id ON Kontecg.est.workplace_payments.Id = ou.WorkPlacePaymentId 
	   CROSS APPLY dbo.GetOrganizationUnitAncestor(ou.Id, 2) ao2 
	   CROSS APPLY dbo.GetOrganizationUnitAncestor(ou.Id, 1) ao1
WHERE  (wc.[Level] = 3)

SELECT  rel.employment_documents.Id AS DocumentId, rel.employment_documents.Code, rel.employment_documents.Type, rel.employment_documents.SubType, rel.employment_documents.Exp, Kontecg.docs.persons.Id AS PersonId, 
        Kontecg.docs.persons.IdentityCard, UPPER(Kontecg.docs.persons.Name + ' ' + Kontecg.docs.persons.Surname + ' ' + Kontecg.docs.persons.Lastname) AS Worker, rel.employment_documents.OrganizationUnitId, rel.employment_documents.FirstLevelDisplayName, rel.employment_documents.ThirdLevelDisplayName, rel.employment_documents.ThirdLevelCode,
        rel.employment_documents.CenterCost, rel.employment_documents.OccupationCode, rel.employment_documents.Occupation, rel.employment_documents.OccupationCategory, 
        rel.employment_documents.ComplexityGroup, #estructura.*
FROM    rel.employment_documents INNER JOIN
        Kontecg.docs.persons ON Kontecg.docs.persons.Id = rel.employment_documents.PersonId
		LEFT OUTER JOIN #estructura ON rel.employment_documents.OrganizationUnitId = #estructura.Id;

DROP TABLE #estructura;