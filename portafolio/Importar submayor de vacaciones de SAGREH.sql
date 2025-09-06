DECLARE @DocumentId int = 0, @DocumentDefinitionId int = 0, @PeriodId int = 0;

select @DocumentDefinitionId = Kontecg.gen.document_definitions.Id from Kontecg.gen.document_definitions where Code = 28

select distinct @PeriodId = max(p.Id) over (partition by p.[Status]) from Kontecg.gen.periods p where p.[Status] = 'Closed'

insert into Kontecg.cnt.accounting_documents(Code, DocumentDefinitionId, [Description], AccountingPeriodId, CompanyId, Review, Exported, CreationTime)
values ('001.25', @DocumentDefinitionId, 'Importación del acumulado de vacaciones desde SAGREH', @PeriodId, 1, 2, 0, getdate())

select @DocumentId = Id from Kontecg.cnt.accounting_documents where Code = '001.25'

insert into SGNOM.vac.holiday_histogram(DocumentDefinitionId, DocumentId, PersonId, GroupId, Since, Until,  [Hours], Amount, Currency, [Status], CompanyId, CreationTime)
select 36 as DocumentDefinitionId, @DocumentId as DocumentId, p.Id as PersonId, ISNULL(d.GroupId, '00000000-0000-0000-0000-000000000000') as GroupId, Since, Until, [Hours], Amount, Currency, [Status], t.CompanyId, t.CreationTime from (
select t.IdPersona, persona.CI as IdentityCard, DATEADD(mm,-1,DATEADD(mm,DATEDIFF(mm,0,GETDATE()),0)) as Since, DATEADD(ms,-1,DATEADD(mm,0,DATEADD(mm,DATEDIFF(mm,0,GETDATE()),0))) as Until, case when sum(t.Amount) <= 0 then 0 else sum(t.[Hours]) end as [Hours], sum(t.Amount) as Amount, t.Currency, t.[Status], t.CompanyId, getdate() as CreationTime from 
(
select submayor.P_ID as IdPersona, submayor.IdEmpleo as GroupId, DATEADD(mm,DATEDIFF(mm,0,submayor.DOP_PER),0) as Since, DATEADD(ms,-1,DATEADD(mm,0,DATEADD(mm,DATEDIFF(mm,0,submayor.DOP_PER)+1,0))) as Until, submayor.SUB_SIDI as [Hours], submayor.SUB_SIIM as Amount, 'CUP' as Currency, 'Made' as [Status], 1 as CompanyId, getdate() as CreationTime from iAraGlobal_ECG.dbo.VAC_SUMVSInicial submayor
union all
select submayor.SVAj_IdPersona as IdPersona, submayor.IdEmpleo as GroupId, DATEADD(mm,DATEDIFF(mm,0,submayor.SVAj_Periodo),0) as Since, DATEADD(ms,-1,DATEADD(mm,0,DATEADD(mm,DATEDIFF(mm,0,submayor.SVAj_Periodo)+1,0))) as Until, submayor.SVAJ_Tiempo as [Hours], submayor.SVAJ_Importe as Amount, 'CUP' as Currency, 'Made' as [Status], 1 as CompanyId, getdate() as CreationTime from iAraGlobal_ECG.dbo.VAC_SUMVAJ submayor
union all
select submayor.DSV_IdPersona as IdPersona, submayor.IdEmpleo as GroupId, DATEADD(mm,DATEDIFF(mm,0,submayor.DSV_Periodo),0) as Since, DATEADD(ms,-1,DATEADD(mm,0,DATEADD(mm,DATEDIFF(mm,0,submayor.DSV_Periodo)+1,0))) as Until, submayor.DSV_TiempoVac as [Hours], submayor.DSV_ImporteVac as Amount, 'CUP' as Currency, 'Made' as [Status], 1 as CompanyId, getdate() as CreationTime from iAraGlobal_ECG.dbo.VAC_SubmayorVAC submayor where submayor.DSV_Estado = 2
) as t

inner join (select distinct max(mov.IdMov) over (partition by mov.IdEmpleo) as idmovultimo, mov.IdPersona, mov.IdEmpleo from iAraGlobal_ECG.dbo.RELMovimientos mov where mov.Procesado = 1 and mov.TipoMov <> 'B') as mov on mov.IdEmpleo = t.GroupId AND mov.IdPersona = t.IdPersona
inner join iAraGlobal_ECG.dbo.PERPersonas persona on persona.IdPersona = mov.IdPersona
group by t.IdPersona, persona.CI, t.Currency, t.[Status], t.CompanyId
having sum(t.Amount) > 0
) t

left outer join Kontecg.docs.persons p on p.IdentityCard = t.IdentityCard
left outer join SGNOM.rel.employment_documents d on p.Id = d.PersonId

where p.Id is not null