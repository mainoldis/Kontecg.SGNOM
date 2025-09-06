select distinct N1.Codigo, N1.NombreArea, N1Tipo.DescripTipo, N1Tipo.Nivel, N.Codigo, N.NombreArea, NTipo.DescripTipo, NTipo.Nivel
from RELRelacionLaboral
inner join ESTAreasTrabajo AS N1 ON RELRelacionLaboral.IdAreaN1 = N1.IdAreasTrabajo
inner join MGTiposArea AS N1Tipo ON N1.IdTipoArea = N1Tipo.IDTipoArea
inner join ESTAreasTrabajo AS N ON RELRelacionLaboral.IdAreaTrabajo = N.IdAreasTrabajo
inner join MGTiposArea AS NTipo ON N.IdTipoArea = NTipo.IDTipoArea
order by N1Tipo.Nivel, N1.Codigo, NTipo.Nivel, N.Codigo
