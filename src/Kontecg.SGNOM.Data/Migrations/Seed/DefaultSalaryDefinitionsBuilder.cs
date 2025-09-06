using System;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.Adjustments;
using Kontecg.Data;
using Kontecg.Domain;
using Kontecg.EFCore;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Retentions;
using Kontecg.Salary;
using Kontecg.SocialSecurity;
using Kontecg.Taxes;
using Microsoft.EntityFrameworkCore;
using NMoneys;

namespace Kontecg.Migrations.Seed
{
    public class DefaultSalaryDefinitionsBuilder
    {
        private readonly SGNOMDbContext _context;
        private readonly ILogger _logger;

        public DefaultSalaryDefinitionsBuilder(SGNOMDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Create()
        {
            var companyId = KontecgCoreConsts.MultiCompanyEnabled ? MultiCompanyConsts.DefaultCompanyId : 1;

            CreateReponsibilities();
            CreateOccupationalCategories();
            CreateComplexityGroups(companyId);
            CreateOccupations(companyId);
            CreateQualificationRequirementDefinition(companyId);

            CreateAdjustmentDefinitions(companyId);
            CreateRetentionDefinitions(companyId);
            CreatePaymentDefinitions(companyId);
            CreatePlusDefinitions(companyId);

            CreateSubsidyPaymentDefinitions(companyId);
        }

        private void CreateAdjustmentDefinitions(int companyId)
        {
            AddAdjustmentDefinitionIfNotExists(1, "Ajuste al Básico que se estimula", companyId, "C_TP504", 25);
            AddAdjustmentDefinitionIfNotExists(2, "Ajuste al Básico que no se estimula", companyId, "C_TP504", 25);
            AddAdjustmentDefinitionIfNotExists(3, "Ajuste de Subsidio por Invalidez Temporal", companyId, "C_INVTEMP", 25);
            AddAdjustmentDefinitionIfNotExists(4, "Ajuste de Subsidio por Maternidad (Prenatal, 1ra Postnatal y 2da Postnatal)", companyId, "C_MATERN", 25);
            AddAdjustmentDefinitionIfNotExists(5, "Ajuste de Subsidio por Prestación Social (Maternidad al 60%)", companyId, "C_MAT60%", 25);
            AddAdjustmentDefinitionIfNotExists(6, "Carta Consesoria", companyId, "C_INVPAR", 25);
            AddAdjustmentDefinitionIfNotExists(7, "Ajuste de Vacaciones", companyId, "C_PVAC", 25);
            AddAdjustmentDefinitionIfNotExists(8, "Ajuste de Homologación", companyId, "C_CERT", 25);
            AddAdjustmentDefinitionIfNotExists(9, "Ajuste de Grado Científico sin vacaciones", companyId, "C_GRADO_SVAC", 25);
            AddAdjustmentDefinitionIfNotExists(10, "Pago de remuneración de la ANIR", companyId, "C_ANIR", 26);
            AddAdjustmentDefinitionIfNotExists(11, "Pago por Eficiencia Económica", companyId, "C_PEFICE", 27);

            AddAdjustmentDefinitionIfNotExists(50, "Devolución de Crédito a la Población", companyId, "C_D_CPOBLA", 10);
            AddAdjustmentDefinitionIfNotExists(51, "Devolución de Viviendas Vinculadas", companyId, "C_D_VVINC", 10);
            AddAdjustmentDefinitionIfNotExists(52, "Devolución de Decreto Ley 116 (Multas)", companyId, "C_D_MULTA", 10);
            AddAdjustmentDefinitionIfNotExists(53, "Devolución de Ley General de la Vivienda", companyId, "C_D_LGV", 10);
            AddAdjustmentDefinitionIfNotExists(54, "Devolución de Seguro de Vida", companyId, "C_D_SVIDA", 10);
            AddAdjustmentDefinitionIfNotExists(55, "Devolución de Decreto Ley 249", companyId, "C_D_DL92", 10);
            AddAdjustmentDefinitionIfNotExists(56, "Devolución de Embargo Judicial", companyId, "C_D_EJU", 10);
            AddAdjustmentDefinitionIfNotExists(57, "Devolución de Seguridad Social", companyId, "C_D_CSS", 10);
            AddAdjustmentDefinitionIfNotExists(58, "Devolución de Impuesto por Ingresos Personales", companyId, "C_D_IMP_ING", 10);
            AddAdjustmentDefinitionIfNotExists(59, "Devolución de Créditos Sociales", companyId, "C_D_CRESO", 10);
            AddAdjustmentDefinitionIfNotExists(60, "Devolución de Insuficiencia Económica", companyId, "C_D_DL91", 10);
            AddAdjustmentDefinitionIfNotExists(61, "Devolución de Pensión Alimenticia", companyId, "C_D_PALIM", 10);
            AddAdjustmentDefinitionIfNotExists(62, "Devolución de Formación de Fondos", companyId, "C_D_FFOND", 10);
            AddAdjustmentDefinitionIfNotExists(63, "Devolución de Transporte", companyId, "C_D_TRA", 10);
            AddAdjustmentDefinitionIfNotExists(64, "Devolución de Responsabilidad Civil", companyId, "C_D_RCIVIL", 10);
            AddAdjustmentDefinitionIfNotExists(67, "Devolución de Aportes", companyId, "C_DAPORTES", 10);
            AddAdjustmentDefinitionIfNotExists(68, "Devolución de Salarios No Reclamados", companyId, "C_DS_NREC", 10);

            AddAdjustmentDefinitionIfNotExists(80, "Ajuste al submayor de vacaciones", companyId, "EMPTY", 28);

            _logger.Info("Adjustment definitions created.");
        }

        private void CreateRetentionDefinitions(int companyId)
        {
            var adjustmentDefinitions = _context.AdjustmentDefinitions.IgnoreQueryFilters().Select(a => new {a.Id, a.Code} ).ToDictionary(k => k.Code, v => v.Id);

            AddRetentionDefinitionIfNotExists(52, "Crédito a la Población (Inversiones)", companyId, "C_CPOBLA", "Créditos Bancarios", false, false, true, true, 14, adjustmentDefinitions[50]);
            AddRetentionDefinitionIfNotExists(54, "Viviendas Vinculadas", companyId, "C_VVINC", "Créditos Bancarios", false, false, true, false, 6, adjustmentDefinitions[51]);
            AddRetentionDefinitionIfNotExists(56, "Decreto Ley 116 (Multas)", companyId, "C_MULTA", "Otras Retenciones", true, false, false, true, 7, adjustmentDefinitions[52]);
            AddRetentionDefinitionIfNotExists(65, "Ley General de la Vivienda", companyId, "C_LGV", "Créditos Bancarios", false, false, true, true, 3, adjustmentDefinitions[53]);
            AddRetentionDefinitionIfNotExists(66, "Arrendamiento con opción de compra", companyId, "C_LGV", "Créditos Bancarios", false, false, true, true, 23, adjustmentDefinitions[53]);

            AddRetentionDefinitionIfNotExists(69, "Seguro de Vida", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);

            AddRetentionDefinitionIfNotExists(72, "Seguro de Vida (Poliza de Seguro 233.46)", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);
            AddRetentionDefinitionIfNotExists(73, "Seguro de Vida (Poliza de Seguro 166.83)", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);
            AddRetentionDefinitionIfNotExists(74, "Seguro de Vida (Poliza de Seguro 199.33)", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);
            AddRetentionDefinitionIfNotExists(75, "Seguro de Vida (Poliza de Seguro 58.39)", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);
            AddRetentionDefinitionIfNotExists(76, "Seguro de Vida (Poliza de Seguro 57.61)", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);
            AddRetentionDefinitionIfNotExists(77, "Seguro de Vida (Poliza de Seguro 56.44)", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);
            AddRetentionDefinitionIfNotExists(78, "Seguro de Vida (Poliza de Seguro 344.85)", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);
            AddRetentionDefinitionIfNotExists(79, "Seguro de Vida (Poliza de Seguro Colectiva)", companyId, "C_SVIDA", "Otras Retenciones", true, false, false, false, 8, adjustmentDefinitions[54]);


            AddRetentionDefinitionIfNotExists(85, "Decreto Ley 249", companyId, "C_DL92", "Otras Retenciones", true, false, false, true, 5, adjustmentDefinitions[55]);
            AddRetentionDefinitionIfNotExists(88, "Embargo Judicial", companyId, "C_EJU", "Otras Retenciones", true, false, false, true, 9, adjustmentDefinitions[56]);

            AddRetentionDefinitionIfNotExists(96, "Deuda de Trabajadores (Seguridad Social)", companyId, "C_IMP_CSS", "Otras Retenciones", false, true, false, false, 1, adjustmentDefinitions[57]);
            AddRetentionDefinitionIfNotExists(97, "Deuda de Trabajadores (Impuesto por Ingresos Personales)", companyId, "C_IMP_ING", "Otras Retenciones", false, true, false, false, 1, adjustmentDefinitions[58]);

            AddRetentionDefinitionIfNotExists(101, "Materiales y Mano de Obra", companyId, "C_CPOBLA", "Otras Retenciones", false, false, true, true, 26, adjustmentDefinitions[50]);
            AddRetentionDefinitionIfNotExists(106, "Créditos Sociales", companyId, "C_CRESO", "Créditos Bancarios", false, false, true, true, 2, adjustmentDefinitions[59]);
            AddRetentionDefinitionIfNotExists(111, "Insuficiencia Económica", companyId, "C_DL91", "Créditos Bancarios", false, false, true, true, 30, adjustmentDefinitions[50]);
            AddRetentionDefinitionIfNotExists(112, "Viviendas Medios Básicos", companyId, "C_VVINC", "Créditos Bancarios", false, false, true, true, 31, adjustmentDefinitions[51]);
            AddRetentionDefinitionIfNotExists(113, "Pensión Alimenticia", companyId, "C_PALIM", "Demandas", true, false, false, false, 0, adjustmentDefinitions[61]);
            AddRetentionDefinitionIfNotExists(114, "Formación de Fondos", companyId, "C_FFOND", "Otras Retenciones", false, false, true, false, 50, adjustmentDefinitions[62]);
            AddRetentionDefinitionIfNotExists(125, "Deuda de Trabajadores (Transporte)", companyId, "C_TRA", "Otras Retenciones", true, false, false, true, 13, adjustmentDefinitions[63]);
            AddRetentionDefinitionIfNotExists(126, "Responsabilidad Civil", companyId, "C_RCIVIL", "Otras Retenciones", true, false, false, true, 9, adjustmentDefinitions[64]);

            _logger.Info("Retention definitions created.");
        }

        private void CreateSubsidyPaymentDefinitions(int companyId)
        {
            AddSubsidyPaymentDefinitionIfNotExists("Enfermedad Común", "525", companyId, 60, 13, "C_INVTEMP");
            AddSubsidyPaymentDefinitionIfNotExists("Enfermedad Profesional", "523", companyId, 80,13,"C_INVTEMP");
            AddSubsidyPaymentDefinitionIfNotExists("Accidente Común", "525", companyId, 60, 13, "C_INVTEMP");
            AddSubsidyPaymentDefinitionIfNotExists("Accidente de Trabajo", "526", companyId, 80, 13, "C_INVTEMP");
            AddSubsidyPaymentDefinitionIfNotExists("Accidente Equiparado", "524", companyId, 80, 13, "C_INVTEMP");
            AddSubsidyPaymentDefinitionIfNotExists("Prestación monetaria madres con hijos enfermos", "527", companyId, 60, 14, "C_M_H_ENF");
            AddSubsidyPaymentDefinitionIfNotExists("Gestante Enferma", "529", companyId, 100, 14, "C_M_G_ENF");
            AddSubsidyPaymentDefinitionIfNotExists("Maternidad Prenatal", "507", companyId, 100, 14, "C_MATERN");
            AddSubsidyPaymentDefinitionIfNotExists("Maternidad 1ra Postnatal", "507", companyId, 100, 14, "C_MATERN");
            AddSubsidyPaymentDefinitionIfNotExists("Maternidad 2da Postnatal", "507", companyId, 100, 14, "C_MATERN");
            AddSubsidyPaymentDefinitionIfNotExists("PRESTACIÓN SOCIAL (POR MATERNIDAD AL 60%) DECRETO NO.339", "511", companyId, 60, 14, "C_MAT60%");
            AddSubsidyPaymentDefinitionIfNotExists("Reubicado", "521", companyId, 100, 15, "C_INVPAR");
            AddSubsidyPaymentDefinitionIfNotExists("No Reubicado - Adeudo", "521", companyId, 100, 15, "C_INVPAR");
            AddSubsidyPaymentDefinitionIfNotExists("No Reubicado - Gastos de la Empresa", "521", companyId, 100, 15, "C_INVTEMP");
            AddSubsidyPaymentDefinitionIfNotExists("Curso de Calificación o Rehabilitacion", "521", companyId, 100, 15, "C_INVPAR");
            AddSubsidyPaymentDefinitionIfNotExists("Jornada Reducida", "521", companyId, 100, 15, "C_INVPAR");

            _logger.Info("Subsidy payment systems created.");
        }

        private void CreatePaymentDefinitions(int companyId)
        {
            AddPaymentDefinitionIfNotExists(companyId, "88", "RECESO LABORAL (DÍA DE RECESO ADICIONAL RETRIBUIDO)", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "100", "INTERRUPCIONES LAB PERMANECEN EN EL PUESTO (SALARIO BÁSICO CARGO)", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "101", "INTERRUPCIONES LAB (100%) HASTA 30 DÍAS (A) ROTURA EQUIPOS TECNOLÓGICOS", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "102", "INTERRUPCIONES LAB(100%) HASTA 30 DÍAS (B) FALTA DE MATERIA PRIMA, PIEZAS DE REPUESTO", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "103", "INTERRUPCIONES LAB(100%) HASTA 30 DÍAS (C) ACCIÓN DE LLUVIA, CICLÓN, INCENDIOS, CONTAMINACIÓN", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "104", "INTERRUPCIONES LAB(100%) HASTA 30 DÍAS (D) FALTA DE ENERGÍA, COMBUSTIBLE (TRANSPORTE)", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "105", "INTERRUPCIONES LAB(100%) HASTA 30 DÍAS (E) PARALIZACIÓN TEMP EQUIPOS, LINEAS PRODUCCIÓN", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "106", "INTERRUPCIONES LAB(100%) HASTA 30 DÍAS (F) PARALIZACIÓN X REPARACIÓN CAPITAL O MTTO GENERAL", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "107", "INTERRUPCIONES LAB(100%) HASTA 30 DÍAS (G) OTRAS CAUSAS NO IMPUTABLES QUE DETERMINE LA PARALIZACIÓN EVENTUAL DE LAS LABORES", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "108", "INTERRUPCIONES LAB(100%) (Salario Escala)", MathType.Percent, 100.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "111", "INTERRUPCIONES LAB(60%) + 30 DÍAS (A)", MathType.Percent, 60.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "112", "INTERRUPCIONES LAB(60%) + 30 DÍAS (B)", MathType.Percent, 0.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "113", "INTERRUPCIONES LAB(60%) + 30 DÍAS (C)", MathType.Percent, 0.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "114", "INTERRUPCIONES LAB(60%) + 30 DÍAS (D)", MathType.Percent, 0.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "115", "INTERRUPCIONES LAB(60%) + 30 DÍAS (E)", MathType.Percent, 0.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "116", "INTERRUPCIONES LAB(60%) + 30 DÍAS (F)", MathType.Percent, 0.000000M, "C_INT_MN", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "117", "INTERRUPCIONES LAB(60%) + 30 DÍAS (G) - COVID-19", MathType.Percent, 60.000000M, "C_COVID_60", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "131", "NOCTURNIDAD DE 7 PM A 11 PM (0,60 CUP)", MathType.RatePerHour, 0.600000M, "C_NOC", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "132", "NOCTURNIDAD DE 11 PM A 7 AM (1,15 CUP)", MathType.RatePerHour, 1.150000M, "C_NOC", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "501", "COBRA FUERA EMPRESA (CON DERECHO A ESTIMULACIÓN)", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "502", "TRÁMITE DE BAJA", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "503", "COBRA FUERA EMPRESA (SIN DERECHO A ESTIMULACIÓN)", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "504", "TIEMPO TRABAJADO NORMAL", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "505", "VACACIONES", MathType.Percent, 0.090900M, "C_PVAC", WageAdjuster.Holiday);
            AddPaymentDefinitionIfNotExists(companyId, "506", "LICENCIA NO RETRIBUIDA", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy);
            AddPaymentDefinitionIfNotExists(companyId, "507", "LICENCIA RETRIBUIDA POR MATERNIDAD (PRE Y POSTNATAL) DECRETO NO.339", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "508", "DÍA CONMEMORACIÓN NACIONAL Y FERIADO NO LABORADO", MathType.Percent, 100.000000M, "C_DFF", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "509", "HORAS EXTRAS", MathType.Percent, 125.000000M, "C_HE", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "510", "LICENCIA POR FALLECIMIENTO DEL PADRE, LA MADRE, EL CÓNYUGE, HERMANOS E HIJOS", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "511", "PRESTACIÓN SOCIAL (POR MATERNIDAD AL 60%)  DECRETO NO.339", MathType.Average, 0.000000M, "C_MAT60%", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "512", "ACCIDENTE DE TRAYECTO", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "513", "TURNO LOCO", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "514", "AUSENCIA JUSTIFICADA (CON PERMISO)", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "515", "AUSENCIA INJUSTIFICADA (SIN PERMISO)", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "516", "LICENCIA NO RETRIBUIDA (LEY NO.116/2013)", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "517", "PRESO (ESTAR DETENIDO O SOMETIDO A PRISIÓN PROVISIONAL)", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "519", "SUSPENSIÓN DEL VINCULO CON LA ENTIDAD SIN RETRIBUCIÓN (MEDIDA DISCIPLINARIA)", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "520", "ENFERMEDAD O ACCIDENTE HASTA 3 DÍAS", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "521", "INVALIDEZ PARCIAL (PENDIENTE A REUBICAR)", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "522", "REPOSO POR PERITAJE MEDICO (6 MESES) TABLA III", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "523", "ENFERMEDAD PROFESIONAL MÁS DE 3 DÍAS", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "524", "ACCIDENTE EQUIPARADO MÁS DE 3 DÍAS", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "525", "ENFERMEDAD O ACCIDENTE MÁS DE 3 DÍAS", MathType.Average, 0.000000M, "C_INVTEMP", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "526", "ACCIDENTE DE TRABAJO  MÁS DE 3 DÍAS", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "527", "CERTIFICADO MÉDICO DEL MENOR (PRESTACIÓN MONETARIA) 60%", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "529", "CERTIFICADO MÉDICO DE LA TRABAJADORA GESTANTE AL 100%", MathType.Average, 0.000000M, "EMPTY", WageAdjuster.Subsidy, 12);
            AddPaymentDefinitionIfNotExists(companyId, "530", "CITACIÓN JUDICIAL (ACUSADO)", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary, 6);
            AddPaymentDefinitionIfNotExists(companyId, "531", "LICENCIA COMO JUEZ LEGO DE LOS TRIBUNALES POPULARES", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary, 6);
            AddPaymentDefinitionIfNotExists(companyId, "532", "LICENCIA DESEMPEÑAR FUNCIONES COMO DIPUTADO ANPP - DELEGADO AMPP", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary, 6);
            AddPaymentDefinitionIfNotExists(companyId, "533", "DOBLE TURNO TRABAJADO", MathType.Percent, 125.000000M, "C_HE", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "534", "LICENCIA DELEGADO O INVITADO A CONGRESO Y CONFERENCIAS ORGANIZACIONES POLÍTICAS", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary, 6);
            AddPaymentDefinitionIfNotExists(companyId, "537", "LICENCIAS CULTURALES", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "538", "LICENCIAS DEPORTIVAS", MathType.Percent, 100.000000M, "C_MTT", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "539", "CITACIÓN JUDICIAL (TESTIGOS O DEMANDANTES)", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "540", "AUDITORES EN COMPROBACIÓN NACIONAL", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "545", "CURSO DE SUPERACIÓN ESCUELA PROVINCIAL PCC, CTC Y DEFENSA", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "546", "MOVILIZACIÓN (TAREAS RELACIONADAS CON LA DEFENSA)", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "547", "S.M.G", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "548", "OTRAS MOVILIZACIONES(GOBIERNO Y COMISIÓN ELECTORAL)", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "549", "CURSO SUPERACIÓN (CAPACITACIÓN A TRABAJADORES DE LA EMPRESA)", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "550", "CURSO A NUEVOS TRABAJADORES INCORPORADOS (SALARIO MÍMINO)", MathType.MinimumWage, 1.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "551", "MOVILIZACIÓN MILITAR +10 DÍAS", MathType.Percent, 100.000000M, "C_MTT", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "552", "MOVILIZACIÓN MILITAR HASTA 10 DÍAS", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "553", "DESCANSO PAGADO MOVILIZACIÓN MILITAR +10 DÍAS", MathType.Percent, 100.000000M, "C_MTT", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "554", "DESCANSO PAGADO MOVILIZACIÓN MILITAR HASTA 10 DÍAS", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "555", "DESCANSO HABILITADO COMO LABORABLE", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "556", "MOVILIZACIÓN ANTE DESASTRES NATURALES, TECNOLÓGICOS Ó SANITARIOS (COVID -19)", MathType.Average, 100.000000M, "C_MOV_CI", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "557", "AFECTACIÓN DESASTRE NATURAL, TECNOLÓGICO, SANITARIO", MathType.Average, 100.000000M, "C_MOV_CI", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "564", "EXÁMEN MÉDICO PLANIFICADO (PROFILACTORIO)", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "565", "LICENCIA RETRIBUIDA X TUBERCULOSIS.", MathType.Percent, 100.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "566", "LICENCIA RETRIBUIDA X VIH", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "568", "CONSULTA GESTACIÓN 100 % (LICENCIA RETRIBUIDA) DECRETO NO. 339", MathType.Percent, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "569", "DONACIÓN VOLUNTARIA DE SANGRE", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "581", "DÍA CONMEMORACIÓN NACIONAL Y FERIADO TRABAJADO", MathType.Percent, 200.000000M, "C_DFF", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "582", "100% MADRES CON GARANTÍA SALARIAL (1ER MES)- COVID-19", MathType.Percent, 100.000000M, "C_COVID_M", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "583", "60% MADRES CON GARANTÍA SALARIAL (A PARTIR 2DO MES)- COVID-19", MathType.Percent, 60.000000M, "C_COVID_60", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "584", "AISLAMIENTO PREVENTIVO EN CASA CON GARANTÍA SALARIAL (100%) - COVID-19", MathType.Percent, 100.000000M, "C_COVID_100", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "586", "APLICACIÓN DE MEDIDA CAUTELAR ADMINISTRATIVA", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "589", "AISLAMIENTO PREVENTIVO EN CENTROS DE SALUD U OTROS (50%) - COVID-19", MathType.Percent, 50.000000M, "C_COVID_50", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "590", "VACUNACIÓN CONTRA COVID-19", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "592", "VACUNACIÓN CONTRA COVID-19 DEL MENOR DE EDAD", MathType.Average, 100.000000M, "C_TP504", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "593", "AISLAMIENTO PREVENTIVO EN CASA (60% SALARIO PROMEDIO) - COVID 19", MathType.Average, 60.000000M, "C_COVID_60", WageAdjuster.Salary);

            AddPaymentDefinitionIfNotExists(companyId, "604", "SALARIO - ESTUDIANTES", MathType.Percent, 100.000000M, "C_PREGADO", WageAdjuster.Salary);
            AddPaymentDefinitionIfNotExists(companyId, "614", "AUSENCIA JUSTIFICADA - ESTUDIANTES", MathType.Percent, 0.000000M, "EMPTY", WageAdjuster.Salary);

            _logger.Info("Payment systems created.");
        }

        private void CreateOccupations(int companyId)
        {
            AddOccupationIfNotExists(companyId, "202001", "Recepcionista", 'S', "Del Cargo", "II");
            AddOccupationIfNotExists(companyId, "202002", "Sereno", 'S', "Del Cargo", "II");
            AddOccupationIfNotExists(companyId, "502001", "Ayudante", 'O', "Del Cargo", "II");
            AddOccupationIfNotExists(companyId, "103001", "Auxiliar de Oficina", 'A', "Del Cargo", "III");
            AddOccupationIfNotExists(companyId, "203001", "Operador de Audio", 'S', "Del Cargo", "III");
            AddOccupationIfNotExists(companyId, "503001", "Engrasador", 'O', "Del Cargo", "III");
            AddOccupationIfNotExists(companyId, "503004", "Fregador de Piezas y Equipos", 'O', "Del Cargo", "III");
            AddOccupationIfNotExists(companyId, "503005", "Jardinero", 'O', "Del Cargo", "III");
            AddOccupationIfNotExists(companyId, "603001", "Ensamblador A", 'O', "Del Cargo", "III");
            AddOccupationIfNotExists(companyId, "603002", "Operador de Compresores", 'O', "Del Cargo", "III");
            AddOccupationIfNotExists(companyId, "603003", "Pañolero", 'O', "Del Cargo", "III");
            AddOccupationIfNotExists(companyId, "104002", "Archivero Especializado", 'A', "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "604001", "Chofer D", 'O', "Del Cargo", "IV", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "604002", "Engrasador Automotor", 'O', "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "604003", "Limpiador de Alambique y Reactor", 'O', "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "604004",
                "Operador B de Equipos de Transporte en Plantas de Prefabricado y Ejecución de Obras", 'O', "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "604006", "Operador de Grúa Viajera", 'O', "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "604007", "Operador de Máquinas Compactadora", 'O', "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "604008", "Operario de Grúa Viajera", 'O', "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "604009", "Operario de Máquinas de cortar y conformar metales", 'O',
                "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "604010", "Soldador C", 'O', "Del Cargo", "IV");
            AddOccupationIfNotExists(companyId, "105005", "Auxiliar Económico", 'A', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "105006", "Auxiliar Técnico", 'A', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "105007", "Encargado de Distribución", 'A', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "205001", "Dependiente de Almacén", 'S', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "205002", "Encargado de almacén", 'S', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605001", "Ayudante", 'O', "Jefe de Brigada", "V");
            AddOccupationIfNotExists(companyId, "605002", "Carpintero Encofrador B", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605003", "Chofer C", 'O', "Del Cargo", "V", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "605004", "Chofer Ómnibus C", 'O', "Del Cargo", "V", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "605005", "Electricista C de Mantenimiento", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605007", "Mecánico en Climatización y Refrigeración", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605008", "Muestrero B", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605009", "Operador B de Cargador Frontal", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605010", "Operador C Planta de Beneficio de Mineral", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605011", "Operador D de Plantas Metalúrgicas", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605012", "Operador de Montacargas", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605013", "Operador de Tractor sobre Neumático con Aditamentos", 'O',
                "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605014", "Pintor de la Construcción", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605015", "Plomero Instalador B", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "605016", "Soldador B", 'O', "Del Cargo", "V");
            AddOccupationIfNotExists(companyId, "206001", "Encargado de almacén", 'S', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606001", "Ajustador Reparador B", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606002", "Albañil B", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606003", "Albañil Reverberista B", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606004", "Ayudante", 'O', "Jefe de Brigada", "VI");
            AddOccupationIfNotExists(companyId, "606005", "Carpintero A", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606006", "Chapista A de Equipos Automotor", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606007", "Chapista de Equipos Automotores", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606008", "Chofer B", 'O', "Del Cargo", "VI", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "606009", "Chofer Operador B de Hormigonera", 'O', "Del Cargo", "VI", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "606011", "Electricista B de Mantenimiento", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606013", "Engrasador", 'O', "Jefe de Brigada", "VI");
            AddOccupationIfNotExists(companyId, "606014", "Ensamblador A", 'O', "Jefe de Brigada", "VI");
            AddOccupationIfNotExists(companyId, "606015", "Mecánico B de Sistemas y Equipos de Automatización", 'O',
                "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606016", "Mecánico C Automotor", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606017", "Mecánico C de Mantenimiento Industrial", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606018", "Mecánico B de Sistemas y Equipos de Automatización ", 'O',
                "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606019", "Mecánico de Taller B", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606020", "Muestrero A", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606021", "Operador C de Fuerzas para Plantas Metalúrgicas", 'O', "Del Cargo",
                "VI");
            AddOccupationIfNotExists(companyId, "606022", "Operador C de Plantas Metalúrgicas", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606024", "Operador de Equipos y Dispositivos para la Reparación de Neumáticos",
                'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606025", "Operador de Movimiento y Suministro de Mineral", 'O', "Del Cargo",
                "VI");
            AddOccupationIfNotExists(companyId, "606026", "Operario de Máquinas Herramientas B", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606027", "Pailero B", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606028", "Pailero Industrial C para la Construcción ", 'O', "Del Cargo", "VI");
            AddOccupationIfNotExists(companyId, "606030", "Pintor A de Vehículos y Equipos Automotores", 'O', "Del Cargo",
                "VI");
            AddOccupationIfNotExists(companyId, "607001", "Carpintero Encofrador A", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607002", "Chofer A", 'O', "Del Cargo", "VII", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "607003", "Chofer Operador B de Grúa Camión", 'O', "Del Cargo", "VII", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "607004", "Mecánico B Automotor", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607005", "Mecánico B de Mantenimiento Industrial", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607007", "Operador A de Grúa de Izaje y Movimiento de Tierra", 'O',
                "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607009", "Operador A de Grupo Electrógeno", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607010", "Operador A Planta de Beneficio de Mineral", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607011", "Operador B de Topador Frontal", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607013", "Operador de Máquinas Herramientas A", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607014", "Operador de Martillo", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607015", "Operador de Montacargas", 'O', "Jefe de Brigada", "VII");
            AddOccupationIfNotExists(companyId, "607016", "Operario B de Estación de Bombeo Eléctrico", 'O', "Del Cargo",
                "VII");
            AddOccupationIfNotExists(companyId, "607017", "Operario de Máquinas Herramientas A", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607018", "Pailero Industrial B para la Construcción", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607019", "Plomero Instalador A", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "607020", "Soldador A", 'O', "Del Cargo", "VII");
            AddOccupationIfNotExists(companyId, "608001", "Ajustador Reparador A", 'O', "Del Cargo", "VIII");
            AddOccupationIfNotExists(companyId, "608002", "Albañil A", 'O', "Del Cargo", "VIII");
            AddOccupationIfNotExists(companyId, "608004", "Chofer Operador A Grúa Camión", 'O', "Del Cargo", "VIII", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "608005", "Chofer Tiro de Mineral", 'O', "Del Cargo", "VIII", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "608006", "Electricista A Automotor", 'O', "Del Cargo", "VIII");
            AddOccupationIfNotExists(companyId, "608007", "Mecánico A Automotor", 'O', "Del Cargo", "VIII");
            AddOccupationIfNotExists(companyId, "608008", "Mecánico de Taller A ", 'O', "Del Cargo", "VIII");
            AddOccupationIfNotExists(companyId, "608010", "Muestrero B", 'O', "Jefe de Brigada", "VIII");
            AddOccupationIfNotExists(companyId, "608011", "Operador A de Motoniveladora", 'O', "Del Cargo", "VIII");
            AddOccupationIfNotExists(companyId, "608012", "Operador C de Plantas Metalúrgicas", 'O', "Jefe de Brigada", "VIII");
            AddOccupationIfNotExists(companyId, "608013", "Operario A de Estación de Bombeo Eléctrico", 'O', "Del Cargo",
                "VIII");
            AddOccupationIfNotExists(companyId, "608014", "Pailero A", 'O', "Del Cargo", "VIII");
            AddOccupationIfNotExists(companyId, "609001", "Mecánico C de Mantenimiento Industrial", 'O', "Jefe de Brigada",
                "IX");
            AddOccupationIfNotExists(companyId, "609002", "Muestrero A", 'O', "Jefe de Brigada", "IX");
            AddOccupationIfNotExists(companyId, "410001", "Agente de Ventas", 'T', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "410002", "Secretaria Ejecutiva", 'T', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "410003", "Técnico A en Tratamiento Médico", 'T', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "410004", "Técnico en Abastecimiento Técnico Material", 'T', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "410005", "Técnico en Climatización y Refrigeración", 'T', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "410006", "Técnico en Información y Bibliotecología", 'T', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "610001", "Albañil Reverberista A", 'O', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "610002", "Chofer A", 'O', "Jefe de Brigada", "X", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "610003", "Instalador A de Tubería Gruesa", 'O', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "610004", "Mecánico B de Mantenimiento Industrial", 'O', "Jefe de Brigada",
                "X");
            AddOccupationIfNotExists(companyId, "610005", "Mecánico de Taller A", 'O', "Jefe de Brigada", "X");
            AddOccupationIfNotExists(companyId, "610006", "Operador B de Centrales Eléctricas", 'O', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "610007", "Operador B de Plantas Metalúrgicas", 'O', "Del Cargo", "X");
            AddOccupationIfNotExists(companyId, "610008", "Operador A de Planta de Beneficio de Mineral", 'O',
                "Jefe de Brigada", "X");
            AddOccupationIfNotExists(companyId, "411001", "Diseñador Gráfico A", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411002", "Técnico A del Transporte Automotor", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411003", "Técnico A en Gestión de Recursos Humanos", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411004", "Técnico A en Gestión Económica", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411006", "Técnico de Comunicaciones y Telecontrol", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411007", "Técnico de Proyecto e Ingeniería", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411008", "Técnico en Ensayos Físicos Químicos y Mecánicos", 'T', "Del Cargo",
                "XI");
            AddOccupationIfNotExists(companyId, "411009", "Técnico en Geología", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411010", "Técnico en Mantenimiento Industrial", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411011", "Técnico en Metrología", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411012", "Técnico en Producción", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411013", "Técnico en Protección por Relés, Automática y Circuitos Secundarios",
                'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411014", "Técnico en Recursos Materiales", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411015", "Técnico en Seguridad y Salud en el Trabajo", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411016", "Técnico en Sistemas de Transmisión", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411017", "Técnico en Topografía", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411018", "Tecnólogo de Corrosión y Contaminación Ambiental", 'T', "Del Cargo",
                "XI");
            AddOccupationIfNotExists(companyId, "411019", "Tecnólogo de Procesos Industriales", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "411020", "Tecnólogo en Instalaciones Industriales", 'T', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611001", "Albañil A", 'O', "Jefe de Brigada", "XI");
            AddOccupationIfNotExists(companyId, "611002", "Chofer Operador A de Grúa Camión", 'O', "Jefe de Brigada", "XI", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "611003", "Chofer Tiro de Mineral", 'O', "Jefe de Brigada", "XI", groupDescription: null, new NameValue("Curso de recalificación para choferes", "true"));
            AddOccupationIfNotExists(companyId, "611004", "Electricista A de Mantenimiento", 'O', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611005", "Electricista A Enrollador ", 'O', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611006", "Electricista Enrollador A", 'O', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611007", "Jefe de Turno", 'O', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611010", "Mecánico A Automotor", 'O', "Jefe de Brigada", "XI");
            AddOccupationIfNotExists(companyId, "611011", "Mecánico A de Mantenimiento Industrial", 'O', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611012", "Mecánico A de Sistemas y Equipos de Automatización", 'O',
                "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611013", "Mecánico de Taller A", 'O', "Jefe de Brigada", "XI");
            AddOccupationIfNotExists(companyId, "611015", "Operador A de Centrales Eléctricas", 'O', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611016", "Operador A de Cuadro de Centrales Eléctricas", 'O', "Del Cargo",
                "XI");
            AddOccupationIfNotExists(companyId, "611017", "Operador A de Plantas Metalúrgicas", 'O', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "611018", "Operador B de Topador Frontal", 'O', "Jefe de Brigada", "XI");
            AddOccupationIfNotExists(companyId, "611019", "Pailero A", 'O', "Jefe de Brigada", "XI");
            AddOccupationIfNotExists(companyId, "611020", "Pailero Industrial A para la Construcción ", 'O', "Del Cargo", "XI");
            AddOccupationIfNotExists(companyId, "612001", "Albañil Reverberista A", 'O', "Jefe de Brigada", "XII");
            AddOccupationIfNotExists(companyId, "612002", "Electricista A de Mantenimiento", 'O', "Jefe de Brigada", "XII");
            AddOccupationIfNotExists(companyId, "612003", "Instalador A de Tubería Gruesa", 'O', "Jefe de Brigada", "XII");
            AddOccupationIfNotExists(companyId, "612004", "Jefe de Turno", 'O', "Del Cargo", "XII");
            AddOccupationIfNotExists(companyId, "612005", "Mecánico A de Sistemas y Equipos de Automatización", 'O',
                "Jefe de Brigada", "XII");
            AddOccupationIfNotExists(companyId, "612007", "Operador B de Plantas Metalúrgicas", 'O', "Jefe de Brigada", "XII");
            AddOccupationIfNotExists(companyId, "613001", "Albañil Reverberista A", 'O', "Jefe de Brigada", "XIII");
            AddOccupationIfNotExists(companyId, "613002", "Electricista A de Mantenimiento", 'O', "Jefe de Brigada", "XIII");
            AddOccupationIfNotExists(companyId, "613004", "Electricista Enrollador A", 'O', "Jefe de Brigada", "XIII");
            AddOccupationIfNotExists(companyId, "613005", "Mecánico A de Mantenimiento Industrial", 'O', "Jefe de Brigada",
                "XIII");
            AddOccupationIfNotExists(companyId, "613006", "Mecánico A de Sistemas y Equipos de Automatización", 'O',
                "Jefe de Brigada", "XIII");
            AddOccupationIfNotExists(companyId, "613007", "Operador A de Centrales Eléctricas", 'O', "Jefe de Brigada", "XIII");
            AddOccupationIfNotExists(companyId, "613008", "Operador A de Cuadro de Centrales Eléctricas", 'O',
                "Jefe de Brigada", "XIII");
            AddOccupationIfNotExists(companyId, "613009", "Operador A de Plantas Metalúrgicas", 'O', "Jefe de Brigada", "XIII");
            AddOccupationIfNotExists(companyId, "613010", "Operador B de Plantas Metalúrgicas", 'O', "Jefe de Brigada", "XIII");
            AddOccupationIfNotExists(companyId, "613011", "Pailero Industrial A para la Construcción", 'O', "Jefe de Brigada",
                "XIII");
            AddOccupationIfNotExists(companyId, "414001", "Especialista C en Medio de Diagnóstico e Investigación Médica", 'T',
                "Del Cargo", "XIV");
            AddOccupationIfNotExists(companyId, "414002", "Especialista en Protección Física y Secreto Estatal", 'T',
                "Del Cargo", "XIV");
            AddOccupationIfNotExists(companyId, "414003", "Técnico en Climatización y Refrigeración",
                'T', "Especialista Principal", "XIV");
            AddOccupationIfNotExists(companyId, "614001", "Carpintero Encofrador A", 'O', "Jefe de Brigada", "XIV");
            AddOccupationIfNotExists(companyId, "614002", "Electricista A de Mantenimiento", 'O', "Jefe de Brigada", "XIV");
            AddOccupationIfNotExists(companyId, "614004", "Jefe de Turno", 'O', "Del Cargo", "XIV");
            AddOccupationIfNotExists(companyId, "614005", "Mecánico A Automotor", 'O', "Jefe de Brigada", "XIV");
            AddOccupationIfNotExists(companyId, "614007", "Mecánico A de Mantenimiento Industrial", 'O', "Jefe de Brigada",
                "XIV");
            AddOccupationIfNotExists(companyId, "614008", "Mecánico A de Sistemas y Equipos de Automatización", 'O',
                "Jefe de Brigada", "XIV");
            AddOccupationIfNotExists(companyId, "614009", "Mecánico de Taller A", 'O', "Jefe de Brigada", "XIV");
            AddOccupationIfNotExists(companyId, "614010", "Operador A de Centrales Eléctricas", 'O', "Jefe de Brigada", "XIV");
            AddOccupationIfNotExists(companyId, "614011", "Operador A de Plantas Metalúrgicas", 'O', "Jefe de Brigada", "XIV");
            AddOccupationIfNotExists(companyId, "614012", "Pailero Industrial A para la Construcción", 'O', "Jefe de Brigada",
                "XIV");
            AddOccupationIfNotExists(companyId, "415001", "Especialista B del Fórum de Ciencia y Técnica", 'T', "Del Cargo",
                "XV");
            AddOccupationIfNotExists(companyId, "415002", "Especialista B del Transporte Automotor", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415003", "Especialista B en Abastecimiento Técnico Material", 'T', "Del Cargo",
                "XV");
            AddOccupationIfNotExists(companyId, "415004", "Especialista B en Automatización", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415005", "Especialista B en Ensayos Físicos Químicos y Mecánicos", 'T',
                "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415006", "Especialista B en Geología", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415007", "Especialista B en Mantenimiento Industrial", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415008", "Especialista B en Minas", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415009", "Especialista B en Recursos Materiales", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415010", "Especialista B en Sistemas de Transmisión", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415011", "Especialista B en Topografía", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415012", "Especialista B para la Defensa y Defensa Civil", 'T', "Del Cargo",
                "XV");
            AddOccupationIfNotExists(companyId, "415013", "Especialista C en Ahorro y Uso Racional de la Energía", 'T',
                "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415014", "Especialista C en Gestión de la Calidad", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415015", "Especialista C en Gestión de Recursos Humanos", 'T', "Del Cargo",
                "XV");
            AddOccupationIfNotExists(companyId, "415016", "Especialista C en Gestión Documental", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415017", "Especialista C en Gestión Económica", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415018", "Especialista C en Seguridad y Salud en el Trabajo", 'T', "Del Cargo",
                "XV");
            AddOccupationIfNotExists(companyId, "415019", "Especialista en Soldadura", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "415020", "Técnico B en Ensayos Físicos Químicos y Mecánicos", 'T', "Del Cargo",
                "XV");
            AddOccupationIfNotExists(companyId, "415021", "Técnico en Seguridad y Salud en el Trabajo", 'T', "Jefe de Brigada",
                "XV");
            AddOccupationIfNotExists(companyId, "415022", "Tecnólogo B de Procesos Industriales", 'T', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "615001", "Jefe de Turno", 'O', "Del Cargo", "XV");
            AddOccupationIfNotExists(companyId, "416001", "Asesor B Jurídico", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416002", "Especialista A  en Minas", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416003", "Especialista A de Comunicaciones y Telecontrol", 'T', "Del Cargo",
                "XVI");
            AddOccupationIfNotExists(companyId, "416004", "Especialista A de Proyectos e Ingeniería", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416005", "Especialista A en Automatización", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416006", "Especialista A en Divulgación y Propaganda", 'T', "Del Cargo",
                "XVI");
            AddOccupationIfNotExists(companyId, "416007", "Especialista A en Ensayos Físicos Químicos y Mecánicos", 'T',
                "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416008", "Especialista A en Geología", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416009", "Especialista A en Mantenimiento Industrial", 'T', "Del Cargo",
                "XVI");
            AddOccupationIfNotExists(companyId, "416010",
                "Especialista A en Protección por Relés, Automática y Circuitos Secundarios", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416011", "Especialista A en Recursos Materiales", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416013", "Especialista B de Seguridad y Protección", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416014", "Especialista B en Ahorro y Uso Racional de Energía", 'T',
                "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416016", "Especialista B en Ciencias Informáticas", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416017", "Especialista B en Gestión de la Calidad", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416018", "Especialista B en Gestión de Recursos Humanos", 'T', "Del Cargo",
                "XVI");
            AddOccupationIfNotExists(companyId, "416019", "Especialista B en Gestión Económica", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416020", "Especialista B en Inversiones", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416021", "Especialista B en Metrología", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416022", "Especialista B en Seguridad y Salud en el Trabajo", 'T', "Del Cargo",
                "XVI");
            AddOccupationIfNotExists(companyId, "416023", "Especialista C en Gestión Documental", 'T', "Especialista Principal",
                "XVI");
            AddOccupationIfNotExists(companyId, "416024", "Especialista Electricista General", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416025", "Especialista en Cuadros ", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416026", "Especialista Superior Mecánico", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416027", "Jefe de Turno", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416028", "Técnico A de Procesos Industriales", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "416029", "Tecnólogo A de Procesos Industriales", 'T', "Del Cargo", "XVI");
            AddOccupationIfNotExists(companyId, "417001", "Auditor Principal", 'T', "Del Cargo", "XVII");
            AddOccupationIfNotExists(companyId, "417002", "Especialista A en Automatización", 'T', "Especialista Principal",
                "XVII");
            AddOccupationIfNotExists(companyId, "417003", "Especialista A en Ensayos Físicos Químicos y Mecánicos", 'T',
                "Especialista Principal", "XVII");
            AddOccupationIfNotExists(companyId, "417004", "Especialista B del Transporte Automotor", 'T',
                "Especialista Principal", "XVII");
            AddOccupationIfNotExists(companyId, "417005", "Especialista B en Gestión de Recursos Humanos", 'T',
                "Especialista Principal", "XVII");
            AddOccupationIfNotExists(companyId, "417006", "Especialista B en Gestión Económica", 'T', "Especialista Principal",
                "XVII");
            AddOccupationIfNotExists(companyId, "417007", "Especialista Superior Mecánico", 'T', "Especialista Principal",
                "XVII");
            AddOccupationIfNotExists(companyId, "417008", "Tecnólogo A de Procesos Industriales", 'T', "Especialista Principal",
                "XVII");
            AddOccupationIfNotExists(companyId, "417009", "Tecnólogo de Corrosión y Contaminación Ambiental", 'T',
                "Especialista Principal", "XVII");
            AddOccupationIfNotExists(companyId, "418001", "Especialista B en  Inversiones", 'T', "Especialista Principal",
                "XVIII");
            AddOccupationIfNotExists(companyId, "418002", "Especialista B en Ensayos Físicos Químicos y Mecánicos", 'T',
                "Especialista Principal", "XVIII");
            AddOccupationIfNotExists(companyId, "418003", "Especialista B en Gestión de la Calidad", 'T',
                "Especialista Principal", "XVIII");
            AddOccupationIfNotExists(companyId, "418004", "Especialista B en Gestión de Recursos Humanos", 'T',
                "Especialista Principal", "XVIII");
            AddOccupationIfNotExists(companyId, "418005", "Especialista B en Gestión Económica", 'T', "Especialista Principal",
                "XVIII");
            AddOccupationIfNotExists(companyId, "418006", "Especialista B en Inversiones", 'T', "Especialista Principal",
                "XVIII");
            AddOccupationIfNotExists(companyId, "418007", "Especialista B en Recursos Materiales", 'T',
                "Especialista Principal", "XVIII");
            AddOccupationIfNotExists(companyId, "418008", "Especialista C en Gestión Económica", 'T', "Especialista Principal",
                "XVIII");
            AddOccupationIfNotExists(companyId, "418009", "Especialista Electricista General", 'T', "Especialista Principal",
                "XVIII");
            AddOccupationIfNotExists(companyId, "418011", "Tecnólogo A de Procesos Industriales", 'T', "Especialista Principal",
                "XVIII");
            AddOccupationIfNotExists(companyId, "418012", "Especialista A en Mantenimiento Industrial", 'T',
                "Especialista Principal", "XVIII");
            AddOccupationIfNotExists(companyId, "618001", "Mecánico A de Mantenimiento Industrial", 'O', "Jefe de Brigada",
                "XVIII");
            AddOccupationIfNotExists(companyId, "419001", "Asesor B Jurídico", 'T', "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419002", "Especialista A en Minas", 'T', "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419003", "Especialista A de Proyectos e Ingeniería", 'T',
                "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419004", "Especialista A en Geología", 'T', "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419005",
                "Especialista A en Protección por Relés, Automática y Circuitos Secundarios", 'T', "Especialista Principal",
                "XIX");
            AddOccupationIfNotExists(companyId, "419006", "Especialista A en Recursos Materiales", 'T',
                "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419007", "Especialista A Mantenimiento Industrial", 'T',
                "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419008", "Especialista B de Seguridad y Protección", 'T',
                "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419009", "Especialista B en Ahorro y Uso Racional de Energía", 'T',
                "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419010", "Especialista B en Ciencias Informáticas", 'T',
                "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419011", "Especialista B en Gestión de Recursos Humanos", 'T',
                "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419012", "Especialista B en Gestión Económica", 'T', "Especialista Principal",
                "XIX");
            AddOccupationIfNotExists(companyId, "419013", "Especialista B en Inversiones", 'T', "Especialista Principal",
                "XIX");
            AddOccupationIfNotExists(companyId, "419014", "Especialista B en Seguridad y Salud en el Trabajo", 'T',
                "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419015", "Especialista B en Inversiones ", 'T', "Especialista Principal",
                "XIX");
            AddOccupationIfNotExists(companyId, "419016", "Especialista en Cuadros ", 'T', "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "419017", "Técnico A de Procesos Industriales", 'T', "Especialista Principal",
                "XIX");
            AddOccupationIfNotExists(companyId, "419018", "Tecnólogo A de Procesos Industriales", 'T', "Especialista Principal",
                "XIX");
            AddOccupationIfNotExists(companyId, "419019", "Auditor Principal", 'T', "Especialista Principal", "XIX");
            AddOccupationIfNotExists(companyId, "619001", "Jefe de Taller Mantenimiento Eléctrico Especializado", 'O',
                "Del Cargo", "XIX");
            AddOccupationIfNotExists(companyId, "719001", "Jefe de Mantenimiento Calcinación y Sínter", 'C', "Del Cargo",
                "XIX");
            AddOccupationIfNotExists(companyId, "719002", "Jefe de Mantenimiento Hornos de Reducción", 'C', "Del Cargo", "XIX");
            AddOccupationIfNotExists(companyId, "719003", "Jefe de Mantenimiento Lixiviación y Lavado", 'C', "Del Cargo",
                "XIX");
            AddOccupationIfNotExists(companyId, "719004", "Jefe de Mantenimiento Mina", 'C', "Del Cargo", "XIX");
            AddOccupationIfNotExists(companyId, "719005", "Jefe de Mantenimiento Preparación de Mineral", 'C', "Del Cargo",
                "XIX");
            AddOccupationIfNotExists(companyId, "719006", "Jefe de Mantenimiento Recepción y Suministro", 'C', "Del Cargo",
                "XIX");
            AddOccupationIfNotExists(companyId, "719007", "Jefe de Mantenimiento Recuperación y Cobalto", 'C', "Del Cargo",
                "XIX");
            AddOccupationIfNotExists(companyId, "719008", "Jefe de Mantenimiento Termoenergética", 'C', "Del Cargo", "XIX");
            AddOccupationIfNotExists(companyId, "720001", "Jefe de Mantenimiento Automotor", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720002", "Jefe de Mantenimiento Electrofiltros", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720003", "Jefe de Mantenimiento Instrumentos", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720004", "Jefe de Mantenimiento Izaje Industrial", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720005", "Jefe de Mantenimiento Mecánico", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720006", "Jefe de Mantenimiento Mecánico 2", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720007", "Jefe de Mantenimiento Misceláneo", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720008", "Jefe de Mantenimiento Reverbería, Mecánica y Pailería", 'C',
                "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720009", "Jefe de Operación Extracción y Transporte", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720010", "Jefe de Operación Transporte", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "720011", "Jefe de Operaciones Muestreo", 'C', "Del Cargo", "XX");
            AddOccupationIfNotExists(companyId, "721001", "Jefe Técnico de Mantenimiento", 'C', "Del Cargo", "XXI");
            AddOccupationIfNotExists(companyId, "722001", "Director Reparaciones Capitales", 'C', "Del Cargo", "XXII");
            AddOccupationIfNotExists(companyId, "722002", "Director UB Producción Planta  Recuperación de Amoniaco y Cobalto",
                'C', "Del Cargo", "XXII");
            AddOccupationIfNotExists(companyId, "722003", "Director UB Producción Planta Calcinación y Sinter", 'C',
                "Del Cargo", "XXII");
            AddOccupationIfNotExists(companyId, "722004", "Director UB Producción Planta Hornos de Reducción", 'C', "Del Cargo",
                "XXII");
            AddOccupationIfNotExists(companyId, "722005", "Director UB Producción Planta Lixiviación y Lavado", 'C',
                "Del Cargo", "XXII");
            AddOccupationIfNotExists(companyId, "722006", "Director UB Producción Planta Preparación del Mineral", 'C',
                "Del Cargo", "XXII");
            AddOccupationIfNotExists(companyId, "722007", "Director UB Servicios Planta de Recepción y Suministro", 'C',
                "Del Cargo", "XXII");
            AddOccupationIfNotExists(companyId, "722008", "Director UB Servicios Planta Termoenergética", 'C', "Del Cargo",
                "XXII");
            AddOccupationIfNotExists(companyId, "722009", "Jefe de Laboratorio Químico", 'C', "Del Cargo", "XXII");
            AddOccupationIfNotExists(companyId, "722010", "Jefe Mantenimiento Eléctrico ", 'C', "Del Cargo", "XXII");
            AddOccupationIfNotExists(companyId, "722011", "Director de Seguridad y Protección", 'C', "Del Cargo", "XXII");

            AddOccupationIfNotExists(companyId, "723001", "Director de Economía y Finanzas", 'C', "Del Cargo", "XXIII");
            AddOccupationIfNotExists(companyId, "723002", "Director de Producción", 'C', "Del Cargo", "XXIII");
            AddOccupationIfNotExists(companyId, "723003", "Director de Recursos Humanos", 'C', "Del Cargo", "XXIII");
            AddOccupationIfNotExists(companyId, "723004", "Director de Inversiones", 'C', "Del Cargo", "XXIII");
            AddOccupationIfNotExists(companyId, "723005", "Director de Seguridad, Salud y Medio Ambiente ", 'C', "Del Cargo",
                "XXIII");
            AddOccupationIfNotExists(companyId, "723006", "Director UB Abastecimiento", 'C', "Del Cargo", "XXIII");
            AddOccupationIfNotExists(companyId, "723007", "Director UB Mantenimiento", 'C', "Del Cargo", "XXIII");
            AddOccupationIfNotExists(companyId, "723008", "Director UB Servicios Técnicos a la Producción", 'C', "Del Cargo",
                "XXIII");
            AddOccupationIfNotExists(companyId, "723009", "Director Unidad Básica Minera", 'C', "Del Cargo", "XXIII");
            
            AddOccupationIfNotExists(companyId, "724001", "Director General", 'C', "Del Cargo", "XXV");
            AddOccupationIfNotExists(companyId, "307001", "Técnico Medio en Adiestramiento", 'T', "Del Cargo", "VII",
                "TM en Adiestramiento");
            AddOccupationIfNotExists(companyId, "413000", "Técnico Superior en Adiestramiento", 'T', "Del Cargo", "XII",
                "TS en Adiestramiento");
            AddOccupationIfNotExists(companyId, "413001", "Técnico Nivel Superior en Adiestramiento", 'T', "Del Cargo", "XIII",
                "TNS en Adiestramiento");
            _logger.Info("Occupations created.");
        }

        private void CreateQualificationRequirementDefinition(int companyId)
        {
            _logger.Info("Qualification Requirements created.");
        }

        private void CreateComplexityGroups(int companyId)
        {
            AddComplexityGroupIfNotExists("I", "Salario Mínimo", new Money(2100, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("II", null, new Money(7300, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("II", "Hornos", new Money(7500, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("III", null, new Money(7632, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("III", "Hornos", new Money(7800, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("IV", null, new Money(7992, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("IV", "Hornos", new Money(8190, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("V", null, new Money(8400, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("V", "Hornos", new Money(8600, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("VI", null, new Money(8808, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("VI", "Hornos", new Money(9000, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("VII", "TM en Adiestramiento", new Money(5160, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("VII", null, new Money(9288, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("VII", "Proceso de Formación de Pregado", new Money(2810, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("VII", "Hornos", new Money(9500, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("VIII", null, new Money(9792, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("VIII", "Hornos", new Money(10000, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("IX", null, new Money(10272, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("X", null, new Money(10776, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("X", "Hornos", new Money(11300, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XI", null, new Money(10970, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XI", "Hornos", new Money(11920, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XII", null, new Money(11220, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XII", "TS en Adiestramiento", new Money(6630, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XIII", "TNS en Adiestramiento", new Money(7000, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XIII", null, new Money(11470, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XIII", "Hornos", new Money(12530, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XIV", null, new Money(11720, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XIV", "Hornos", new Money(12780, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XV", null, new Money(11970, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XV", "Hornos (Tecnólogo B de Procesos Industriales)", new Money(12500, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XV", "Hornos", new Money(13000, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XVI", null, new Money(12230, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XVI", "Hornos", new Money(13500, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XVII", null, new Money(12480, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XVIII", null, new Money(12730, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XIX", null, new Money(13040, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XIX", "Hornos", new Money(13940, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XX", null, new Money(13360, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XXI", null, new Money(13670, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XXII", null, new Money(13990, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XXII", "Hornos", new Money(14500, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XXIII", null, new Money(14300, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XXIV", null, new Money(14620, KontecgCoreConsts.DefaultCurrency),
                companyId);
            AddComplexityGroupIfNotExists("XXV", null, new Money(15000, KontecgCoreConsts.DefaultCurrency),
                companyId);

            _logger.Info("Complexity groups created.");
        }

        private void CreateReponsibilities()
        {
            AddResponsibilityIfNotExists("Especialista Principal");
            AddResponsibilityIfNotExists("Jefe de Brigada");
            AddResponsibilityIfNotExists("Jefe de Taller");
            AddResponsibilityIfNotExists("Del Cargo");
            _logger.Info("Reponsibilities created.");
        }

        private void CreateOccupationalCategories()
        {
            AddOccupationalCategoryIfNotExists("Administrativos", 'A');
            AddOccupationalCategoryIfNotExists("Servicios", 'S');
            AddOccupationalCategoryIfNotExists("Técnicos", 'T');
            AddOccupationalCategoryIfNotExists("Obreros", 'O');
            AddOccupationalCategoryIfNotExists("Cuadros", 'C');
            _logger.Info("Occupational categories created.");
        }

        private void CreatePlusDefinitions(int companyId)
        {
            AddPlusDefinitionIfNotExists("PLUS", ScopeData.Company, "Pago Adicional", companyId, "EMPTY");
            AddPlusDefinitionIfNotExists("CLA", ScopeData.Company, "Condiciones Laborales Anormales", companyId, "C_CLA");
            AddPlusDefinitionIfNotExists("CIES", ScopeData.Company, "Coeficiente de Interés Económico Social", companyId, "EMPTY");
            AddPlusDefinitionIfNotExists("HIRR", ScopeData.Company, "Horario Irregular", companyId, "EMPTY");
            AddPlusDefinitionIfNotExists("HOM", ScopeData.Personal, "Homologación", companyId, "C_CERT");
            AddPlusDefinitionIfNotExists("CCIE", ScopeData.Personal, "Categoría Científica", companyId, "C_GRADO");
            AddPlusDefinitionIfNotExists("DS", ScopeData.Personal, "Diferencia Salarial", companyId, "EMPTY");
            _logger.Info("Plus definitions created.");
        }

        #region Add methods

        private void AddAdjustmentDefinitionIfNotExists(int code, string description, int companyId, string reference, int processOn, bool sumHoursForHolidayHistogram = false, bool sumAmountForHolidayHistogram = false, bool sumHoursForPaymentHistogram = false, bool sumAmountForPaymentHistogram = false, bool sumHoursForSocialSecurity = false, bool sumAmountForSocialSecurity = false, TaxAmountToContribute contributeForCompanySocialSecurityTaxes = TaxAmountToContribute.NoContribute, TaxAmountToContribute contributeForIncomeTaxes = TaxAmountToContribute.NoContribute, TaxAmountToContribute contributeForCompanyWorkforceTaxes = TaxAmountToContribute.NoContribute)
        {
            if (_context.AdjustmentDefinitions.IgnoreQueryFilters().Any(s =>
                    s.Code == code && s.Description == description && s.CompanyId == companyId))
                return;

            _context.AdjustmentDefinitions.Add(new AdjustmentDefinition(companyId, code, description, processOn, reference, sumHoursForHolidayHistogram, sumAmountForHolidayHistogram, sumHoursForPaymentHistogram, sumAmountForPaymentHistogram, sumHoursForSocialSecurity, sumAmountForSocialSecurity, contributeForCompanySocialSecurityTaxes, contributeForIncomeTaxes, contributeForCompanyWorkforceTaxes));
            _context.SaveChanges();
        }

        private void AddRetentionDefinitionIfNotExists(int code, string description, int companyId, string reference, string type, bool partial, bool auto, bool credit, bool total, int priority = 0, int? refundId = null)
        {
            if (_context.RetentionDefinitions.IgnoreQueryFilters().Any(s =>
                    s.Code == code && s.Description == description && s.Reference == reference && s.CompanyId == companyId))
                return;

            _context.RetentionDefinitions.Add(new RetentionDefinition(companyId, code, description, reference, type, partial, auto, credit, total, priority, refundId));
            _context.SaveChanges();
        }

        private void AddOccupationalCategoryIfNotExists(string description, char code)
        {
            if (_context.OccupationalCategories.IgnoreQueryFilters().Any(s => s.Code == code && s.DisplayName == description))
                return;

            _context.OccupationalCategories.Add(new OccupationalCategory(code, description));
            _context.SaveChanges();
        }

        private void AddResponsibilityIfNotExists(string description)
        {
            if (_context.Responsibilities.IgnoreQueryFilters().Any(s => s.DisplayName == description))
                return;

            _context.Responsibilities.Add(new Responsibility(description));
            _context.SaveChanges();
        }

        private void AddComplexityGroupIfNotExists(string group, string description, Money baseSalary, int companyId)
        {
            if (_context.ComplexityGroups.IgnoreQueryFilters().Any(s =>
                    s.Group == group && s.Description == description &&
                    s.BaseSalary == baseSalary && s.CompanyId == companyId))
                return;

            _context.ComplexityGroups.Add(new ComplexityGroup(companyId, group, description, baseSalary));
            _context.SaveChanges();
        }

        private void AddPlusDefinitionIfNotExists(string name, ScopeData plusType, string description, int companyId, string reference)
        {
            if (_context.PlusDefinitions.IgnoreQueryFilters().Any(s => s.Name == name && s.Reference == reference && s.CompanyId == companyId))
                return;

            _context.PlusDefinitions.Add(new PlusDefinition(name, plusType, description, companyId, reference));
            _context.SaveChanges();
        }

        private void AddSubsidyPaymentDefinitionIfNotExists(string displayName, string paymentDefinitionName, int companyId, decimal basePercent, int processOn, string reference)
        {
            if (_context.SubsidyPaymentDefinitions.IgnoreQueryFilters().Include(s => s.PaymentDefinition).Any(s =>
                    s.DisplayName == displayName && s.PaymentDefinition.Name == paymentDefinitionName && s.Reference == reference && s.CompanyId == companyId))
                return;

            var paymentDefinition = _context.PaymentDefinitions.IgnoreQueryFilters().FirstOrDefault(s => s.Name == paymentDefinitionName && s.WageAdjuster == WageAdjuster.Subsidy);
            if(paymentDefinition == null) return;

            _context.SubsidyPaymentDefinitions.Add(new SubsidyPaymentDefinition(displayName, paymentDefinition.Id, reference, basePercent, processOn, companyId));
            _context.SaveChanges();
        }

        private void AddPaymentDefinitionIfNotExists(int companyId, string name, string description, MathType mathType, decimal factor, string reference, WageAdjuster wageAdjuster, int averageTime = 0, EmployeeSalaryForm salaryForm = EmployeeSalaryForm.Royal, PaymentSystem paymentSystem = PaymentSystem.ByTime)
        {
            if (_context.PaymentDefinitions.IgnoreQueryFilters().Any(s =>
                    s.Name == name && s.WageAdjuster == wageAdjuster && s.SalaryForm == salaryForm && s.Reference == reference &&
                    s.PaymentSystem == paymentSystem && s.CompanyId == companyId))
                return;

            _context.PaymentDefinitions.Add(new PaymentDefinition(companyId, name, description, mathType, factor, reference, averageTime, wageAdjuster, salaryForm, paymentSystem));
            _context.SaveChanges();
        }

        private void AddOccupationIfNotExists(int companyId, string code, string description, char category, string responsibility, string group, string groupDescription = null, params NameValue[] requirements)
        {
            if (_context.Occupations.IgnoreQueryFilters()
                .Include(g => g.Group)
                .Include(c => c.Category)
                .Include(r => r.Responsibility)
                .Any(s => s.DisplayName == description
                          && s.CompanyId == companyId
                          && s.Group.Group == group
                          && s.Group.Description == groupDescription
                          && s.Category.Code == category
                          && s.Responsibility.DisplayName == responsibility))
                return;

            var complexityGroup = _context.ComplexityGroups.IgnoreQueryFilters().FirstOrDefault(g => g.Group == group && g.Description == groupDescription);
            var cat = _context.OccupationalCategories.IgnoreQueryFilters().FirstOrDefault(g => g.Code == category);
            var resp = _context.Responsibilities.IgnoreQueryFilters().FirstOrDefault(g => g.DisplayName == responsibility);
            var occupation = new Occupation(companyId, description, complexityGroup, cat, resp);

            if (!code.IsNullOrEmpty())
                occupation.Code = code;

            _context.Occupations.Add(occupation);
            _context.SaveChanges();
        }

        private void AddQualificationRequirementDefinitionIfNotExists(int companyId, string occupationCode, string displayName, string acronym)
        {
            if (_context.QualificationRequirementDefinition.IgnoreQueryFilters()
                .Include(o => o.Occupation)
                .Any(s => s.DisplayName == displayName && s.CompanyId == companyId &&
                          s.Occupation.Code == occupationCode))
                return;

            var occupation = _context.Occupations.IgnoreQueryFilters().FirstOrDefault(g => g.Code == occupationCode);
            if(occupation == null )
                return;

            var qualificationRequirementDefinition = new QualificationRequirementDefinition(displayName)
                {
                    CompanyId = companyId,
                    OccupationId = occupation.Id
                };

            _context.QualificationRequirementDefinition.Add(qualificationRequirementDefinition);
            _context.SaveChanges();
        }
        #endregion
    }
}
