SELECT employment_documents.Id, employment_documents.Code, employment_documents.[Type], employment_documents.SubType, employment_documents.Exp, UPPER(Kontecg.docs.Persons.[Name] + ' ' + Kontecg.docs.Persons.Surname + ' ' + Kontecg.docs.Persons.Lastname) AS Worker, employment_documents.OrganizationUnitId, employment_documents.CenterCost, employment_documents.OccupationCode, employment_documents.Occupation, employment_documents.OccupationCategory, employment_documents.ComplexityGroup
FROM Sgnom.rel.employment_documents INNER JOIN
	 Kontecg.docs.Persons ON Kontecg.docs.Persons.Id = Sgnom.rel.employment_documents.PersonId	LEFT JOIN 
	 Sgnom.est.template_job_positions ON template_job_positions.DocumentId = employment_documents.Id
WHERE employment_documents.SubType IN ('DI', 'FMP','I') AND template_job_positions.DocumentId IS NULL
ORDER BY employment_documents.Exp