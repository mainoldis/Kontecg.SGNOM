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
END