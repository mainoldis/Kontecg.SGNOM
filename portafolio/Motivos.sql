SELECT MGMotivosMvtos.Motivo, MGMotivosMvtos.Tipo, MGMotivosMvtos.Abreviatura, MGEspecifMotivosMvtos.EspecificacionMvto, MGMotivosBajas.ClasifBaja, MGMotivosBajas.ProvocaFluct
FROM   MGMotivosBajas RIGHT OUTER JOIN
       MGMotivosMvtos ON MGMotivosBajas.IdMotivoBaja = MGMotivosMvtos.IdMotivoMvto LEFT OUTER JOIN
       MGEspecifMotivosMvtos ON MGMotivosMvtos.IdMotivoMvto = MGEspecifMotivosMvtos.IdMotivoMvto
ORDER BY MGMotivosMvtos.Tipo