using System;
using System.Linq;
using Castle.Core.Logging;
using Itenso.TimePeriod;
using Kontecg.Accounting;
using Kontecg.DynamicEntityProperties;
using Kontecg.EFCore;
using Kontecg.Extensions;
using Kontecg.Organizations;
using Kontecg.Timing;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Migrations.Seed.Companies
{
    public class CompanyAccountingInfoBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly int _companyId;
        private readonly ILogger _logger;

        public CompanyAccountingInfoBuilder(KontecgCoreDbContext context, int companyId, ILogger logger)
        {
            _context = context;
            _companyId = companyId;
            _logger = logger;
        }

        public void Create()
        {
            CreateCenterCostDefinitions();
            CreateExpenseItemDefinitions();
            CreateWorkPlacePayments();
            CreateDynamicProperties();
            GeneratePeriods();
        }

        private void GeneratePeriods()
        {
            var year = Clock.Now.Year;
            var months = Enum.GetValuesAsUnderlyingType<YearMonth>();
            var calendar = WorkCalendarTool.New();
            foreach (var month in months)
                AddPeriodIfNotExists(_companyId, "SGNOM", calendar, year, (YearMonth) month);

            _logger.Info($"Accounting periods for year {year} was created.");
        }

        private void CreateCenterCostDefinitions()
        {
            AddCenterCostIfNotExists(_companyId, "GEOLOGIA DESARROLLO", 100101, 702);
            AddCenterCostIfNotExists(_companyId, "DESTAPE DE MINA", 100102, 702);
            AddCenterCostIfNotExists(_companyId, "CONSTRUCCION Y MTTO DE CAMINOS", 100103, 702);
            AddCenterCostIfNotExists(_companyId, "DEPOSITOS MINEROS", 100108, 702);
            AddCenterCostIfNotExists(_companyId, "EXTRACCION DE MINERAL", 100110, 702);
            AddCenterCostIfNotExists(_companyId, "TRANSPORTE DE MINERAL", 100111, 702);
            AddCenterCostIfNotExists(_companyId, "GEOLOGIA PRODUCCION", 100112, 702);
            AddCenterCostIfNotExists(_companyId, "TOPOGRAFIA", 100113, 702);
            AddCenterCostIfNotExists(_companyId, "RECEPCION DE MINERAL", 100114, 702);
            AddCenterCostIfNotExists(_companyId, "PLANTA DE SECADERO", 100121, 702);
            AddCenterCostIfNotExists(_companyId, "PLANTA DE HORNOS DE REDUCCION", 100122, 702);
            AddCenterCostIfNotExists(_companyId, "PLANTA DE LIXIVIACION Y LAVADO", 100123, 702);
            AddCenterCostIfNotExists(_companyId, "PLANTA DE SULFURO", 100124, 702);
            AddCenterCostIfNotExists(_companyId, "PLANTA DE RECUPERACION DE AMONIACO", 100125, 702);
            AddCenterCostIfNotExists(_companyId, "PLANTA DE CALCINACION Y SINTER", 100126, 702);
            AddCenterCostIfNotExists(_companyId, "PLANTA POTABILIZADORA DE H2O", 100201, 709);
            AddCenterCostIfNotExists(_companyId, "PLANTA DE VAPOR", 100202, 709);
            AddCenterCostIfNotExists(_companyId, "ENERGIA ELECTRICA", 100203, 709);
            AddCenterCostIfNotExists(_companyId, "AGUA SUAVIZADA", 100204, 709);
            AddCenterCostIfNotExists(_companyId, "AIRE", 100205, 709);
            AddCenterCostIfNotExists(_companyId, "TORRES DE ENFRIAMIENTO", 100206, 709);
            AddCenterCostIfNotExists(_companyId, "OPERACIONES 2", 100207, 709);
            AddCenterCostIfNotExists(_companyId, "DIP TERMOELECTRICA", 400210, 709);
            AddCenterCostIfNotExists(_companyId, "DIRECCION DE INVERSIONES", 400226, 709);
            AddCenterCostIfNotExists(_companyId, "TALLER MTTO AUTOMOTOR", 400301, 731);
            AddCenterCostIfNotExists(_companyId, "BRIG ELECTRICA MINA", 400302, 731);
            AddCenterCostIfNotExists(_companyId, "MTTO REVERBERIA, MECANICA Y PAILERI", 400310, 731);
            AddCenterCostIfNotExists(_companyId, "MTTO MECANICO 2", 400313, 731);
            AddCenterCostIfNotExists(_companyId, "TALLER MISCELANEO", 400314, 731);
            AddCenterCostIfNotExists(_companyId, "TALLER MTTO MECANICO", 400315, 731);
            AddCenterCostIfNotExists(_companyId, "TALLER ELECTRICO", 400316, 731);
            AddCenterCostIfNotExists(_companyId, "TALLER INSTRUMENTOS", 400317, 731);
            AddCenterCostIfNotExists(_companyId, "DIRECCIÓN Y DEPRECIACIÓN AFT TERMOENERGÉTICA", 400318, 731);
            AddCenterCostIfNotExists(_companyId, "UB MTTO", 400319, 731);
            AddCenterCostIfNotExists(_companyId, "CANALIZACION Y ESTACADA", 400320, 731);
            AddCenterCostIfNotExists(_companyId, "LABORATORIO CENTRAL", 400321, 731);
            AddCenterCostIfNotExists(_companyId, "MTTO INDUSTRIAL", 400322, 731);
            AddCenterCostIfNotExists(_companyId, "GRUPO ALMACENES ORKS", 400323, 731);
            AddCenterCostIfNotExists(_companyId, "TRANSPORTE DE CARGA", 400324, 731);
            AddCenterCostIfNotExists(_companyId, "LIMPIEZA", 400325, 731);
            AddCenterCostIfNotExists(_companyId, "GRUPO ALMACEN CENTRAL", 400326, 731);
            AddCenterCostIfNotExists(_companyId, "ALMACEN DE ROLO", 400327, 731);
            AddCenterCostIfNotExists(_companyId, "BASE ALMACEN CENTRAL", 400328, 731);
            AddCenterCostIfNotExists(_companyId, "GRUPO ICT", 400329, 822);
            AddCenterCostIfNotExists(_companyId, "SUBDIRECCION PRODUCTIVA", 400331, 731);
            AddCenterCostIfNotExists(_companyId, "CONTROL DE LA CALIDAD", 400333, 731);
            AddCenterCostIfNotExists(_companyId, "SEGURIDAD INDUSTRIAL", 400336, 731);
            AddCenterCostIfNotExists(_companyId, "SUDIRECCION TECNICA", 400339, 731);
            AddCenterCostIfNotExists(_companyId, "SUBDIRECCION MINA", 400341, 731);
            AddCenterCostIfNotExists(_companyId, "DPTO CONTABILIDAD METALURGICA", 400342, 731);
            AddCenterCostIfNotExists(_companyId, "GRUPO ECONOMIA ENERGETICA", 400343, 731);
            AddCenterCostIfNotExists(_companyId, "DPTO TRANSPORTE", 400344, 731);
            AddCenterCostIfNotExists(_companyId, "PLANTA DIESEL", 400345, 731);
            AddCenterCostIfNotExists(_companyId, "DPTO TECNICO MTTO", 400346, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR MTTO IZAJE", 400347, 731);
            AddCenterCostIfNotExists(_companyId, "GRUPO DE COMPUTACION", 400349, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR DE MUESTREO", 400350, 731);
            AddCenterCostIfNotExists(_companyId, "DIRECCION SEGURIDAD INDUSTRIAL", 400351, 822);
            AddCenterCostIfNotExists(_companyId, "GRUPO DE EMERGENCIA", 400352, 731);
            AddCenterCostIfNotExists(_companyId, "GRUPO TÉCNICO  DIRECCIÓN TÉCNICA", 400354, 731);
            AddCenterCostIfNotExists(_companyId, "GRUPO PUESTO MEDICO", 400355, 731);
            AddCenterCostIfNotExists(_companyId, "BRIGADA LIMPIEZA DE GASES", 400367, 731);
            AddCenterCostIfNotExists(_companyId, "MTTO PLANTA PILOTO CEINI", 400368, 731);
            AddCenterCostIfNotExists(_companyId, "BRIG. REPARAC EQUIPOS LIGEROS", 400369, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR MTTO PREPARACION DE MINERAL", 400376, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR MTTO HORNOS DE REDUCCION", 400377, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR MTTO RECUPERACION NH3", 400378, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR MTTO LIXIVIACION Y LAVADO", 400379, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR MTTO CALCINACION Y SINTER", 400380, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR MTTO PLANTAS AUXILIARES", 400381, 731);
            AddCenterCostIfNotExists(_companyId, "SECTOR MTTO PLANTA AGUA", 400382, 731);
            AddCenterCostIfNotExists(_companyId, "BRIGADA DE ACARREO Y TIRO DE CARBON", 400383, 731);
            AddCenterCostIfNotExists(_companyId, "BRIGADA DE MTTO DE AMONIACO", 400384, 731);
            AddCenterCostIfNotExists(_companyId, "DIRECCIÓN DE SEGURIDAD, SALUD Y MEDIO AMBIENTE", 400385, 731);
            AddCenterCostIfNotExists(_companyId, "UB MINERA EN ECG", 400386, 731);
            AddCenterCostIfNotExists(_companyId, "DIRECCION", 400401, 822);
            AddCenterCostIfNotExists(_companyId, "DIRECTOR ADJUNTO", 400402, 822);
            AddCenterCostIfNotExists(_companyId, "MEDIO AMBIENTE", 400403, 822);
            AddCenterCostIfNotExists(_companyId, "SUB-DIRECCION DE RECURSOS HUMANOS", 400404, 822);
            AddCenterCostIfNotExists(_companyId, "SUB-DIRECCION ECONOMICA", 400405, 822);
            AddCenterCostIfNotExists(_companyId, "SUB-DIRECCION DE ABASTECIMIENTO", 400406, 822);
            AddCenterCostIfNotExists(_companyId, "GRUPO RECURSOS LABORALES", 400407, 822);
            AddCenterCostIfNotExists(_companyId, "SEGURIDAD Y PROTECCION", 400408, 822);
            AddCenterCostIfNotExists(_companyId, "GRUPO TECNICO", 400409, 731);
            AddCenterCostIfNotExists(_companyId, "GRUPO TECNICO DE RECURSOS HUMANOS", 400411, 822);
            AddCenterCostIfNotExists(_companyId, "DPTO ASESOR", 400413, 822);
            AddCenterCostIfNotExists(_companyId, "ASESOR JURIDICO", 400414, 822);
            AddCenterCostIfNotExists(_companyId, "DPTO DE CUADROS", 400415, 822);
            AddCenterCostIfNotExists(_companyId, "GRUPO DE AUDITORIA", 400416, 822);
            AddCenterCostIfNotExists(_companyId, "GRUPO CONTROL INTERNO", 400417, 822);
            AddCenterCostIfNotExists(_companyId, "SERVICIOS GENERALES", 400420, 822);
            AddCenterCostIfNotExists(_companyId, "GRUPO CAPACITACION Y ADIESTRAMIENTO", 400421, 822);
            AddCenterCostIfNotExists(_companyId, "DIRECCION DE INVERSIONES", 400426, 822);
            AddCenterCostIfNotExists(_companyId, "DESASTRE X HURACAN", 402609, 866);
            AddCenterCostIfNotExists(_companyId, "CONTROL DE LA CALIDAD", 402622, 866);
            AddCenterCostIfNotExists(_companyId, "DIP REPARACIONES CAPITALES", 408211, 728);
            AddCenterCostIfNotExists(_companyId, "NUEVA PRESA DE COLA", 408238, 728);
            AddCenterCostIfNotExists(_companyId, "MANTENIMIENTO PREPARACION MINERAL", 403376, 731);
            AddCenterCostIfNotExists(_companyId, "MANTENIMIENTO HORNOS DE REDUCCION", 403377, 731);
            AddCenterCostIfNotExists(_companyId, "MANTENIMIENTO RECUPERACION NH3 Y CO", 403378, 731);
            AddCenterCostIfNotExists(_companyId, "MANTENIMIENTO LIXIVIACION Y LAVADO", 403379, 731);
            AddCenterCostIfNotExists(_companyId, "MANTENIMIENTO CALCINACION Y SINTER", 403380, 731);
            AddCenterCostIfNotExists(_companyId, "MANTENIMIENTO TERMOENERGETICA", 403381, 731);
            AddCenterCostIfNotExists(_companyId, "MANTENIMIENTO RECEPCION Y SUMINISTROS", 403382, 731);
            AddCenterCostIfNotExists(_companyId, "MANTENIMIENTO DE AMONIACO", 403384, 731);
            _logger.Info("Center costs was created.");
        }

        private void CreateExpenseItemDefinitions()
        {
            AddExpenseItemIfNotExists(_companyId, "Salario", 500101, "SALARIO");
            AddExpenseItemIfNotExists(_companyId, "Salario Personal Contratado", 500102, "SALARIO_CONTRATOS");

            AddExpenseItemIfNotExists(_companyId, "Habilitado de Descanso", 500500, "HD");
            AddExpenseItemIfNotExists(_companyId, "Horas Extras", 500501, "HORAS_EXTRAS");
            AddExpenseItemIfNotExists(_companyId, "Doble Turno", 500502, "DOBLE_TURNO");
            AddExpenseItemIfNotExists(_companyId, "Nocturnidad", 500506, "NOCTURNIDAD");
            AddExpenseItemIfNotExists(_companyId, "Homologación", 500507, "HOMOLOGACION");
            AddExpenseItemIfNotExists(_companyId, "Maestría, Doctorado y Grado Científico", 500508, "GRADO");
            AddExpenseItemIfNotExists(_companyId, "Dias Feriados y Festivos", 500509, "DFF");
            AddExpenseItemIfNotExists(_companyId, "Trabajadores en Curso", 500510);
            AddExpenseItemIfNotExists(_companyId, "Movilizado por el Comité Militar hasta 10 Dias", 500511, "MOV_10");
            AddExpenseItemIfNotExists(_companyId, "Ausencias Autorizadas por la Legislación", 500512, "AAL");
            AddExpenseItemIfNotExists(_companyId, "Salario Pluriempleo", 500521);
            AddExpenseItemIfNotExists(_companyId, "Horario Irregular", 500522, "HORARIO_IRREGULAR");
            AddExpenseItemIfNotExists(_companyId, "Movilizados por Ciclones", 500525, "MOV_CI");
            AddExpenseItemIfNotExists(_companyId, "Ausente al trabajo por Afectación a causa de ciclones", 500526);
            AddExpenseItemIfNotExists(_companyId, "Salarios estudiantes", 500535, "PREGADO");

            AddExpenseItemIfNotExists(_companyId, "Pago por Resultado", 500701, "ESTIMULACION");

            AddExpenseItemIfNotExists(_companyId, "Acumulación de Vacaciones (9.09%)", 500901, "VACACIONES");

            AddExpenseItemIfNotExists(_companyId, "Gastos Salarial Trabajadores Interruptos (Por el Ciclón, etc.)", 800813, "INTERRUPTO");
            AddExpenseItemIfNotExists(_companyId, "Tratamiento Salarial 100% COVID-19", 800825, "COVID_100");
            AddExpenseItemIfNotExists(_companyId, "Tratamiento Salarial 60% COVID-19", 800826, "COVID_60");
            AddExpenseItemIfNotExists(_companyId, "Tratamiento Salarial 50% COVID-19", 800835,"COVID_50");
            AddExpenseItemIfNotExists(_companyId, "Tratamiento Salarial Proceso Vacunación", 800836, "COVID_VACUNA");

            _logger.Info("Expense items was created.");
        }

        private void CreateWorkPlacePayments()
        {
            AddWorkPlacePaymentIfNotExists(_companyId, "01", "Mantenimiento Mina");
            AddWorkPlacePaymentIfNotExists(_companyId, "02", "Unidad Básica Minera");
            AddWorkPlacePaymentIfNotExists(_companyId, "03", "UB Producción Planta Preparación del Mineral");
            AddWorkPlacePaymentIfNotExists(_companyId, "04", "UB Producción Planta Hornos de Reducción");
            AddWorkPlacePaymentIfNotExists(_companyId, "05", "UB Producción Planta Lixiviación y Lavado");
            AddWorkPlacePaymentIfNotExists(_companyId, "06", "UBP Recuperación de Amoníaco y Cobalto");
            AddWorkPlacePaymentIfNotExists(_companyId, "07", "UB Producción Planta Calcinación y Sínter");
            AddWorkPlacePaymentIfNotExists(_companyId, "08", "UB Servicios Planta Termoenergética");
            AddWorkPlacePaymentIfNotExists(_companyId, "09", "UB Servicios Planta de Recepción y Suministro");
            AddWorkPlacePaymentIfNotExists(_companyId, "10", "Mantenimiento Preparación de Mineral");
            AddWorkPlacePaymentIfNotExists(_companyId, "11", "Mantenimiento Eléctrico");
            AddWorkPlacePaymentIfNotExists(_companyId, "12", "Mantenimiento Mecánico");
            AddWorkPlacePaymentIfNotExists(_companyId, "13", "Mantenimiento Izaje Industrial");
            AddWorkPlacePaymentIfNotExists(_companyId, "15", "Mantenimiento Misceláneo");
            AddWorkPlacePaymentIfNotExists(_companyId, "16", "Mantenimiento Instrumentos");
            AddWorkPlacePaymentIfNotExists(_companyId, "17", "Mantenimiento Hornos de Reducción");
            AddWorkPlacePaymentIfNotExists(_companyId, "20", "Dirección de Inversiones");
            AddWorkPlacePaymentIfNotExists(_companyId, "22", "Operaciones Transporte");
            AddWorkPlacePaymentIfNotExists(_companyId, "25", "Mantenimiento Recuperación NH3 y Cobalto");
            AddWorkPlacePaymentIfNotExists(_companyId, "26", "Mantenimiento Lixiviación y Lavado");
            AddWorkPlacePaymentIfNotExists(_companyId, "27", "Mantenimiento Calcinación y Sínter");
            AddWorkPlacePaymentIfNotExists(_companyId, "28", "Dirección de Economía y Finanzas");
            AddWorkPlacePaymentIfNotExists(_companyId, "29", "Dirección de Recursos Humanos");
            AddWorkPlacePaymentIfNotExists(_companyId, "30", "Dirección de Seguridad Salud y Medio Ambiente");
            AddWorkPlacePaymentIfNotExists(_companyId, "32", "UB Mantenimiento");
            AddWorkPlacePaymentIfNotExists(_companyId, "33", "Mantenimiento Electrofiltro");
            AddWorkPlacePaymentIfNotExists(_companyId, "34", "Mantenimiento Automotor");
            AddWorkPlacePaymentIfNotExists(_companyId, "35", "Dirección de Producción");
            AddWorkPlacePaymentIfNotExists(_companyId, "39", "Dirección General");
            AddWorkPlacePaymentIfNotExists(_companyId, "44", "Mantenimiento Termoenergética");
            AddWorkPlacePaymentIfNotExists(_companyId, "46", "Mantenimiento Mecánico 2");
            AddWorkPlacePaymentIfNotExists(_companyId, "47", "Mantenimiento Recepción y Suministro");
            AddWorkPlacePaymentIfNotExists(_companyId, "48", "DIP Reparaciones Capitales");
            AddWorkPlacePaymentIfNotExists(_companyId, "50", "UB de Servicios Técnicos a la Producción");
            AddWorkPlacePaymentIfNotExists(_companyId, "51", "Mantenimiento Reverbería, Mecánica y Pailería");
            AddWorkPlacePaymentIfNotExists(_companyId, "56", "UB Abastecimiento");
            AddWorkPlacePaymentIfNotExists(_companyId, "57", "Dirección de Seguridad y Protección");

            _logger.Info("Workplace payments was created.");
        }

        private void CreateDynamicProperties()
        {
            //Variables dinámicas para los documentos que sean nóminas
            AddDynamicPropertyIfNotExists(_companyId, "CSS", "Aporta a la Seguridad Social", "CHECKBOX");
            AddDynamicPropertyIfNotExists(_companyId, "ISIP", "Impuesto sobre Ingresos Personales", "CHECKBOX");
            AddDynamicPropertyIfNotExists(_companyId, "RET", "Retenciones", "CHECKBOX");

            //Variables dinámicas para personas en concreto
            AddDynamicPropertyIfNotExists(_companyId, "ISIP-50", "Devolución 50% ISIP", "CHECKBOX");

            _logger.Info("Dynamic properties was created.");
        }

        
        private void AddCenterCostIfNotExists(int companyId, string description, int code, int account, string reference = null)
        {
            var referenceToSetUp = reference.IsNullOrWhiteSpace() ? $"CC_{account}_{code}" : reference;

            var cc = _context.CenterCostDefinitions.IgnoreQueryFilters()
                .FirstOrDefault(c => c.CompanyId == companyId && c.Code == code && c.Reference == referenceToSetUp && c.IsActive);

            if (cc != null) return;

            var acc = _context.AccountDefinitions.IgnoreQueryFilters()
                .FirstOrDefault(a => a.Account == account);
            if (acc == null) return;

            _context.CenterCostDefinitions.Add(new CenterCostDefinition(acc, description, code, referenceToSetUp)
                {CompanyId = companyId});
            _context.SaveChanges();
        }

        private void AddExpenseItemIfNotExists(int companyId, string description, int code, string reference = null)
        {
            var referenceToSetUp = reference.IsNullOrWhiteSpace() ? $"EG_{code}" : reference;

            var cc = _context.ExpenseItemDefinitions.IgnoreQueryFilters()
                .FirstOrDefault(c => c.CompanyId == companyId && c.Code == code && c.Reference == referenceToSetUp && c.IsActive);

            if (cc != null) return;
            _context.ExpenseItemDefinitions.Add(new ExpenseItemDefinition(description, code, referenceToSetUp) {CompanyId = companyId });
            _context.SaveChanges();
        }

        private void AddWorkPlacePaymentIfNotExists(int companyId, string code, string description)
        {
            if (_context.WorkPlacePayments.IgnoreQueryFilters().Any(s => s.CompanyId == companyId && s.Code == code))
                return;

            _context.WorkPlacePayments.Add(new WorkPlacePayment(code, description){CompanyId = companyId});
            _context.SaveChanges();
        }


        private void AddPeriodIfNotExists(int companyId, string moduleKey, ITimeCalendar calendar, int year, YearMonth month)
        {
            if (_context.AccountingPeriods.IgnoreQueryFilters()
                        .FirstOrDefault(p => p.ReferenceGroup == moduleKey && p.Year == year && p.Month == month && p.CompanyId == companyId) != null)
                return;

            var period = new Period(moduleKey, calendar, year, month) {CompanyId = companyId};

            if (year <= Clock.Now.Year && (int) month < Clock.Now.Month)
                period.Status = PeriodStatus.Closed;

            _context.AccountingPeriods.Add(period);
            _context.SaveChanges();
        }

        private DynamicProperty AddDynamicPropertyIfNotExists(int companyId, string propertyName, string displayName, string inputType, string permission = null)
        {
            if (_context.DynamicProperties.IgnoreQueryFilters().Any(p => p.PropertyName == propertyName && p.CompanyId == companyId))
                return _context.DynamicProperties.IgnoreQueryFilters().Single(p => p.PropertyName == propertyName && p.CompanyId == companyId);

            var dynamicProperty = new DynamicProperty
            {
                CompanyId = companyId,
                PropertyName = propertyName,
                DisplayName = displayName,
                InputType = inputType,
                Permission = permission
            };

            var entityEntry = _context.DynamicProperties.Add(dynamicProperty);
            _context.SaveChanges();
            return entityEntry.Entity;
        }
    }
}
