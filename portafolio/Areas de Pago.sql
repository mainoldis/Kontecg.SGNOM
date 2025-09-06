SELECT  DISTINCT      RELMovimientosHist.AreaN1, RELMovimientosHist.Area, RELMovimientosHist.CodArea, RELMovimientosHist.CCosto, GEN_AREASPAGO.Descripcion
FROM            GEN_AREASPAGO INNER JOIN
                         ESTAreasTrabajo ON GEN_AREASPAGO.IdAreasPago = ESTAreasTrabajo.IdAreaPago INNER JOIN
                         RELRelacionLaboral INNER JOIN
                         RELMovimientosHist ON RELRelacionLaboral.IdMov = RELMovimientosHist.IdMov ON ESTAreasTrabajo.IdAreasTrabajo = RELRelacionLaboral.IdAreaTrabajo
order by Descripcion, Area