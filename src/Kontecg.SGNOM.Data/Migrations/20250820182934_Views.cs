using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kontecg.Migrations
{
    /// <inheritdoc />
    public partial class Views : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ext");

            if (migrationBuilder.IsSqlServer())
            {
                migrationBuilder.Sql(@"
CREATE FUNCTION [dbo].[GetOrganizationUnitAncestor] 
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
END");
                migrationBuilder.Sql(@"
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
END");

                migrationBuilder.Sql(@"
CREATE FUNCTION [dbo].[RomanToDecimal] (@roman VARCHAR(50))
RETURNS INT
AS
BEGIN
    -- Tabla de símbolos romanos y sus valores
    DECLARE @symbols TABLE (
        symbol CHAR(1),
        value INT
    );
    
    -- Insertamos los símbolos en orden descendente de valor
    INSERT INTO @symbols VALUES 
	    ('I', 1),
        ('V', 5),
        ('X', 10),
        ('L', 50),
        ('C', 100),
        ('D', 500),
        ('M', 1000);

    DECLARE @result INT = 0;
    DECLARE @currentValue INT;
    DECLARE @nextValue INT = 0;
    DECLARE @position INT = 1;
    
    -- Validación básica
    IF @roman IS NULL OR LEN(@roman) = 0
        RETURN NULL;
        
    -- Procesamos el número romano de izquierda a derecha
    WHILE @position <= LEN(@roman) 
    BEGIN
        -- Obtenemos el valor del símbolo actual
        SELECT TOP 1 @currentValue = value
        FROM @symbols
        WHERE symbol = SUBSTRING(@roman, @position, 1);

		-- Obtenemos el valor del símbolo actual
        SELECT TOP 1 @nextValue = value
        FROM @symbols
        WHERE symbol = SUBSTRING(@roman, @position + 1, 1);
        
        -- Aplicamos la regla de suma/resta
        IF @currentValue < @nextValue
            SET @result = @result - @currentValue;
        ELSE
            SET @result = @result + @currentValue;
            
        SET @position = @position + 1;
    END
    
    RETURN @result;
END");

                migrationBuilder.Sql(@"
                    CREATE VIEW ext.vw_persons_accounts
                    AS
                    SELECT
                        Kontecg.docs.persons.Id AS PersonId, 
                        Kontecg.docs.persons.Name, 
                        Kontecg.docs.persons.Surname, 
                        Kontecg.docs.persons.Lastname, 
                        Kontecg.docs.persons.IdentityCard, 
                        docs.person_accounts.AccountNumber, 
                        docs.person_accounts.Currency, 
                        docs.person_accounts.IsActive
                    FROM 
                        docs.person_accounts RIGHT OUTER JOIN
                        Kontecg.docs.persons ON docs.person_accounts.PersonId = Kontecg.docs.persons.Id
                    GO");

                migrationBuilder.Sql(@"
                    CREATE VIEW ext.vw_employment_documents_out_of_positions
                    AS
                    SELECT 
                        employment_documents.Id AS DocumentId, 
                        employment_documents.Code, 
                        employment_documents.[Type], 
                        employment_documents.SubType, 
                        employment_documents.Exp, 
                        Kontecg.docs.persons.Id AS PersonId,
                        Kontecg.docs.persons.IdentityCard,
                        UPPER(Kontecg.docs.Persons.[Name] + ' ' + Kontecg.docs.Persons.Surname + ' ' + Kontecg.docs.Persons.Lastname) AS Worker, 
                        employment_documents.OrganizationUnitId, 
                        employment_documents.CenterCost, 
                        employment_documents.OccupationCode, 
                        employment_documents.Occupation, 
                        employment_documents.OccupationCategory, 
                        employment_documents.ComplexityGroup
                    FROM 
                        Sgnom.rel.employment_documents INNER JOIN
	                    Kontecg.docs.Persons ON Kontecg.docs.Persons.Id = Sgnom.rel.employment_documents.PersonId LEFT JOIN 
	                    Sgnom.est.template_job_positions ON template_job_positions.DocumentId = employment_documents.Id
                        WHERE employment_documents.SubType IN ('DI', 'FMP','I') AND template_job_positions.DocumentId IS NULL
                    GO");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsSqlServer())
            {
                migrationBuilder.Sql(@"DROP FUNCTION dbo.RomanToDecimal");

                migrationBuilder.Sql(@"DROP FUNCTION dbo.GetEmploymentProvisionalAncestor");

                migrationBuilder.Sql(@"DROP FUNCTION dbo.GetOrganizationUnitAncestor");

                migrationBuilder.Sql(@"DROP VIEW ext.vw_persons_accounts");

                migrationBuilder.Sql(@"DROP VIEW ext.vw_employment_documents_out_of_positions");
            }
        }
    }
}
