SELECT Parent.DisplayName AS Parent, ParentClass.Level, ou.DisplayName AS Area, c.Description, c.Level, ou.[Order], ou.Code, p.Description AS WorkPlacePayment
FROM Kontecg.est.organization_units ou
INNER JOIN Kontecg.est.workplace_classifications c ON ou.ClassificationId = c.Id 
LEFT OUTER JOIN Kontecg.est.workplace_payments p ON ou.WorkPlacePaymentId = p.Id 
LEFT OUTER JOIN Kontecg.est.organization_units AS Parent ON ou.ParentId = Parent.Id
LEFT OUTER JOIN Kontecg.est.workplace_classifications AS ParentClass ON Parent.ClassificationId = ParentClass.Id 
ORDER BY ou.Code