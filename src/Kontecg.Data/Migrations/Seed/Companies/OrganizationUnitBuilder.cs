using System.Linq;
using Castle.Core.Logging;
using Kontecg.EFCore;
using Kontecg.Extensions;
using Kontecg.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Migrations.Seed.Companies
{
    public class OrganizationUnitBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly int _companyId;
        private readonly ILogger _logger;

        public OrganizationUnitBuilder(KontecgCoreDbContext context, int companyId, ILogger logger)
        {
            _context = context;
            _companyId = companyId;
            _logger = logger;
        }

        public void Create()
        {
            CreateWorkPlaces();
            _logger.InfoFormat("Organization Units created.");
        }

        private void CreateWorkPlaces()
        {
            var ouId0 = AddWorkPlaceIfNotExists("ECG", "ECG", "EMPRESA", 3043, numbers: [_companyId]);

            #region Dirección General

            var ouId1 = AddWorkPlaceIfNotExists("Dirección General", "DG", "Dirección General", 25, ouId0);
            var ouId2 = AddWorkPlaceIfNotExists("Dirección General", "DG", "Áreas", 25, ouId1);
            AddWorkPlaceIfNotExists("Dirección General", "DG", "Grupo", 4, ouId2, "39");
            AddWorkPlaceIfNotExists("Grupo Organización y Control", "DG", "Grupo", 4, ouId2, "39");
            AddWorkPlaceIfNotExists("Grupo Auditoría", "DG", "Grupo", 4, ouId2, "39");
            AddWorkPlaceIfNotExists("Órgano de Cuadros", "DG", "Grupo", 4, ouId2, "39");
            AddWorkPlaceIfNotExists("Asesoría Jurídica", "DG", "Grupo", 9, ouId2, "39");

            #endregion

            #region Dirección de Inversiones

            ouId1 = AddWorkPlaceIfNotExists("Dirección de Inversiones", "DI", "Direcciones Funcionales", 43, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("Dirección de Inversiones", "DI", "Áreas", 43, ouId1);
            AddWorkPlaceIfNotExists("Dirección de Inversiones", "DI", "Grupo", 5, ouId2, "20");
            AddWorkPlaceIfNotExists("Grupo de Preparación de las Inversiones", "DI", "Grupo", 5, ouId2, "20");
            AddWorkPlaceIfNotExists("Grupo de Control y Coordinación", "DI", "Grupo", 4, ouId2, "20");
            AddWorkPlaceIfNotExists("Grupo Económico de Inversiones", "DI", "Grupo", 8, ouId2, "20");
            var ouId3 = AddWorkPlaceIfNotExists("Ejecución de Inversiones", "DI", "Áreas", 21, ouId2);
            AddWorkPlaceIfNotExists("Ejecución de Inversiones", "DI", "Grupo", 7, ouId3, "20");
            AddWorkPlaceIfNotExists("Grupo Reparaciones Capitales 1", "DI", "Grupo", 5, ouId3, "20");
            AddWorkPlaceIfNotExists("Grupo Reparaciones Capitales 2", "DI", "Grupo", 5, ouId3, "20");
            AddWorkPlaceIfNotExists("Grupo Reparaciones Capitales 3", "DI", "Grupo", 4, ouId3, "20");

            #endregion

            #region Dirección de Economía

            ouId1 = AddWorkPlaceIfNotExists("Dirección de Economía y Finanzas", "DE", "Direcciones Funcionales", 45, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("Dirección de Economía y Finanzas", "DE", "Áreas", 45, ouId1);
            AddWorkPlaceIfNotExists("Dirección de Economía y Finanzas", "DE", "Grupo", 4, ouId2, "28");
            AddWorkPlaceIfNotExists("Grupo de Planificación", "DE", "Grupo", 4, ouId2, "28");
            AddWorkPlaceIfNotExists("Grupo de Contabilidad y Costos", "DE", "Grupo", 9, ouId2, "28");
            AddWorkPlaceIfNotExists("Grupo de Nóminas", "DE", "Grupo", 3, ouId2, "28");
            AddWorkPlaceIfNotExists("Grupo de Inventarios", "DE", "Grupo", 12, ouId2, "28");
            AddWorkPlaceIfNotExists("Grupo de Cobros y Pagos", "DE", "Grupo", 13, ouId2, "28");

            #endregion

            #region Dirección de Recursos Humanos

            ouId1 = AddWorkPlaceIfNotExists("Dirección de Recursos Humanos", "RRHH", "Direcciones Funcionales", 34, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("Dirección de Recursos Humanos", "RRHH", "Áreas", 34, ouId1);
            AddWorkPlaceIfNotExists("Dirección de Recursos Humanos", "RRHH", "Grupo", 4, ouId2, "29");
            AddWorkPlaceIfNotExists("Grupo de Capacitación", "RRHH", "Grupo", 4, ouId2, "29");
            AddWorkPlaceIfNotExists("Grupo de Recursos Laborales", "RRHH", "Grupo", 11, ouId2, "29");
            AddWorkPlaceIfNotExists("Grupo de Servicios Generales", "RRHH", "Grupo", 10, ouId2, "29");
            AddWorkPlaceIfNotExists("Grupo Técnico", "RRHH", "Grupo", 5, ouId2, "29");

            #endregion

            #region Dirección de Producción

            ouId1 = AddWorkPlaceIfNotExists("Dirección de Producción", "DP", "Direcciones Funcionales", 20, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("Dirección de Producción", "DP", "Áreas", 20, ouId1);
            AddWorkPlaceIfNotExists("Dirección de Producción", "DP", "Grupo", 4, ouId2, "35");
            AddWorkPlaceIfNotExists("Grupo de Control de la Producción", "DP", "Grupo", 12, ouId2, "35");
            AddWorkPlaceIfNotExists("Economía Energética", "DP", "Grupo", 4, ouId2, "35");

            #endregion

            #region Dirección de Seguridad Salud y Medio Ambiente

            ouId1 = AddWorkPlaceIfNotExists("Dirección de Seguridad Salud y Medio Ambiente", "SSMA", "Dirección", 49, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("Dirección de Seguridad Salud y Medio Ambiente", "SSMA", "Áreas", 49, ouId1);
            AddWorkPlaceIfNotExists("Dirección de Seguridad Salud y Medio Ambiente", "SSMA", "Grupo", 5, ouId2, "30");
            AddWorkPlaceIfNotExists("Puesto Médico", "SSMA", "Grupo", 5, ouId2, "30");
            AddWorkPlaceIfNotExists("Grupo Medio Ambiente", "SSMA", "Grupo", 4, ouId2, "30");
            AddWorkPlaceIfNotExists("Seguridad Industrial", "SSMA", "Grupo", 12, ouId2, "30");
            AddWorkPlaceIfNotExists("Grupo de Rescate y Emergencia", "SSMA", "Grupo", 3, ouId2, "30");
            AddWorkPlaceIfNotExists("Brigada Turno \"A\"", "SSMA", "Brigada", 5, ouId2, "30");
            AddWorkPlaceIfNotExists("Brigada Turno \"B\"", "SSMA", "Brigada", 5, ouId2, "30");
            AddWorkPlaceIfNotExists("Brigada Turno \"C\"", "SSMA", "Brigada", 5, ouId2, "30");
            AddWorkPlaceIfNotExists("Brigada Turno \"D\"", "SSMA", "Brigada", 5, ouId2, "30");

            #endregion

            #region UBST

            ouId1 = AddWorkPlaceIfNotExists("UBS Servicios Técnicos a la Producción", "UBST", "Áreas de apoyo", 175, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UBS Servicios Técnicos a la Producción", "UBST", "Áreas", 44, ouId1);
            AddWorkPlaceIfNotExists("UBS Servicios Técnicos a la Producción", "UBST", "Grupo", 7, ouId2, "50");
            AddWorkPlaceIfNotExists("Grupo Observación Tecnológica", "UBST", "Grupo", 3, ouId2, "50");
            AddWorkPlaceIfNotExists("Grupo de Informática", "UBST", "Grupo", 18, ouId2, "50");
            AddWorkPlaceIfNotExists("Grupo de Gestión de la Información", "UBST", "Grupo", 4, ouId2, "50");
            AddWorkPlaceIfNotExists("Grupo de Implementación y Control SGC", "UBST", "Grupo", 6, ouId2, "50");
            AddWorkPlaceIfNotExists("Grupo de Técnico", "UBST", "Grupo", 6, ouId2, "50");
            ouId2 = AddWorkPlaceIfNotExists("Laboratorio Químico", "UBST", "Laboratorio", 62, ouId1);
            AddWorkPlaceIfNotExists("Laboratorio Químico", "UBST", "Grupo", 4, ouId2, "50");
            AddWorkPlaceIfNotExists("Análisis Especiales", "UBST", "Grupo", 6, ouId2, "50");
            AddWorkPlaceIfNotExists("Grupo de Desarrollo", "UBST", "Grupo", 7, ouId2, "50");
            AddWorkPlaceIfNotExists("Grupo Producto Final", "UBST", "Grupo", 9, ouId2, "50");
            AddWorkPlaceIfNotExists("TURNO A", "UBST", "Grupo", 9, ouId2, "50");
            AddWorkPlaceIfNotExists("TURNO B", "UBST", "Grupo", 9, ouId2, "50");
            AddWorkPlaceIfNotExists("TURNO C", "UBST", "Grupo", 9, ouId2, "50");
            AddWorkPlaceIfNotExists("TURNO D", "UBST", "Grupo", 9, ouId2, "50");
            ouId2 = AddWorkPlaceIfNotExists("Operaciones Muestreo", "UBST", "Áreas", 69, ouId1);
            AddWorkPlaceIfNotExists("Operaciones Muestreo", "UBST", "Grupo", 2, ouId2, "50");
            AddWorkPlaceIfNotExists("Grupo Mediciones de Gases", "UBST", "Grupo", 6, ouId2, "50");
            AddWorkPlaceIfNotExists("Brigada Muestreros Diurnos", "UBST", "Brigada", 9, ouId2, "50");
            AddWorkPlaceIfNotExists("Brigada A", "UBST", "Brigada", 13, ouId2, "50");
            AddWorkPlaceIfNotExists("Brigada B", "UBST", "Brigada", 13, ouId2, "50");
            AddWorkPlaceIfNotExists("Brigada C", "UBST", "Brigada", 13, ouId2, "50");
            AddWorkPlaceIfNotExists("Brigada D", "UBST", "Brigada", 13, ouId2, "50");

            #endregion

            #region UB Abastecimiento

            ouId1 = AddWorkPlaceIfNotExists("UB Abastecimiento", "UBA", "UEB", 111, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UB Abastecimiento", "UBA", "Áreas", 31, ouId1);
            AddWorkPlaceIfNotExists("UB Abastecimiento", "UBA", "Grupo", 6, ouId2, "56");
            AddWorkPlaceIfNotExists("Gestión y Abastecimiento", "UBA", "Grupo", 15, ouId2, "56");
            AddWorkPlaceIfNotExists("Brigada de Sostenimiento Logístico", "UBA", "Brigada", 20, ouId2, "56");
            ouId2 = AddWorkPlaceIfNotExists("Base Almacén Central", "UBA", "Bases", 33, ouId1);
            AddWorkPlaceIfNotExists("Base Almacén Central", "UBA", "Grupo", 16, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén Central", "UBA", "Almacenes", 10, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén de Reactivos Químicos", "UBA", "Almacenes", 4, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén Metales", "UBA", "Almacenes", 3, ouId2, "56");
            ouId2 = AddWorkPlaceIfNotExists("Base Almacén ORSK", "UBA", "Bases", 16, ouId1);
            AddWorkPlaceIfNotExists("Base Almacén ORSK", "UBA", "Grupo", 4, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén ORSK", "UBA", "Almacenes", 7, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén de Gases y lubricantes", "UBA", "Almacenes", 2, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén Mina", "UBA", "Almacenes", 3, ouId2, "56");
            ouId2 = AddWorkPlaceIfNotExists("Base Almacén Rolo", "UBA", "Bases", 21, ouId1);
            AddWorkPlaceIfNotExists("Base Almacén Rolo", "UBA", "Grupo", 5, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén de piezas de Repuesto", "UBA", "Almacenes", 8, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén Químico y Ociosos", "UBA", "Almacenes", 4, ouId2, "56");
            AddWorkPlaceIfNotExists("Almacén Inversiones y Activos Fijos", "UBA", "Almacenes", 4, ouId2, "56");

            #endregion

            #region Dirección de Seguridad y Protección

            ouId1 = AddWorkPlaceIfNotExists("Dirección de Seguridad y Protección", "SP", "Dirección", 174, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("Dirección de Seguridad y Protección", "SP", "Áreas", 9, ouId1);
            AddWorkPlaceIfNotExists("Dirección de Seguridad y Protección", "SP", "Grupo", 4, ouId2, "57");
            AddWorkPlaceIfNotExists("Grupo de Seguridad Interna", "SP", "Grupo", 5, ouId2, "57");
            ouId3 = AddWorkPlaceIfNotExists("Objetivo Almacén Rolo", "SP", "Áreas", 31, ouId2);
            AddWorkPlaceIfNotExists("Objetivo Almacén Rolo", "SP", "Grupo", 3, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno A", "SP", "Brigada", 7, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno B", "SP", "Brigada", 7, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno C", "SP", "Brigada", 7, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno D", "SP", "Brigada", 7, ouId3, "57");
            ouId3 = AddWorkPlaceIfNotExists("Objetivo Industria", "SP", "Áreas", 85, ouId2);
            AddWorkPlaceIfNotExists("Objetivo Industria", "SP", "Grupo", 17, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno A", "SP", "Brigada", 17, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno B", "SP", "Brigada", 17, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno C", "SP", "Brigada", 17, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno D", "SP", "Brigada", 17, ouId3, "57");
            ouId3 = AddWorkPlaceIfNotExists("Objetivo Mina", "SP", "Áreas", 49, ouId2);
            AddWorkPlaceIfNotExists("Objetivo Mina", "SP", "Grupo", 17, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno A", "SP", "Brigada", 8, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno B", "SP", "Brigada", 8, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno C", "SP", "Brigada", 8, ouId3, "57");
            AddWorkPlaceIfNotExists("Turno D", "SP", "Brigada", 8, ouId3, "57");

            #endregion

            #region UB Mantenimiento

            ouId1 = AddWorkPlaceIfNotExists("UB Mantenimiento", "MTTO", "UEB", 841, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UB Mantenimiento", "MTTO", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("UB Mantenimiento", "MTTO", "Grupo", 5, ouId2, "32");
            AddWorkPlaceIfNotExists("Grupo Económico Mantenimiento", "MTTO", "Grupo", 15, ouId2, "32");
            ouId2 = AddWorkPlaceIfNotExists("Departamento Técnico de Mantenimiento", "MTTO", "Áreas", 35, ouId1);
            AddWorkPlaceIfNotExists("Departamento Técnico de Mantenimiento", "MTTO", "Grupo", 1, ouId2, "32");
            AddWorkPlaceIfNotExists("Grupo Programación y Control del Mantenimiento", "MTTO", "Grupo", 11, ouId2, "32");
            AddWorkPlaceIfNotExists("Grupo Pieza y Repuesto", "MTTO", "Grupo", 4, ouId2, "32");
            AddWorkPlaceIfNotExists("Grupo Corrosión", "MTTO", "Grupo", 7, ouId2, "32");
            AddWorkPlaceIfNotExists("Grupo Ingeniería y Desarrollo", "MTTO", "Grupo", 12, ouId2, "32");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Electrofiltros", "MTTO", "Áreas", 30, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Electrofiltros", "MTTO", "Grupo", 5, ouId2, "33");
            AddWorkPlaceIfNotExists("Brigada de Electrofiltro", "MTTO", "Brigada", 14, ouId2, "33");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Grupo Piloto (CEINNIQ)", "MTTO", "Brigada", 11, ouId2, "33");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Izaje Industrial", "MTTO", "Áreas", 31, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Izaje Industrial", "MTTO", "Grupo", 4, ouId2, "13");
            AddWorkPlaceIfNotExists("Brigada Eléctrica", "MTTO", "Brigada", 9, ouId2, "13");
            AddWorkPlaceIfNotExists("Brigada Mecánica 1", "MTTO", "Brigada", 9, ouId2, "13");
            AddWorkPlaceIfNotExists("Brigada Mecánica 2", "MTTO", "Brigada", 9, ouId2, "13");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Misceláneo", "MTTO", "Áreas", 76, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Misceláneo", "MTTO", "Grupo", 6, ouId2, "15");
            AddWorkPlaceIfNotExists("Brigada Canalización y Pintura", "MTTO", "Brigada", 10, ouId2, "15");
            AddWorkPlaceIfNotExists("Brigada Construcción Civil", "MTTO", "Brigada", 20, ouId2, "15");
            AddWorkPlaceIfNotExists("Brigada Construcción Civil 2", "MTTO", "Brigada", 15, ouId2, "15");
            AddWorkPlaceIfNotExists("Brigada Carpintero Tech y Montaje", "MTTO", "Brigada", 19, ouId2, "15");
            AddWorkPlaceIfNotExists("Brigada de Andamios", "MTTO", "Brigada", 7, ouId2, "15");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Mecánico", "MTTO", "Áreas", 66, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Mecánico", "MTTO", "Grupo", 8, ouId2, "12");
            AddWorkPlaceIfNotExists("Brigada de Reparación de Equipos Auxiliares", "MTTO", "Brigada", 11, ouId2, "12");
            AddWorkPlaceIfNotExists("Brigada Mixta de Reparaciones 1", "MTTO", "Brigada", 14, ouId2, "12");
            AddWorkPlaceIfNotExists("Brigada Mixta de Reparaciones 2", "MTTO", "Brigada", 14, ouId2, "12");
            AddWorkPlaceIfNotExists("Brigada Mecánica del Área", "MTTO", "Brigada", 9, ouId2, "12");
            AddWorkPlaceIfNotExists("Brigada Reparación del Taller", "MTTO", "Brigada", 10, ouId2, "12");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Mecánico 2", "MTTO", "Áreas", 66, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Mecánico 2", "MTTO", "Grupo", 6, ouId2, "46");
            AddWorkPlaceIfNotExists("Grupo Técnico Mantenimiento Mecánico 2", "MTTO", "Grupo", 6, ouId2, "46");
            AddWorkPlaceIfNotExists("Brigada Mecánica", "MTTO", "Brigada", 6, ouId2, "46");
            AddWorkPlaceIfNotExists("Brigada de Estructuras Metálicas 1", "MTTO", "Brigada", 10, ouId2, "46");
            AddWorkPlaceIfNotExists("Brigada de Estructuras Metálicas 2", "MTTO", "Brigada", 8, ouId2, "46");
            AddWorkPlaceIfNotExists("Brigada de Maquinado", "MTTO", "Brigada", 11, ouId2, "46");
            AddWorkPlaceIfNotExists("Brigada de Preparación de la Producción", "MTTO", "Brigada", 8, ouId2, "46");
            AddWorkPlaceIfNotExists("Brigada de Reparación de Maq y Equipos", "MTTO", "Brigada", 11, ouId2, "46");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Instrumentos", "MTTO", "Áreas", 87, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Instrumentos", "MTTO", "Grupo", 5, ouId2, "16");
            AddWorkPlaceIfNotExists("Grupo de Mantenimiento a Plantas", "MTTO", "Grupo", 7, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Instrumentación Recuperación de NH3", "MTTO", "Brigada", 6, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Instrumentación Calcinación y Sínter", "MTTO", "Brigada", 4, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Instrumentación Hornos de Reducción", "MTTO", "Brigada", 14, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Instrumentación Lixiviación y Lavado", "MTTO", "Brigada", 10, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Instrumentación Preparación Mineral", "MTTO", "Brigada", 6, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Contra Incendios", "MTTO", "Brigada", 5, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Instrumentación Laboratorio Químico", "MTTO", "Brigada", 4, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Instrumentación Potabilizadora", "MTTO", "Brigada", 3, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Instrumentación Termoenergética", "MTTO", "Brigada", 7, ouId2, "16");
            AddWorkPlaceIfNotExists("Brigada Piloto CEINNIQ", "MTTO", "Brigada", 5, ouId2, "16");
            AddWorkPlaceIfNotExists("Grupo Instrumentación", "MTTO", "Grupo", 11, ouId2, "16");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Eléctrico", "MTTO", "Áreas", 197, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Eléctrico", "MTTO", "Grupo", 3, ouId2, "11");
            AddWorkPlaceIfNotExists("Grupo de Protecciones, Accionamientos y Mediciones Eléctricas", "MTTO", "Grupo", 14, ouId2, "11");
            AddWorkPlaceIfNotExists("Grupo Técnico Desarrollo", "MTTO", "Grupo", 9, ouId2, "11");
            ouId3 = AddWorkPlaceIfNotExists("Taller Mantenimiento Eléctrico Especializado", "MTTO", "Taller", 54, ouId2);
            AddWorkPlaceIfNotExists("Taller Mantenimiento Eléctrico Especializado", "MTTO", "Grupo", 9, ouId3, "11");
            AddWorkPlaceIfNotExists("Grupo de Refrigeración", "MTTO", "Grupo", 10, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Miscelánea", "MTTO", "Brigada", 9, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Reparaciones Mayores", "MTTO", "Brigada", 12, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Subestaciones y Agregados", "MTTO", "Brigada", 7, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada de Baterías y Reparación de Máquinas de Soldar", "MTTO", "Brigada", 7, ouId3, "11");
            ouId3 = AddWorkPlaceIfNotExists("Taller Mantenimiento Eléctrico a Plantas Principales", "MTTO", "Taller", 70, ouId2);
            AddWorkPlaceIfNotExists("Taller Mantenimiento Eléctrico a Plantas Principales", "MTTO", "Grupo", 3, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Eléctrico Preparación de Mineral", "MTTO", "Brigada", 16, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Eléctrico Hornos de Reducción", "MTTO", "Brigada", 14, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Eléctrico Lixiviación y Lavado", "MTTO", "Brigada", 14, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Eléctrico Recuperación de Amoníaco", "MTTO", "Brigada", 12, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Eléctrico Calcinación y Sínter", "MTTO", "Brigada", 11, ouId3, "11");
            ouId3 = AddWorkPlaceIfNotExists("Taller Mantenimiento Eléctrico a Plantas Auxiliares", "MTTO", "Taller", 46, ouId2);
            AddWorkPlaceIfNotExists("Taller Mantenimiento Eléctrico a Plantas Auxiliares", "MTTO", "Grupo", 3, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Eléctrico UB Mina", "MTTO", "Brigada", 11, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Eléctrico Termoenergética", "MTTO", "Brigada", 11, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Eléctrico Recepción y Suministro", "MTTO", "Brigada", 5, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Despacho Eléctrico A", "MTTO", "Brigada", 4, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Despacho Eléctrico B", "MTTO", "Brigada", 4, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Despacho Eléctrico C", "MTTO", "Brigada", 4, ouId3, "11");
            AddWorkPlaceIfNotExists("Brigada Despacho Eléctrico D", "MTTO", "Brigada", 4, ouId3, "11");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Reverbería, Mecánica y Pailería", "MTTO", "Áreas", 131, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Reverbería, Mecánica y Pailería", "MTTO", "Grupo", 8, ouId2, "51");
            AddWorkPlaceIfNotExists("Brigada Reparación de Tapas de Cámaras", "MTTO", "Brigada", 8, ouId2, "51");
            AddWorkPlaceIfNotExists("Brigada Reverbería 1", "MTTO", "Brigada", 23, ouId2, "51");
            AddWorkPlaceIfNotExists("Brigada Reverbería 2", "MTTO", "Brigada", 23, ouId2, "51");
            AddWorkPlaceIfNotExists("Brigada Reverbería 3", "MTTO", "Brigada", 24, ouId2, "51");
            AddWorkPlaceIfNotExists("Brigada Mecánica y Pailería 1", "MTTO", "Brigada", 15, ouId2, "51");
            AddWorkPlaceIfNotExists("Brigada Mecánica y Pailería 2", "MTTO", "Brigada", 15, ouId2, "51");
            AddWorkPlaceIfNotExists("Brigada Mecánica y Pailería 3", "MTTO", "Brigada", 15, ouId2, "51");
            ouId2 = AddWorkPlaceIfNotExists("Operaciones Transporte", "MTTO", "Áreas", 55, ouId1);
            AddWorkPlaceIfNotExists("Operaciones Transporte", "MTTO", "Grupo", 8, ouId2, "22");
            AddWorkPlaceIfNotExists("Brigada Transporte Carga e Izaje", "MTTO", "Brigada", 23, ouId2, "22");
            AddWorkPlaceIfNotExists("Brigada Operaciones", "MTTO", "Brigada", 19, ouId2, "22");
            AddWorkPlaceIfNotExists("Grupo Control del Transporte", "MTTO", "Grupo", 5, ouId2, "22");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Automotor", "MTTO", "Áreas", 47, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Automotor", "MTTO", "Grupo", 4, ouId2, "34");
            AddWorkPlaceIfNotExists("Brigada Miscelánea", "MTTO", "Brigada", 10, ouId2, "34");
            AddWorkPlaceIfNotExists("Brigada Reparación de Equipos de Izaje", "MTTO", "Brigada", 8, ouId2, "34");
            AddWorkPlaceIfNotExists("Brigada Reparación de Equipos Ligeros", "MTTO", "Brigada", 11, ouId2, "34");
            AddWorkPlaceIfNotExists("Brigada Reparación de Equipos Pesados", "MTTO", "Brigada", 10, ouId2, "34");
            AddWorkPlaceIfNotExists("Grupo de Control y Mantenimiento del Transporte", "MTTO", "Grupo", 4, ouId2, "34");

            #endregion

            #region UBS Recepción y Suministros

            ouId1 = AddWorkPlaceIfNotExists("UBS Recepción y Suministros", "UBRS", "Plantas", 77, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UBS Recepción y Suministros", "UBRS", "Áreas", 10, ouId1);
            AddWorkPlaceIfNotExists("UBS Recepción y Suministros", "UBRS", "Grupo", 4, ouId2, "09");
            AddWorkPlaceIfNotExists("Grupo Laboratorio", "UBRS", "Grupo", 6, ouId2, "09");
            AddWorkPlaceIfNotExists("Brigada de Atención a las Operaciones Amoníaco", "UBRS", "Brigada", 9, ouId2, "09");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Recepción y Suministro", "UBRS", "Áreas", 16, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Recepción y Suministro", "UBRS", "Grupo", 4, ouId2, "47");
            AddWorkPlaceIfNotExists("Brigada de Mantenimiento Amoníaco", "UBRS", "Brigada", 6, ouId2, "47");
            AddWorkPlaceIfNotExists("Brigada de Mantenimiento Potabilizadora", "UBRS", "Brigada", 6, ouId2, "47");
            ouId2 = AddWorkPlaceIfNotExists("Control de las Operaciones", "UBRS", "Áreas", 45, ouId1);
            AddWorkPlaceIfNotExists("Control de las Operaciones", "UBRS", "Grupo", 6, ouId2, "09");
            AddWorkPlaceIfNotExists("Turno A Potabilizadora", "UBRS", "Brigada", 6, ouId2, "09");
            AddWorkPlaceIfNotExists("Turno B Potabilizadora", "UBRS", "Brigada", 6, ouId2, "09");
            AddWorkPlaceIfNotExists("Turno C Potabilizadora", "UBRS", "Brigada", 6, ouId2, "09");
            AddWorkPlaceIfNotExists("Turno D Potabilizadora", "UBRS", "Brigada", 6, ouId2, "09");
            AddWorkPlaceIfNotExists("Turno A Amoníaco", "UBRS", "Brigada", 3, ouId2, "09");
            AddWorkPlaceIfNotExists("Turno B Amoníaco", "UBRS", "Brigada", 3, ouId2, "09");
            AddWorkPlaceIfNotExists("Turno C Amoníaco", "UBRS", "Brigada", 3, ouId2, "09");
            AddWorkPlaceIfNotExists("Turno D Amoníaco", "UBRS", "Brigada", 3, ouId2, "09");
            
            #endregion

            #region UBS Termoenergética

            ouId1 = AddWorkPlaceIfNotExists("UBS Termoenergética", "UBPT", "Plantas", 0, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UBS Termoenergética", "UBPT", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("UBS Termoenergética", "UBPT", "Grupo", 7, ouId2, "08");
            AddWorkPlaceIfNotExists("Brigada de Limpieza", "UBPT", "Brigada", 4, ouId2, "08");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Termoenergética", "UBPT", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Termoenergética", "UBPT", "Grupo", 9, ouId2, "44");
            AddWorkPlaceIfNotExists("Brigada Redes", "UBPT", "Brigada", 6, ouId2, "44");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Termoeléctrica", "UBPT", "Brigada", 13, ouId2, "44");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Energético", "UBPT", "Brigada", 9, ouId2, "44");
            ouId2 = AddWorkPlaceIfNotExists("Control de las Operaciones", "UBPT", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Control de las Operaciones", "UBPT", "Grupo", 5, ouId2, "08");
            ouId3 = AddWorkPlaceIfNotExists("Turno A", "UBPT", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno A", "UBPT", "Grupo", 3, ouId3, "08");
            AddWorkPlaceIfNotExists("Brigada Energética", "UBPT", "Brigada", 6, ouId3, "08");
            AddWorkPlaceIfNotExists("Brigada Termo", "UBPT", "Brigada", 13, ouId3, "08");
            ouId3 = AddWorkPlaceIfNotExists("Turno B", "UBPT", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno B", "UBPT", "Grupo", 3, ouId3, "08");
            AddWorkPlaceIfNotExists("Brigada Energética", "UBPT", "Brigada", 6, ouId3, "08");
            AddWorkPlaceIfNotExists("Brigada Termo", "UBPT", "Brigada", 13, ouId3, "08");
            ouId3 = AddWorkPlaceIfNotExists("Turno C", "UBPT", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno C", "UBPT", "Grupo", 3, ouId3, "08");
            AddWorkPlaceIfNotExists("Brigada Energética", "UBPT", "Brigada", 6, ouId3, "08");
            AddWorkPlaceIfNotExists("Brigada Termo", "UBPT", "Brigada", 13, ouId3, "08");
            ouId3 = AddWorkPlaceIfNotExists("Turno D", "UBPT", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno D", "UBPT", "Grupo", 3, ouId3, "08");
            AddWorkPlaceIfNotExists("Brigada Energética", "UBPT", "Brigada", 6, ouId3, "08");
            AddWorkPlaceIfNotExists("Brigada Termo", "UBPT", "Brigada", 13, ouId3, "08");

            #endregion

            #region UBP Preparación de Mineral

            ouId1 = AddWorkPlaceIfNotExists("UBP Preparación de Mineral", "UBPM", "Plantas", 223, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UBP Preparación de Mineral", "UBPM", "Áreas", 6, ouId1);
            AddWorkPlaceIfNotExists("UBP Preparación de Mineral", "UBPM", "Grupo", 6, ouId2, "03");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Preparación Mineral", "UBPM", "Áreas", 63, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Preparación Mineral", "UBPM", "Grupo", 8, ouId2, "10");
            AddWorkPlaceIfNotExists("Brigada de Engrase", "UBPM", "Brigada", 7, ouId2, "10");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Grúa", "UBPM", "Brigada", 10, ouId2, "10");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Secaderos", "UBPM", "Brigada", 15, ouId2, "10");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Molino", "UBPM", "Brigada", 7, ouId2, "10");
            AddWorkPlaceIfNotExists("Brigada Transportadores", "UBPM", "Brigada", 8, ouId2, "10");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Recepción y Trituración", "UBPM", "Brigada", 8, ouId2, "10");
            ouId2 = AddWorkPlaceIfNotExists("Control de las Operaciones", "UBPM", "Áreas", 154, ouId1);
            AddWorkPlaceIfNotExists("Control de las Operaciones", "UBPM", "Grupo", 6, ouId2, "03");
            ouId3 = AddWorkPlaceIfNotExists("Turno A", "UBPM", "Áreas", 37, ouId2);
            AddWorkPlaceIfNotExists("Turno A", "UBPM", "Grupo", 9, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Grúa", "UBPM", "Brigada", 8, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Molino", "UBPM", "Brigada", 7, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Secadero", "UBPM", "Brigada", 6, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Recepción y Beneficio", "UBPM", "Brigada", 7, ouId3, "03");
            ouId3 = AddWorkPlaceIfNotExists("Turno B", "UBPM", "Áreas", 37, ouId2);
            AddWorkPlaceIfNotExists("Turno B", "UBPM", "Grupo", 9, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Grúa", "UBPM", "Brigada", 8, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Molino", "UBPM", "Brigada", 7, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Secadero", "UBPM", "Brigada", 6, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Recepción y Beneficio", "UBPM", "Brigada", 7, ouId3, "03");
            ouId3 = AddWorkPlaceIfNotExists("Turno C", "UBPM", "Áreas", 37, ouId2);
            AddWorkPlaceIfNotExists("Turno C", "UBPM", "Grupo", 9, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Grúa", "UBPM", "Brigada", 8, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Molino", "UBPM", "Brigada", 7, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Secadero", "UBPM", "Brigada", 6, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Recepción y Beneficio", "UBPM", "Brigada", 7, ouId3, "03");
            ouId3 = AddWorkPlaceIfNotExists("Turno D", "UBPM", "Áreas", 37, ouId2);
            AddWorkPlaceIfNotExists("Turno D", "UBPM", "Grupo", 9, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Grúa", "UBPM", "Brigada", 8, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Molino", "UBPM", "Brigada", 7, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Secadero", "UBPM", "Brigada", 6, ouId3, "03");
            AddWorkPlaceIfNotExists("Brigada Recepción y Beneficio", "UBPM", "Brigada", 7, ouId3, "03");

            #endregion

            #region UBP Hornos de Reducción

            ouId1 = AddWorkPlaceIfNotExists("UBP Hornos de Reducción", "UBHR", "Plantas", 0, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UBP Hornos de Reducción", "UBHR", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("UBP Hornos de Reducción", "UBHR", "Grupo", 7, ouId2, "04");
            AddWorkPlaceIfNotExists("Brigada de Limpieza", "UBHR", "Brigada", 9, ouId2, "04");
            AddWorkPlaceIfNotExists("Brigada de Limpieza de Garganta", "UBHR", "Brigada", 16, ouId2, "04");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Hornos de Reducción", "UBHR", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Hornos de Reducción", "UBHR", "Grupo", 11, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada Brazos y Dientes", "UBHR", "Brigada", 21, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada Romana", "UBHR", "Brigada", 10, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada Taller", "UBHR", "Brigada", 9, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada Transporte Neumático", "UBHR", "Brigada", 8, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada Planta Baja", "UBHR", "Brigada", 18, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada de Mantenimiento 1", "UBHR", "Brigada", 9, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada de Mantenimiento 2", "UBHR", "Brigada", 9, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada de Engrase", "UBHR", "Brigada", 5, ouId2, "17");
            AddWorkPlaceIfNotExists("Brigada Rep. de Cámaras", "UBHR", "Brigada", 11, ouId2, "17");
            ouId2 = AddWorkPlaceIfNotExists("Control de las Operaciones", "UBHR", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Control de las Operaciones", "UBHR", "Grupo", 8, ouId2, "04");
            ouId3 = AddWorkPlaceIfNotExists("Turno A", "UBHR", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno A", "UBHR", "Grupo", 9, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa I", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa II", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa III", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Transporte Neumático", "UBHR", "Brigada", 8, ouId3, "04");
            ouId3 = AddWorkPlaceIfNotExists("Turno B", "UBHR", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno B", "UBHR", "Grupo", 9, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa I", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa II", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa III", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Transporte Neumático", "UBHR", "Brigada", 8, ouId3, "04");
            ouId3 = AddWorkPlaceIfNotExists("Turno C", "UBHR", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno C", "UBHR", "Grupo", 9, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa I", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa II", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa III", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Transporte Neumático", "UBHR", "Brigada", 8, ouId3, "04");
            ouId3 = AddWorkPlaceIfNotExists("Turno D", "UBHR", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno D", "UBHR", "Grupo", 9, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa I", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa II", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Losa III", "UBHR", "Brigada", 7, ouId3, "04");
            AddWorkPlaceIfNotExists("Transporte Neumático", "UBHR", "Brigada", 8, ouId3, "04");

            #endregion

            #region UBS Lixiviación y Lavado

            ouId1 = AddWorkPlaceIfNotExists("UBS Lixiviación y Lavado", "UBLL", "Plantas", 0, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UBS Lixiviación y Lavado", "UBLL", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("UBS Lixiviación y Lavado", "UBLL", "Grupo", 5, ouId2, "05");
            AddWorkPlaceIfNotExists("Brigada de Limpieza", "UBLL", "Brigada", 14, ouId2, "05");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Lixiviación y Lavado", "UBLL", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Lixiviación y Lavado", "UBLL", "Grupo", 10, ouId2, "26");
            AddWorkPlaceIfNotExists("Brigada Contacto", "UBLL", "Brigada", 12, ouId2, "26");
            AddWorkPlaceIfNotExists("Brigada Engrasadores", "UBLL", "Brigada", 5, ouId2, "26");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Lavado", "UBLL", "Brigada", 15, ouId2, "26");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Lixiviación", "UBLL", "Brigada", 15, ouId2, "26");
            ouId2 = AddWorkPlaceIfNotExists("Control de las Operaciones", "UBLL", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Control de las Operaciones", "UBLL", "Grupo", 5, ouId2, "05");
            ouId3 = AddWorkPlaceIfNotExists("Turno A", "UBLL", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno A", "UBLL", "Grupo", 4, ouId3, "05");
            AddWorkPlaceIfNotExists("Brigada Lixiviación y Lavado", "UBLL", "Brigada", 11, ouId3, "05");
            ouId3 = AddWorkPlaceIfNotExists("Turno B", "UBLL", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno B", "UBLL", "Grupo", 4, ouId3, "05");
            AddWorkPlaceIfNotExists("Brigada Lixiviación y Lavado", "UBLL", "Brigada", 11, ouId3, "05");
            ouId3 = AddWorkPlaceIfNotExists("Turno C", "UBLL", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno C", "UBLL", "Grupo", 4, ouId3, "05");
            AddWorkPlaceIfNotExists("Brigada Lixiviación y Lavado", "UBLL", "Brigada", 11, ouId3, "05");
            ouId3 = AddWorkPlaceIfNotExists("Turno D", "UBLL", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno D", "UBLL", "Grupo", 4, ouId3, "05");
            AddWorkPlaceIfNotExists("Brigada Lixiviación y Lavado", "UBLL", "Brigada", 11, ouId3, "05");

            #endregion

            #region UBP Recuperación de NH3 y Cobalto

            ouId1 = AddWorkPlaceIfNotExists("UBP Recuperación de NH3 y Cobalto", "UBRC", "Plantas", 0, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UBP Recuperación de NH3 y Cobalto", "UBRC", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("UBP Recuperación de NH3 y Cobalto", "UBRC", "Grupo", 6, ouId2, "06");
            AddWorkPlaceIfNotExists("Brigada de Limpieza", "UBRC", "Brigada", 14, ouId2, "06");
            AddWorkPlaceIfNotExists("Brigada Envase de Sulfuro", "UBRC", "Brigada", 15, ouId2, "06");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Recuperación NH3 y Co", "UBRC", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Recuperación NH3 y Co", "UBRC", "Grupo", 8, ouId2, "25");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Cobalto", "UBRC", "Brigada", 6, ouId2, "25");
            AddWorkPlaceIfNotExists("Brigada Pailería", "UBRC", "Brigada", 14, ouId2, "25");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Mecánico 1", "UBRC", "Brigada", 9, ouId2, "25");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Mecánico 2", "UBRC", "Brigada", 11, ouId2, "25");
            ouId2 = AddWorkPlaceIfNotExists("Control de las Operaciones", "UBRC", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Control de las Operaciones", "UBRC", "Grupo", 5, ouId2, "06");
            ouId3 = AddWorkPlaceIfNotExists("Turno A", "UBRC", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno A", "UBRC", "Grupo", 4, ouId3, "06");
            AddWorkPlaceIfNotExists("Brigada Recuperación y Cobalto", "UBRC", "Brigada", 11, ouId3, "06");
            ouId3 = AddWorkPlaceIfNotExists("Turno B", "UBRC", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno B", "UBRC", "Grupo", 4, ouId3, "06");
            AddWorkPlaceIfNotExists("Brigada Recuperación y Cobalto", "UBRC", "Brigada", 11, ouId3, "06");
            ouId3 = AddWorkPlaceIfNotExists("Turno C", "UBRC", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno C", "UBRC", "Grupo", 4, ouId3, "06");
            AddWorkPlaceIfNotExists("Brigada Recuperación y Cobalto", "UBRC", "Brigada", 11, ouId3, "06");
            ouId3 = AddWorkPlaceIfNotExists("Turno D", "UBRC", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno D", "UBRC", "Grupo", 4, ouId3, "06");
            AddWorkPlaceIfNotExists("Brigada Recuperación y Cobalto", "UBRC", "Brigada", 11, ouId3, "06");

            #endregion

            #region UBP Calcinación y Sínter

            ouId1 = AddWorkPlaceIfNotExists("UBP Calcinación y Sínter", "UBCS", "Plantas", 0, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UBP Calcinación y Sínter", "UBCS", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("UBP Calcinación y Sínter", "UBCS", "Grupo", 5, ouId2, "07");
            AddWorkPlaceIfNotExists("Brigada de Envase Producto Final", "UBCS", "Brigada", 9, ouId2, "07");
            AddWorkPlaceIfNotExists("Brigada de Limpieza", "UBCS", "Brigada", 6, ouId2, "07");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Calcinación y Sínter", "UBCS", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Calcinación y Sínter", "UBCS", "Grupo", 7, ouId2, "27");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Calcinación", "UBCS", "Brigada", 17, ouId2, "27");
            AddWorkPlaceIfNotExists("Brigada Mantenimiento Sínter", "UBCS", "Brigada", 14, ouId2, "27");
            ouId2 = AddWorkPlaceIfNotExists("Control de las Operaciones", "UBCS", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Control de las Operaciones", "UBCS", "Grupo", 4, ouId2, "07");
            ouId3 = AddWorkPlaceIfNotExists("Turno A", "UBCS", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno A", "UBCS", "Grupo", 3, ouId3, "07");
            AddWorkPlaceIfNotExists("Brigada Operaciones Calcinación", "UBCS", "Brigada", 7, ouId3, "07");
            AddWorkPlaceIfNotExists("Brigada Operaciones Sínter", "UBCS", "Brigada", 10, ouId3, "07");
            ouId3 = AddWorkPlaceIfNotExists("Turno B", "UBCS", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno B", "UBCS", "Grupo", 3, ouId3, "07");
            AddWorkPlaceIfNotExists("Brigada Operaciones Calcinación", "UBCS", "Brigada", 7, ouId3, "07");
            AddWorkPlaceIfNotExists("Brigada Operaciones Sínter", "UBCS", "Brigada", 10, ouId3, "07");
            ouId3 = AddWorkPlaceIfNotExists("Turno C", "UBCS", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno C", "UBCS", "Grupo", 3, ouId3, "07");
            AddWorkPlaceIfNotExists("Brigada Operaciones Calcinación", "UBCS", "Brigada", 7, ouId3, "07");
            AddWorkPlaceIfNotExists("Brigada Operaciones Sínter", "UBCS", "Brigada", 10, ouId3, "07");
            ouId3 = AddWorkPlaceIfNotExists("Turno D", "UBCS", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Turno D", "UBCS", "Grupo", 3, ouId3, "07");
            AddWorkPlaceIfNotExists("Brigada Operaciones Calcinación", "UBCS", "Brigada", 7, ouId3, "07");
            AddWorkPlaceIfNotExists("Brigada Operaciones Sínter", "UBCS", "Brigada", 10, ouId3, "07");

            #endregion

            #region UB Minera

            ouId1 = AddWorkPlaceIfNotExists("UB Minera", "UBM", "UEB", 0, ouId0);
            ouId2 = AddWorkPlaceIfNotExists("UB Minera", "UBM", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("UB Minera", "UBM", "Grupo", 6, ouId2, "02");
            AddWorkPlaceIfNotExists("Grupo Económico", "UBM", "Grupo", 6, ouId2, "02");
            AddWorkPlaceIfNotExists("Grupo Geología Desarrollo y Depósito", "UBM", "Grupo", 13, ouId2, "02");
            AddWorkPlaceIfNotExists("Grupo Planificación y Topografía", "UBM", "Grupo", 18, ouId2, "02");
            ouId2 = AddWorkPlaceIfNotExists("Operaciones Extracción y Transporte", "UBM", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Operaciones Extracción y Transporte", "UBM", "Grupo", 6, ouId2, "02");
            AddWorkPlaceIfNotExists("Grupo Control de la Producción Minera", "UBM", "Grupo", 16, ouId2, "02");
            ouId3 = AddWorkPlaceIfNotExists("Extracción y Transporte", "UBM", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Extracción y Transporte", "UBM", "Grupo", 5, ouId3, "02");
            var ouId4 = AddWorkPlaceIfNotExists("Turno A Extracción y Transporte", "UBM", "Áreas", 0, ouId3);
            AddWorkPlaceIfNotExists("Turno A Extracción y Transporte", "UBM", "Grupo", 1, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada Extracción y Transporte 1", "UBM", "Brigada", 14, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada Extracción y Transporte 2", "UBM", "Brigada", 22, ouId4, "02");
            ouId4 = AddWorkPlaceIfNotExists("Turno B Extracción y Transporte", "UBM", "Áreas", 0, ouId3);
            AddWorkPlaceIfNotExists("Turno B Extracción y Transporte", "UBM", "Grupo", 1, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada Extracción y Transporte 1", "UBM", "Brigada", 14, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada Extracción y Transporte 2", "UBM", "Brigada", 22, ouId4, "02");
            ouId4 = AddWorkPlaceIfNotExists("Turno C Extracción y Transporte", "UBM", "Áreas", 0, ouId3);
            AddWorkPlaceIfNotExists("Turno C Extracción y Transporte", "UBM", "Grupo", 1, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada Extracción y Transporte 1", "UBM", "Brigada", 14, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada Extracción y Transporte 2", "UBM", "Brigada", 22, ouId4, "02");
            ouId4 = AddWorkPlaceIfNotExists("Turno D Extracción y Transporte", "UBM", "Áreas", 0, ouId3);
            AddWorkPlaceIfNotExists("Turno D Extracción y Transporte", "UBM", "Grupo", 1, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada Extracción y Transporte 1", "UBM", "Brigada", 14, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada Extracción y Transporte 2", "UBM", "Brigada", 22, ouId4, "02");
            AddWorkPlaceIfNotExists("Brigada de Camino 1", "UBM", "Brigada", 13, ouId3, "02");
            AddWorkPlaceIfNotExists("Brigada de Camino 2", "UBM", "Brigada", 13, ouId3, "02");
            ouId3 = AddWorkPlaceIfNotExists("Depósitos Mineros", "UBM", "Áreas", 0, ouId2);
            AddWorkPlaceIfNotExists("Depósitos Mineros", "UBM", "Grupo", 12, ouId3, "02");
            AddWorkPlaceIfNotExists("Brigada Depósito A", "UBM", "Brigada", 15, ouId3, "02");
            AddWorkPlaceIfNotExists("Brigada Depósito B", "UBM", "Brigada", 15, ouId3, "02");
            AddWorkPlaceIfNotExists("Brigada Depósito C", "UBM", "Brigada", 15, ouId3, "02");
            AddWorkPlaceIfNotExists("Brigada Depósito D", "UBM", "Brigada", 15, ouId3, "02");
            ouId2 = AddWorkPlaceIfNotExists("Mantenimiento Mina", "UBM", "Áreas", 0, ouId1);
            AddWorkPlaceIfNotExists("Mantenimiento Mina", "UBM", "Grupo", 3, ouId2, "01");
            AddWorkPlaceIfNotExists("Brigada Aseguramiento", "UBM", "Brigada", 11, ouId2, "01");
            AddWorkPlaceIfNotExists("Brigada Revisiones Diarias", "UBM", "Brigada", 11, ouId2, "01");
            AddWorkPlaceIfNotExists("Brigada Equipos Pesados", "UBM", "Brigada", 13, ouId2, "01");
            AddWorkPlaceIfNotExists("Brigada Equipos sobre Neumáticos", "UBM", "Brigada", 12, ouId2, "01");
            AddWorkPlaceIfNotExists("Grupo Técnico Mantenimiento", "UBM", "Grupo", 3, ouId2, "01");

            #endregion
        }

        private long? AddWorkPlaceIfNotExists(string displayName, string description, string classification, int maxMembersApproved, long? parentId = null, string paymentCode = null, params int[] numbers)
        {
            if (_context.WorkPlaceUnits.IgnoreQueryFilters().Include(s => s.Classification).Any(s =>
                    s.CompanyId == _companyId && s.DisplayName == displayName &&
                    s.Classification.Description == classification.ToUpper() && s.ParentId == parentId))
                return _context.WorkPlaceUnits.IgnoreQueryFilters().Include(s => s.Classification).FirstOrDefault(s =>
                    s.CompanyId == _companyId && s.DisplayName == displayName &&
                    s.Classification.Description == classification.ToUpper() && s.ParentId == parentId)?.Id;

            var workplace = new WorkPlaceUnit(_companyId, displayName, description, parentId: parentId);
            var placeClassification = _context.WorkPlaceClassifications.FirstOrDefault(c => c.Description == classification.ToUpper());
            workplace.Classification = placeClassification;

            if (!paymentCode.IsNullOrEmpty())
            {
                var paymentArea = _context.WorkPlacePayments.FirstOrDefault(c => c.Code == paymentCode && c.CompanyId == _companyId);
                workplace.WorkPlacePayment = paymentArea;
            }

            if (numbers is { Length: > 0 })
            {
                workplace.Code = OrganizationUnit.CreateCode(numbers);
            }
            else
            {
                var lastChild = _context.WorkPlaceUnits.IgnoreQueryFilters().Where(ou => ou.ParentId == parentId).OrderByDescending(ou => ou.Code).FirstOrDefault();
                if (lastChild == null)
                {
                    var parent = parentId != null ? _context.WorkPlaceUnits.IgnoreQueryFilters().FirstOrDefault(ou => ou.Id == parentId) : null;
                    workplace.Code = OrganizationUnit.AppendCode(parent?.Code, OrganizationUnit.CreateCode(1));
                }
                else
                {
                    workplace.Code = OrganizationUnit.CalculateNextCode(lastChild.Code);
                }
            }

            workplace.Order = _context.WorkPlaceUnits.IgnoreQueryFilters().Count(ou => ou.ParentId == parentId);

            workplace.MaxMembersApproved = maxMembersApproved;

            _context.WorkPlaceUnits.Add(workplace);
            _context.SaveChanges();
            return workplace.Id;
        }
    }
}
