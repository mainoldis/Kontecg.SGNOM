SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetEmploymentProvisionalAncestor] 
(
	@ID bigint
)
RETURNS 
@Result TABLE 
(
	Id bigint,
	PreviousId bigint,
	[Type] nvarchar(1),
	SubType nvarchar(3),
	[Exp] int,
	Code nvarchar(11),
	PersonId bigint,
	GroupId uniqueidentifier,
	CompanyId int
)
AS
BEGIN
	WITH ou AS (
	SELECT e.Id, e.PreviousId, e.[Type], e.SubType, e.[Exp], e.Code, e.PersonId, e.GroupId, e.CompanyId, 1 AS [Index]
	FROM SGNOM.rel.employment_documents e WHERE e.[Type] = 'R' AND e.SubType IN ('MP', 'PMP') AND e.Id = @ID

	UNION ALL

	SELECT e.Id, e.PreviousId, e.[Type], e.SubType, e.[Exp], e.Code, e.PersonId, e.GroupId, e.CompanyId, [Index] + 1
	FROM SGNOM.rel.employment_documents e
	INNER JOIN ou ON e.Id = ou.PreviousId
	)

	INSERT @Result
	SELECT  TOP 1 t.Id, t.PreviousId, t.[Type], t.SubType, t.[Exp], t.Code, t.PersonId, t.GroupId, t.CompanyId FROM
	(SELECT *, ROW_NUMBER() OVER (
        PARTITION BY GroupId
        ORDER BY [Index] ASC
    ) as rn 
	FROM ou) t WHERE t.SubType NOT IN ('MP', 'PMP');

	RETURN 
END