DECLARE @METODO nvarchar(150) = N'AddCenterCostIfNotExists(';

SELECT  @METODO + '_companyId, "' + UPPER(RTRIM(LTRIM(CONT_CC.CC_DESC))) + '", ' + CONT_CC.CC_COD + ', '+ convert(nvarchar(5), CONT_DEFCNT.CT_CNT) + + ');'
FROM            CONT_CC INNER JOIN
                         CONT_DEFCNT ON CONT_CC.id_CuentaCont = CONT_DEFCNT.CT_COD
ORDER BY CONVERT(int, CONT_CC.CC_COD)