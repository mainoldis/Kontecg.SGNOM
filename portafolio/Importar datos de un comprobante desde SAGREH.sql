select doc_oper.DOP_COD AS Code, doc_def.Id AS DocumentDefinitionId, doc.Descripcion AS Description, p.Id AS AccountingPeriodId, 1 AS CompanyId, 2 AS Review, getdate() AS CreationTime, comprobante.CodComprobante, comprobante.Descripcion AS Comprobante, p.Since AS Since, p.Until AS Until, doc.IdDocumento AS SAGREH_IdDocumento, doc.IdComprobante AS SAGREH_IdComprobante
into #documento
from iAraGlobal_ECG.dbo.CONT_Documentos doc 
inner join iAraGlobal_ECG.dbo.DOP_DOCOPER doc_oper ON doc.IdDocumento = doc_oper.DOP_ID
inner join iAraGlobal_ECG.dbo.GEN_PERIODOSPAGO periodo ON doc.IdPeriodo = periodo.IdPeriodo
inner join iAraGlobal_ECG.dbo.CONT_COMPROBANTES comprobante ON doc.IdComprobante = comprobante.IdComprobante
left outer join Kontecg.gen.periods p ON periodo.Periodo >= p.Since AND periodo.Periodo <= p.Until
left outer join Kontecg.gen.document_definitions doc_def ON doc.TD_COD = doc_def.Code
where doc.IdPeriodo IN (select [Value] from String_Split('1010124,1010125,1010126',','))

select * from #documento

merge Kontecg.cnt.accounting_documents as t
using #documento as o
on (SUBSTRING(t.Code, 1, 15) = SUBSTRING(o.Code, 1, 15))
when not matched then insert (Code, DocumentDefinitionId, Description, AccountingPeriodId, CompanyId, Review, Exported, CreationTime)
values(SUBSTRING(o.Code, 1, 15), o.DocumentDefinitionId, o.Description, o.AccountingPeriodId, o.CompanyId, o.Review, 0, o.CreationTime)
when matched then update set t.LastModificationTime = getdate()
output deleted.*,$action,inserted.*;

merge Kontecg.cnt.accounting_voucher_documents AS t 
using (select doc_def.Id AS DocumentDefinitionId, #documento.CodComprobante, #documento.Comprobante, #documento.Since, #documento.Until, ac.id AS DocumentId, #documento.CompanyId, #documento.Review, getdate() AS CreationTime from #documento inner join Kontecg.cnt.accounting_documents ac on ac.Code = SUBSTRING(#documento.Code, 1, 15) inner join Kontecg.gen.document_definitions doc_def ON doc_def.Code = 40) AS o
on (t.Code = o.CodComprobante)
when not matched then insert (DocumentDefinitionId, Code, Description, StartingDate, FinishingDate, DocumentId, CompanyId, Review, CreationTime)
values(o.DocumentDefinitionId, o.CodComprobante, o.Comprobante, o.Since, o.Until, o.DocumentId, o.CompanyId, o.Review, o.CreationTime)
when matched then update set LastModificationTime = getdate()
output deleted.*,$action,inserted.*;

merge Kontecg.cnt.accounting_voucher_notes AS t
using (
select voucher.Id AS DocumentId, CASE WHEN IMP < 0 THEN 'Debit' ELSE 'Credit' END AS Operation, apct.CSF AS Scope, CNT AS Account, SCNT AS SubAccount, SCTRL AS SubControl, ANAL AS Analysis, IMP AS Amount, N'CUP' AS Currency, #documento.CompanyId, #documento.CreationTime from #documento inner join Kontecg.cnt.accounting_voucher_documents voucher on #documento.CodComprobante = voucher.Code
inner join iAraGlobal_ECG.dbo.CONT_APCT apct on apct.IdComprobante = #documento.SAGREH_IdComprobante and apct.CSF IN (1,2)) AS o
on (t.DocumentId = o.DocumentId and t.Operation = o.Operation and t.ScopeId = o.Scope and t.Account = o.Account and t.SubAccount = o.SubAccount and t.SubControl = o.SubControl and t.Analysis = o.Analysis)
when not matched then insert (DocumentId, Operation, ScopeId, Account, SubAccount, SubControl, Analysis, Amount, Currency, CompanyId, CreationTime)
values(o.DocumentId, o.Operation, o.Scope, o.Account, o.SubAccount, o.SubControl, o.Analysis, o.Amount, o.Currency, o.CompanyId, o.CreationTime)
when matched then update set CreationTime = getdate()
output deleted.*,$action,inserted.*;

drop table #documento;