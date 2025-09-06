DECLARE @IdEmpresa int = 101;
DECLARE @IdAreaN1 int = -1;
DECLARE @IdEstructura int = 101049;

SELECT     UPPER(ESTAreasTrabajo_N1.NombreArea) AS AreaPadre, UPPER(ESTAreasTrabajo.NombreArea) AS NombreArea, ESTPlazasEnPlantilla.Tipo, MGGrupoSalarial.Grupo,
                       MGCargos.NombreCargo AS NombreComunCargo, MGCategoriasOcupac.Categoria, vist_Plazas_CantOcupada.CantOcupada AS Cubiertas, 
                      ESTPlazasEnPlantilla.CantProp, ESTPlazasEnPlantilla.CantAprob, MGCentrosLaborales.NombreComun, MGProvincias.Provincia, MGMunicipios.Municipio, 
                      CASE ESTPlazasEnPlantilla.IdResponsabilidadCargo WHEN 141 THEN MGCargos.NombreCargo ELSE MGCargos.NombreCargo + ' (' + MGResponsabilidadCargo.Responsabilidad
                       + ')' END AS NombreCargo, MGNivelesEducacionales.NivelEducacional, ESTAreasEstructura_1.Posicion, CASE MGNivelesEducacionales.Abrev when  'NS-EP' then 'NS(EP)'when 'MS-EP'then 'MS(EP)' when 'NS-CH' then 'NS(CH)' when 'MS-CH' then 'MS(CH)' when 'TM-EP' then 'TM(EP)' when 'TM-CH' then 'TM(CH)'
																																							when 'NM-EP' then 'NM(EP)' when 'MS-EP' then 'MS(EP)' when 'NMEPC' then 'NM(EP,CH)' when 'NSEPC' then 'NS(EP-CH)' when 'NMSEC' then 'MS(EP-CH)' when 'NM-CH' then 'NM(CH)' when   'NMEPL' then 'NM(EP-LC)'  else isnull(MGNivelesEducacionales.Abrev,'') end as Abrev
FROM         MGResponsabilidadCargo INNER JOIN
                      ESTAreasTrabajo INNER JOIN
                      MGCategoriasOcupac INNER JOIN
                      ESTPlazasEnPlantilla INNER JOIN
                      MGGrupoSalarial ON ESTPlazasEnPlantilla.IdGrupoSalarial = MGGrupoSalarial.IdGrupoSalarial INNER JOIN
                      MGCargos ON ESTPlazasEnPlantilla.IdCargo = MGCargos.IdCargo ON MGCategoriasOcupac.IDCategoria = MGCargos.IdCategOcupac INNER JOIN
                      ESTPlantilla ON ESTPlazasEnPlantilla.IdPlantilla = ESTPlantilla.IdPlantilla INNER JOIN
                      ESTAreasEstructura ON ESTPlazasEnPlantilla.IdAreasTrabajo = ESTAreasEstructura.IdAreasTrabajo ON 
                      ESTAreasTrabajo.IdAreasTrabajo = ESTAreasEstructura.IdAreasTrabajo INNER JOIN
                      ESTAreasTrabajo AS ESTAreasTrabajo_N1 ON ESTAreasEstructura.IdAreaN1 = ESTAreasTrabajo_N1.IdAreasTrabajo INNER JOIN
                      vist_Plazas_CantOcupada ON ESTPlazasEnPlantilla.IdPlaza = vist_Plazas_CantOcupada.IdPlaza INNER JOIN
                      MGCentrosLaborales ON ESTPlantilla.IdEmpresa = MGCentrosLaborales.IdCentro INNER JOIN
                      MGProvincias INNER JOIN
                      MGMunicipios ON MGProvincias.IdProvincia = MGMunicipios.IdProvincia ON MGCentrosLaborales.Municipio = MGMunicipios.IdMunicipio ON 
                      MGResponsabilidadCargo.IdResponsabilidadCargo = ESTPlazasEnPlantilla.IdResponsabilidadCargo INNER JOIN
                      ESTAreasEstructura AS ESTAreasEstructura_1 ON ESTAreasTrabajo_N1.IdAreasTrabajo = ESTAreasEstructura_1.IdAreasTrabajo LEFT OUTER JOIN
                      MGNivelesEducacionales ON MGCargos.IdNivelEduc = MGNivelesEducacionales.IdNivelEduc

WHERE  ESTAreasEstructura.IdEstrucOrg = @IdEstructura and ((@IdEmpresa = - 1) AND (ESTAreasEstructura.IdAreaN1 = @IdAreaN1) OR
                      (@IdEmpresa = - 1) AND (@IdAreaN1 = - 1) OR
                      (ESTAreasEstructura.IdAreaN1 = @IdAreaN1) AND (ESTPlantilla.IdEmpresa = @IdEmpresa) OR
                      (@IdAreaN1 = - 1) AND (ESTPlantilla.IdEmpresa = @IdEmpresa))
ORDER BY ESTAreasEstructura_1.Posicion,ESTAreasEstructura.Posicion, ESTPlazasEnPlantilla.Tipo desc, MGGrupoSalarial.IdGrupoSalarial DESC, MGCargos.NombreCargo, ESTPlazasEnPlantilla.CantAprob