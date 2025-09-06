SELECT c.CompanyName, e.[Exp], per.[Name], per.Surname, per.Lastname, e.Salary, 
       e.TotalSalary, docs.Code, docs.DisplayName, 
       docs.Since, docs.Until, docs.Review, t.Kind, 
       p.[Name] AS [Key], t.[Hours], t.Amount, t.RatePerHour AS RatePerHour, t.[Status]
FROM   Sgnom.sal.time_distributions t INNER JOIN
       Sgnom.sal.time_distribution_documents docs ON t.DocumentId = docs.Id INNER JOIN
       Sgnom.sal.payment_definitions p ON t.PaymentDefinitionId = p.Id INNER JOIN
       Sgnom.rel.employment_documents e ON t.EmploymentId = e.Id AND t.GroupId = e.GroupId LEFT OUTER JOIN
       Kontecg.gen.companies c ON e.CompanyId = c.Id LEFT OUTER JOIN
       Kontecg.docs.persons per ON e.PersonId = per.Id
ORDER BY Exp