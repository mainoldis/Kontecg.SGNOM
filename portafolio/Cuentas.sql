DECLARE @METODO nvarchar(150) = N'CreateAccountDefinitionIfNotExists(';

SELECT @METODO + convert(nvarchar(10), CT_CNT) + ', ' + convert(nvarchar(10), CT_SUBC) + ', ' + convert(nvarchar(10), CT_SCTRL) + ', ' + convert(nvarchar(10), CT_ANAL) + ', "' + CT_DESC + '");'
FROM CONT_DEFCNT
WHERE (Activo = 1)