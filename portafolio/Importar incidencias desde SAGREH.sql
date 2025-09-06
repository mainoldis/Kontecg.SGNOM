DECLARE @DocumentDefinitionId INT, @Periodo datetime = '20250503';
SELECT @DocumentDefinitionId = Id FROM Kontecg.gen.document_definitions WHERE Reference = 'DOCTIME';

SELECT e.* INTO #Ultimo FROM SGNOM.rel.employment_documents e INNER JOIN (
SELECT DISTINCT MAX(DocumentDefinitionId) OVER (PARTITION BY GroupId) AS IdSagreh, PersonId, GroupId
FROM SGNOM.rel.employment_documents WHERE Review = 2) t ON t.IdSagreh = e.DocumentDefinitionId

SELECT @DocumentDefinitionId AS DocumentDefinitionId,
	   CONCAT(areaPago.Code, ' ', areaPago.[Description], ' ', YEAR(GEN_PERIODOSPAGO.Periodo), ' ', MONTH(GEN_PERIODOSPAGO.Periodo)) AS DisplayName, 
	   REPLACE(SAL_RPTINC.RI_COD, '.','') AS Code, 
	   1 AS CompanyId,
	   areaPago.Id AS WorkPlacePaymentId, 
	   p.Id AS PeriodId,
	   p.Since,
	   p.Until,
	   2 AS Review,	   
	   getdate() AS CreationTime
INTO   #Documentos
FROM   SAL_RPTINC INNER JOIN
	   GEN_PERIODOSPAGO ON GEN_PERIODOSPAGO.Periodo = SAL_RPTINC.RI_PERPAGO INNER JOIN
	   Kontecg.est.workplace_payments areaPago ON areaPago.Code = SUBSTRING(SAL_RPTINC.RI_DESC, 1,2) INNER JOIN
	   Kontecg.gen.periods p ON p.[Year] = YEAR(GEN_PERIODOSPAGO.Periodo) AND p.[Month] = MONTH(GEN_PERIODOSPAGO.Periodo)
WHERE  (SAL_RPTINC.RI_PRES = 1) AND (SAL_RPTINC.RI_PROCESADO = 2) AND (SAL_RPTINC.Valida = 1) AND GEN_PERIODOSPAGO.Periodo = @Periodo
ORDER BY SUBSTRING(SAL_RPTINC.RI_DESC, 1,2)

SELECT * FROM #Documentos

MERGE SGNOM.sal.time_distribution_documents AS t
USING #Documentos AS o
ON (t.Code = o.Code)
WHEN NOT MATCHED THEN INSERT (DocumentDefinitionId, DisplayName, Code, CompanyId, WorkPlacePaymentId, PeriodId, Since, Until, Review, CreationTime)
VALUES(o.DocumentDefinitionId, o.DisplayName, o.Code, o.CompanyId, o.WorkPlacePaymentId, o.PeriodId, o.Since, o.Until, o.Review, o.CreationTime)
WHEN MATCHED THEN UPDATE SET t.LastModificationTime = getdate()
OUTPUT deleted.*,$action,inserted.*;

MERGE SGNOM.sal.time_distributions AS t
USING (
SELECT doc.Id AS DocumentId,
	   e.PersonId, 
	   e.Id AS EmploymentId,
	   SAL_INCIDENC.IdEmpleo AS GroupId,
	   CONT_CC.CC_COD AS CenterCost, 
	   CASE SAL_INCIDENC.TIPO_INC WHEN 1 THEN 'Reported' WHEN 2 THEN 'Presented' WHEN 3 THEN 'Generated' ELSE 'Unknown' END AS Kind,
	   paydef.Id AS PaymentDefinitionId,
	   SAL_INCIDENC.INC_HORAS AS [Hours], 
	   SAL_INCIDENC.INC_IMP AS Amount, 
	   'CUP' AS Currency,
	   0 AS ReservedForHoliday,
	   SAL_INCIDENC.IVAC AS AmountReservedForHoliday,
	   SAL_INCIDENC.INC_TARIFA AS RatePerHour,
	   CASE SAL_INCIDENC.STATUS WHEN 3 THEN 'Made' ELSE 'ToMake' END AS [Status], 
	   1 AS CompanyId

FROM   SAL_RPTINC INNER JOIN
       SAL_INCIDENC ON SAL_RPTINC.RI_ID = SAL_INCIDENC.RI_ID INNER JOIN
	   GEN_PERIODOSPAGO ON GEN_PERIODOSPAGO.Periodo = SAL_RPTINC.RI_PERPAGO INNER JOIN
       CONT_CC ON SAL_INCIDENC.id_CC = CONT_CC.id_CC INNER JOIN
	   #Ultimo e ON e.GroupId = IdEmpleo INNER JOIN
	   Kontecg.est.workplace_payments areaPago ON areaPago.Code = SUBSTRING(SAL_RPTINC.RI_DESC, 1,2) INNER JOIN
	   Kontecg.gen.periods p ON p.[Year] = YEAR(GEN_PERIODOSPAGO.Periodo) AND p.[Month] = MONTH(GEN_PERIODOSPAGO.Periodo) INNER JOIN
	   SGNOM.sal.payment_definitions paydef ON paydef.[Name] = CLAVE INNER JOIN
	   SGNOM.sal.time_distribution_documents doc ON doc.Code = REPLACE(SAL_RPTINC.RI_COD, '.','')

WHERE  (SAL_RPTINC.RI_PRES = 1) AND (SAL_RPTINC.RI_PROCESADO = 2) AND (SAL_RPTINC.Valida = 1) AND GEN_PERIODOSPAGO.Periodo = @Periodo
) AS o

ON (t.DocumentId = o.DocumentId AND t.PersonId = o.PersonId AND t.GroupId = o.GroupId AND t.EmploymentId = o.EmploymentId AND t.PaymentDefinitionId = o.PaymentDefinitionId AND t.Kind = o.Kind AND t.CenterCost = o.CenterCost)
WHEN NOT MATCHED THEN INSERT (DocumentId, PersonId, EmploymentId, GroupId, CenterCost, Kind, PaymentDefinitionId, [Hours], Amount, Currency, ReservedForHoliday, AmountReservedForHoliday, RatePerHour, [Status], CompanyId, CreationTime)
VALUES(o.DocumentId, o.PersonId, o.EmploymentId, o.GroupId, o.CenterCost, o.Kind, o.PaymentDefinitionId, o.[Hours], o.Amount, o.Currency, o.ReservedForHoliday, o.AmountReservedForHoliday, o.RatePerHour, o.[Status], o.CompanyId, getdate())
WHEN MATCHED THEN UPDATE SET t.LastModificationTime = getdate()
OUTPUT deleted.*,$action,inserted.*;

DROP TABLE #Documentos;
DROP TABLE #Ultimo;