SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION GetOrganizationUnitAncestor 
(
	@ID bigint, 
	@LEVEL int = -1
)
RETURNS 
@Result TABLE 
(
	Id bigint,
	Code nvarchar(12), 
	DisplayName nvarchar(128),
	ParentId bigint,
	[Level] int,
	Kind nvarchar(150),
	WorkPlacePaymentCode nvarchar(150)
)
AS
BEGIN
	WITH ou AS (
	SELECT e.Id, e.Code, e.DisplayName, e.ParentId, c.Level, c.Description, e.WorkPlacePaymentId, 1 AS [Index]
	FROM Kontecg.est.organization_units e
	INNER JOIN Kontecg.est.workplace_classifications c ON c.Id = e.ClassificationId
	WHERE e.Id = @ID

	UNION ALL

	SELECT o.Id, o.Code, o.DisplayName, o.ParentId, c.Level, c.Description, o.WorkPlacePaymentId, [Index] + 1
	FROM Kontecg.est.organization_units o
	INNER JOIN Kontecg.est.workplace_classifications c ON c.Id = o.ClassificationId
	INNER JOIN ou ON o.Id = ou.ParentId
	)

	INSERT @Result
	SELECT t.Id, t.Code, t.DisplayName, t.ParentId, t.[Level], t.[Description] AS Kind, wp.Code AS WorkPlacePaymentCode FROM
	(SELECT *, ROW_NUMBER() OVER (
        PARTITION BY [Level]
        ORDER BY [Index] DESC
    ) as rn 
	FROM ou WHERE (ou.Level = @LEVEL) OR (@LEVEL IS NULL OR @LEVEL = -1)) t
	LEFT OUTER JOIN Kontecg.est.workplace_payments wp ON wp.Id = t.WorkPlacePaymentId
	WHERE rn = 1;

	RETURN 
END
