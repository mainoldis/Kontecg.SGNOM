DECLARE @METODO nvarchar(150) = N'CreateExpenseItemIfNotExists(';

SELECT  @METODO + '_companyId, "' + RTRIM(LTRIM(CONT_ELEGAST.EG_DESC)) + '".TrimEnd(), ' + CONVERT(NVARCHAR(10),CONT_ELEGAST.EG_COD) + ');'
FROM            CONT_ELEGAST