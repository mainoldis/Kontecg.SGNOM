CREATE FUNCTION [dbo].[GetAccountingVoucherNotes] 
(
	@ID bigint, 
	@Scope int = 1
)
RETURNS 
@Result TABLE 
(
	Account int,
	SubAccount int, 
	SubControl int,
	Analysis int,
	Parcial decimal(10,2),
	Debe decimal(10,2),
	Haber decimal(10,2),
	Signo int
)
AS
BEGIN
	INSERT @Result
	SELECT Account, SubAccount, SubControl, Analysis, ABS(SUM(Amount)) AS Parcial, CASE WHEN SUM(Amount) >= 0 THEN ABS(SUM(Amount)) ELSE NULL END AS Debe, CASE WHEN SUM(Amount) < 0 THEN ABS(SUM(Amount)) ELSE NULL END AS Haber, CASE WHEN SUM(Amount) >= 0 THEN 1 ELSE -1 END AS Signo
	FROM Kontecg.cnt.accounting_voucher_notes
	WHERE DocumentId = @ID and ScopeId = @Scope
	GROUP BY Account, SubAccount, SubControl, Analysis
	RETURN 
END