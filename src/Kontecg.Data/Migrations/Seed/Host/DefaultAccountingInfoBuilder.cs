using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using JetBrains.Annotations;
using Kontecg.Accounting;
using Kontecg.Currencies;
using Kontecg.Domain;
using Kontecg.DynamicEntityProperties;
using Kontecg.EFCore;
using Kontecg.Extensions;
using Kontecg.Organizations;
using Kontecg.Workflows;
using Microsoft.EntityFrameworkCore;
using NMoneys;

namespace Kontecg.Migrations.Seed.Host
{
    public class DefaultAccountingInfoBuilder
    {
        private readonly KontecgCoreDbContext _context;
        private readonly ILogger _logger;

        public DefaultAccountingInfoBuilder(KontecgCoreDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Create()
        {
            CreateWorkPlaceClassifications();
            CreateAccountDefinitions();
            CreateAccountingFunctionDefinition();
            CreateAccountingClassifierDefinitions();
            CreateViewNames();
            CreateDocumentDefinitions();
            CreateBanks();
            CreateBillDenominations();
        }

        private void CreateWorkPlaceClassifications()
        {
            AddWorkPlaceClassificationIfNotExists("EMPRESA", 0);
            AddWorkPlaceClassificationIfNotExists("Dirección General");
            AddWorkPlaceClassificationIfNotExists("Direcciones Funcionales");
            AddWorkPlaceClassificationIfNotExists("Plantas");
            AddWorkPlaceClassificationIfNotExists("UEB");
            AddWorkPlaceClassificationIfNotExists("Divisiones");
            AddWorkPlaceClassificationIfNotExists("Dirección");
            AddWorkPlaceClassificationIfNotExists("SubDirección");
            AddWorkPlaceClassificationIfNotExists("Unidades");
            AddWorkPlaceClassificationIfNotExists("Áreas de apoyo");
            AddWorkPlaceClassificationIfNotExists("Direcciones Integradas (DIP y DIC)");
            AddWorkPlaceClassificationIfNotExists("Unidades de servicios", 2);
            AddWorkPlaceClassificationIfNotExists("Áreas", 2);
            AddWorkPlaceClassificationIfNotExists("Bases", 2);
            AddWorkPlaceClassificationIfNotExists("Taller", 2);
            AddWorkPlaceClassificationIfNotExists("Laboratorio", 2);
            AddWorkPlaceClassificationIfNotExists("Brigada", 3);
            AddWorkPlaceClassificationIfNotExists("Grupo", 3);
            AddWorkPlaceClassificationIfNotExists("Almacenes", 3);
            AddWorkPlaceClassificationIfNotExists("Otras áreas", 3);
            AddWorkPlaceClassificationIfNotExists("Brigadas de apoyo", 3);
            
            _logger.Info("WorkPlaceClassification created.");
        }

        private void CreateAccountingClassifierDefinitions()
        {
            AddAccountingClassifierDefinitionIfNotExists(ScopeData.Company.ToString());
            AddAccountingClassifierDefinitionIfNotExists(ScopeData.Personal.ToString());

            _logger.Info("Classifiers was created.");
        }
        
        private void CreateAccountDefinitions()
        {
            AddAccountDefinitionIfNotExists(35, 0, 0, 0, "Facilidades a trabajadores (DL 91)", AccountKind.Debit);
            
            AddAccountDefinitionIfNotExists(99, 35, 0, 0, "Facilidades a Trabajadores", AccountKind.Credit);
            
            AddAccountDefinitionIfNotExists(101, 11, 0, 0, "Fondo Especial para Nóminas", AccountKind.Debit, "FECAJA");
            AddAccountDefinitionIfNotExists(101, 14, 0, 0, "Extraído para Nóminas", AccountKind.Debit, "EXTNOM");
            
            AddAccountDefinitionIfNotExists(110, 7262, 0, 0, "Pago al banco", AccountKind.Debit);
            
            AddAccountDefinitionIfNotExists(164, 19, 0, 0, "Prestación monetaria madres con hijos enfermos (Articulo 47.1, DL 56/2021)", AccountKind.Debit, "M_H_ENF");
            AddAccountDefinitionIfNotExists(164, 20, 0, 0, "Invalidez Parcial", AccountKind.Debit, "INVPAR");
            AddAccountDefinitionIfNotExists(164, 30, 0, 0, "Licencia de Maternidad (DL 234/2003) 60% y 100%", AccountKind.Debit, "MATERNIDAD");
            AddAccountDefinitionIfNotExists(164, 30, 0, 0, "Prestación monetaria gestante enferma", AccountKind.Debit, "GESTENF");
            
            AddAccountDefinitionIfNotExists(164, 90, 0, 0, "Otros (Movilizado más de 10 días, Enfermedades ITS, etc.)", AccountKind.Debit);
            AddAccountDefinitionIfNotExists(167, 20, 0, 0, "Decreto Ley 91", AccountKind.Debit);
            
            AddAccountDefinitionIfNotExists(334, 0, 0, 0, "Cuenta por cobrar al trabajador", AccountKind.Debit,"CTAXCOB");
            AddAccountDefinitionIfNotExists(334, 20, 0, 0, "Deudas de Trabajadores", AccountKind.Debit, "R_DEUDA");
            AddAccountDefinitionIfNotExists(334, 40, 0, 0, "Responsabilidad Material", AccountKind.Debit, "R_DL92");
            AddAccountDefinitionIfNotExists(334, 99, 0, 0, "Otras deudas", AccountKind.Debit ,"R_OTROS");
            
            AddAccountDefinitionIfNotExists(440, 6, 0, 0, "Impuesto sobre los Recursos (Imp. Utl fuerza de Trabajo)", AccountKind.Credit);
            AddAccountDefinitionIfNotExists(440, 8, 0, 0, "Contribuciones a la Seguridad Social 12.5%", AccountKind.Credit);
            AddAccountDefinitionIfNotExists(440, 10, 0, 0, "Ingresos no tributarios (Canon, Derecho de Superficie, Reintegros Sal., Venta ropa a trabajadores, Multas, Rend. Inv. Estatal, Dividendos)", AccountKind.Credit, "C_MULTA");

            AddAccountDefinitionIfNotExists(455, 0, 0, 0, "Nóminas por pagar", AccountKind.Credit,"NOMPAGAR");

            AddAccountDefinitionIfNotExists(460, 1, 0, 0, "Pensiones Alimenticias (**)", AccountKind.Credit,"R_PALIM");
            AddAccountDefinitionIfNotExists(460, 2, 0, 0, "Créditos a la Población (**)", AccountKind.Credit, "R_CPOBLA");
            AddAccountDefinitionIfNotExists(460, 3, 0, 0, "Ley General de la Vivienda (**)", AccountKind.Credit, "R_LGVIVI");
            AddAccountDefinitionIfNotExists(460, 4, 0, 0, "Préstamos Estudiantiles (**)", AccountKind.Credit, "R_PESTUD");
            AddAccountDefinitionIfNotExists(460, 5, 0, 0, "Formación de Fondos (Cuenta de Ahorro) (**)", AccountKind.Credit, "R_FFOND");
            AddAccountDefinitionIfNotExists(460, 7, 0, 0, "Embargos Judiciales (**)", AccountKind.Credit, "R_EJUDI");
            AddAccountDefinitionIfNotExists(460, 12, 0, 0, "Créditos Sociales (**)", AccountKind.Credit, "R_CRESOC");
            AddAccountDefinitionIfNotExists(460, 13, 0, 0, "Vivienda Vinculadas (**)", AccountKind.Credit, "R_VVINCU");
            AddAccountDefinitionIfNotExists(460, 19, 0, 0, "Seguros de Vida (**)", AccountKind.Credit, "R_SVIDA");

            AddAccountDefinitionIfNotExists(462, 1, 0, 0, "Contribución Especial a la Seguridad Social", AccountKind.Credit, "R_SEGSOC");
            AddAccountDefinitionIfNotExists(462, 2, 0, 0, "Impuesto sobre los Ingresos Personales", AccountKind.Credit, "R_IMPING");
            AddAccountDefinitionIfNotExists(462, 3, 0, 0, "Responsabilidad Material", AccountKind.Credit, "R_RESMAT");

            AddAccountDefinitionIfNotExists(492, 0, 0, 0, "Provisión para vacaciones", AccountKind.Credit, "VACACIONES");

            AddAccountDefinitionIfNotExists(494, 10, 0, 0, "Otras Provisiones (ANIR)", AccountKind.Credit, "ANIR");

            AddAccountDefinitionIfNotExists(500, 0, 0, 0, "Provisión para subsidios", AccountKind.Credit, "SUBSIDIO");

            AddAccountDefinitionIfNotExists(567, 70, 0, 0, "Salarios no Reclamados", AccountKind.Credit, "SAL_NREC");
            
            AddAccountDefinitionIfNotExists(690, 30, 0, 0, "Distribución de Utilidades a Trabajadores", AccountKind.Debit,"UTILIDADES");

            AddAccountDefinitionIfNotExists(699, 0, 0, 0, "Transitoria del Sistema Automatizado", AccountKind.Memo, "TRANSITORIA");
            
            AddAccountDefinitionIfNotExists(702, 0, 0, 0, "PRODUCCIÓN PRINCIPAL EN PROCESO", AccountKind.Debit);

            AddAccountDefinitionIfNotExists(709, 0, 0, 0, "PRODUCCIONES AUXILIARES EN PROCESO", AccountKind.Debit);

            AddAccountDefinitionIfNotExists(728, 0, 0, 0, "INVERSIONES CON MEDIOS PROPIOS", AccountKind.Debit);

            AddAccountDefinitionIfNotExists(731, 0, 0, 0, "GASTOS INDIRECTOS DE PRODUCCIÓN", AccountKind.Debit);

            AddAccountDefinitionIfNotExists(822, 0, 0, 0, "GASTOS GENERALES Y DE ADMINISTRACIÓN", AccountKind.Debit);

            AddAccountDefinitionIfNotExists(855, 10, 0, 0, "Contribución a la Seguridad Social a Largo Plazo (12.5%)", AccountKind.Debit,"SEGSOC12");

            AddAccountDefinitionIfNotExists(856, 10, 0, 0, "Impuesto de la Fuerza de Trabajo (5%)", AccountKind.Debit, "FT25");

            AddAccountDefinitionIfNotExists(857, 10, 0, 0, "Gastos Planificados de los Subsidios a Corto Plazo (1.5%)", AccountKind.Debit, "GPS15");

            AddAccountDefinitionIfNotExists(866, 0, 0, 0, "Otros gastos", AccountKind.Debit, "OTROS_GASTOS");
            AddAccountDefinitionIfNotExists(866, 402665, 0, 0, "Otros gastos - Interrupciones Laborales", AccountKind.Debit, "INTERRUPCIONES");

            AddAccountDefinitionIfNotExists(873, 400019, 0, 0, "Gastos por pérdidas – Desastres", AccountKind.Debit, "DESASTRES");

            AddAccountDefinitionIfNotExists(951, 11, 0, 0, "Otros Ingresos", AccountKind.Credit, "OTROS_INGRESOS");

            _logger.Info("AccountDefinition created.");
        }

        private void CreateAccountingFunctionDefinition()
        {
            AddAccountingFunctionDefinitionIfNotExists("EMPTY", "FUNCIÓN PREDETERMINADA (NO CONTABILIZA)", "EMPTY");
            
            AddAccountingFunctionDefinitionIfNotExists("C_COVID_50", "GASTOS POR PERDIDAS–DESASTRES 50%", "SAL");
            AddAccountingFunctionDefinitionIfNotExists("C_COVID_60", "GASTOS POR PERDIDAS–DESASTRES 60%", "SAL");
            AddAccountingFunctionDefinitionIfNotExists("C_COVID_100", "GASTOS POR PERDIDAS–DESASTRES 100%", "SAL");
            AddAccountingFunctionDefinitionIfNotExists("C_COVID_M", "GASTOS POR PERDIDAS–MADRES", "SAL");
            
            AddAccountingFunctionDefinitionIfNotExists("C_HI", "Horario Irregular", "SAL");
            AddAccountingFunctionDefinitionIfNotExists("C_INT_MN", "Interrupto", "SAL");
            AddAccountingFunctionDefinitionIfNotExists("C_MOV_CI", "Movilizados Ciclón", "SAL");
            AddAccountingFunctionDefinitionIfNotExists("C_PREGADO", "Salario de Estudiantes", "SAL");
            AddAccountingFunctionDefinitionIfNotExists("C_RECLAM", "Reclamaciones", "SAL");
            AddAccountingFunctionDefinitionIfNotExists("C_VACUNA", "Vacunación", "SAL");

            AddAccountingFunctionDefinitionIfNotExists("C_PPEN", "Pago de Pensiones", "PA");

            AddAccountingFunctionDefinitionIfNotExists("C_CLA", "Condiciones Anormales Laborales", "ADIC");
            AddAccountingFunctionDefinitionIfNotExists("C_CERT", "Certificación", "ADIC");
            AddAccountingFunctionDefinitionIfNotExists("C_GRADO", "Grado Científico", "ADIC");
            AddAccountingFunctionDefinitionIfNotExists("C_NOC", "Nocturnidad", "ADIC");

            AddAccountingFunctionDefinitionIfNotExists("C_DFF", "Dia Feriado o Festivo", "TPG");
            AddAccountingFunctionDefinitionIfNotExists("C_HE", "Horas Extras", "TPG");
            AddAccountingFunctionDefinitionIfNotExists("C_MTT", "Movilizaciones Militares", "TPG");
            AddAccountingFunctionDefinitionIfNotExists("C_TP504", "Tiempo Trabajado Normal (504)", "TPG");

            AddAccountingFunctionDefinitionIfNotExists("C_PVAC", "Pago de Vacaciones", "VAC");

            AddAccountingFunctionDefinitionIfNotExists("C_INVPAR", "Invalidez Parcial", "SEGSOC");
            AddAccountingFunctionDefinitionIfNotExists("C_INVTEMP", "Invalidez Temporal", "SEGSOC");
            AddAccountingFunctionDefinitionIfNotExists("C_M_G_ENF", "Prestación Gestante enferma", "SEGSOC");
            AddAccountingFunctionDefinitionIfNotExists("C_M_H_ENF", "Prestación monetaria madres con hijos enfermos", "SEGSOC");
            AddAccountingFunctionDefinitionIfNotExists("C_MAT60%", "Maternidad al 60%", "SEGSOC");
            AddAccountingFunctionDefinitionIfNotExists("C_MATERN", "Maternidad al 100%", "SEGSOC");

            AddAccountingFunctionDefinitionIfNotExists("C_CPOBLA", "Créditos a la Población", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_CRESO", "Créditos Sociales", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_DL91", "Decreto Ley 91", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_DL92", "Decreto Ley 92", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_EJU", "Embargo Judicial", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_FFOND", "Formación de Fondos", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_LGV", "Ley General de la Vivienda", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_MULTA", "Multas", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_PALIM", "Pensión Alimenticia", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_RCIVIL", "Responsabilidad Civil", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_SVIDA", "Seguro de Vida", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_TRA", "Transporte", "RET");
            AddAccountingFunctionDefinitionIfNotExists("C_VVINC", "Vivienda Vinculada", "RET");

            AddAccountingFunctionDefinitionIfNotExists("C_IMP_CSS", "Aporte Especial a la Seguridad Social", "APORTE");
            AddAccountingFunctionDefinitionIfNotExists("C_IMP_ING", "Aporte del Impuesto sobre los Ingresos Personales", "APORTE");
            

            AddAccountingFunctionDefinitionIfNotExists("C_GRADO_SVAC", "Grado Científico sin vacaciones", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_ANIR", "Pago por remuneración ANIR", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_PEFICE", "Pago por Eficiencia Económica", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_CPOBLA", "Devolución de Créditos a la Población", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_CRESO", "Devolución de Créditos Sociales", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_DL91", "Devolución de Decreto Ley 91", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_DL92", "Devolución de Decreto Ley 92", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_EJU", "Devolución de Embargo Judicial", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_FFOND", "Devolución de Formación de Fondos", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_LGV", "Devolución de Ley General de la Vivienda", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_MULTA", "Devolución de Multas", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_PALIM", "Devolución de Pensión Alimenticia", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_RCIVIL", "Devolución de Responsabilidad Civil", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_SVIDA", "Devolución de Seguro de Vida", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_TRA", "Devolución de Transporte", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_VVINC", "Devolución de Vivienda Vinculada", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_CSS", "Devolución de Seguridad Social", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_D_IMP_ING", "Devolución de Impuestos sobre Ingresos Personales", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_DAPORTES", "Devolución de Aportes", "AJUST");
            AddAccountingFunctionDefinitionIfNotExists("C_DS_NREC", "Devolución de Salarios no Reclamados", "AJUST");

            _logger.Info("AccountingFunctionDefinition created.");
        }

        private void CreateViewNames()
        {
            if (_context.Database.IsSqlServer())
            {
                var query = @"SELECT TABLE_CATALOG AS TableCatalog, TABLE_SCHEMA AS TableSchema, TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'cnt' AND TABLE_NAME LIKE 'vw_%'";
                var scanned = _context.Database.SqlQueryRaw<ViewMetadata>(query).ToList();

                foreach (var s in scanned)
                {
                    var compoundName = $"{s.TableCatalog}.{s.TableSchema}.{s.TableName}";
                    if (_context.ViewNames.IgnoreQueryFilters().FirstOrDefault(c => c.Name == compoundName) != null) continue;
                    
                    _context.ViewNames.Add(new ViewName
                    {
                        Name = compoundName,
                        Description = s.TableName.Replace("vw_", "").Replace("_", " ").ToUpperInvariant(),
                        ReferenceGroup = s.TableCatalog != KontecgCoreConsts.DefaultReferenceGroup ? s.TableCatalog : ""
                    });

                    _context.SaveChanges();
                }
            }

            _logger.Info("Loaded views.");
        }

        private void CreateDocumentDefinitions()
        {
            AddDocumentDefinitionIfNotExists(1, "Nómina de Salario", "NSA", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(2, "Nómina de Embargo Judicial", "NEJ", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(3, "Nómina de Vacaciones ", "NVAC", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(4, "Nómina de Pensionada", "NPEN", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(5, "Nómina de Adeudos no Reclamados Trabajadores", "NRECLAMTRB", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(6, "Liquidación de Adeudos no Reclamados Trabajadores", "LIQNRECTRB", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(7, "Liquidación de Nómina (Trabajador)", "LIQNT", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(8, "Liquidación de Nómina (Pensionada)", "LIQNPEN", "SGNOM", null, null);

            AddDocumentDefinitionIfNotExists(10, "Nómina de Ajustes de Devoluciones", "AJUSTDEV", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(11, "Nómina de Salario a Rendimiento", "NSAR", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(12, "Liquidación de Nómina Embargo Judicial", "LIQEJ", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(13, "Nómina de Invalidez Temporal", "NINVTMP", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(14, "Nómina de Maternidad", "NMATERN", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(15, "Nómina de Invalidez Parcial", "NINVPARC", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(16, "Nómina de Estimulación (Pago por Resultados)", "NESTPRES", "SGNOM", "SC-4-06", null);

            AddDocumentDefinitionIfNotExists(17, "Nómina de Transporte", "TRANSPORTE", "SGNOM", "SC-4-06", null);

            AddDocumentDefinitionIfNotExists(20, "Prenómina de Salario (Pago por Resultados)", "PNSAR", "SGNOM", "SC-4-05", null);
            AddDocumentDefinitionIfNotExists(21, "Cheques", "CHEQ", "", null, null);
            AddDocumentDefinitionIfNotExists(22, "Reembolsos", "REEMB", "", null, null);
            AddDocumentDefinitionIfNotExists(23, "Devoluciones Bancarias", "DEVBANC", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(24, "Ajustes al Submayor de Retenciones", "AJSBMAYRET", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(25, "Nómina de Ajustes", "NAJUST", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(26, "Nómina de Pago de remuneración de la ANIR", "NSANIR", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(27, "Nómina de Pago por Eficiencia Económica", "NSPE", "SGNOM", "SC-4-06", null);
            AddDocumentDefinitionIfNotExists(28, "Ajustes al Submayor de Vacaciones", "AJSBMAYVAC", "SGNOM", null, null);

            AddDocumentDefinitionIfNotExists(30, "Documento de Ajuste", "DOCAJUST", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(31, "Plantilla de la Empresa", "DOCTEMPLATE", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(32, "Movimiento de Nómina", "DOCNOM", "SGNOM", "SC-4-02", null);
            AddDocumentDefinitionIfNotExists(33, "Evaluación de Desempeño", "DOCCPL", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(34, "Solicitud de Retención", "DOCRET", "SGNOM", "SC-4-04", null);
            AddDocumentDefinitionIfNotExists(35, "Reporte de Tiempo", "DOCTIME", "SGNOM", "SC-4-05", null);
            AddDocumentDefinitionIfNotExists(36, "Solicitud de Subsidio", "DOCSUB", "SGNOM", "SC-4-04", null);
            AddDocumentDefinitionIfNotExists(37, "Solicitud de Vacaciones", "DOCVAC", "SGNOM", "SC-4-04", null);
            AddDocumentDefinitionIfNotExists(38, "Ajustes al SC-4-08", "AJSC408", "SGNOM", null, null);

            AddDocumentDefinitionIfNotExists(40, "Comprobante", "COMPROBANTE", "", null, null);
            AddDocumentDefinitionIfNotExists(41, "Submayor de Retenciones", "SUBRET", "SGNOM", null, null);
            AddDocumentDefinitionIfNotExists(42, "Submayor de Vacaciones", "SUBVAC", "SGNOM", "SC-4-07", null);
            AddDocumentDefinitionIfNotExists(43, "Registro de Salarios y Tiempo de Servicio", "SC408", "SGNOM", "SC-4-08", null);
            
            AddDocumentDefinitionIfNotExists(50, "Firmas autorizadas", "DOCSIGN", "", null, null);
            _logger.Info("Documents created.");
        }

        private void CreateBanks()
        {
            AddBankIfNotExists("BANMET", "BANCO METROPOLITANO");
            AddBankIfNotExists("BICSA", "BANCO INTERNACIONAL DE COMERCIO S.A.");
            AddBankIfNotExists("BCC", "BANCO CENTRAL DE CUBA");
            AddBankIfNotExists("BPA", "BANCO POPULAR DE AHORROS");
            AddBankIfNotExists("CADECA", "CASAS DE CAMBIO");
            _logger.Info("Banks created.");
        }

        private void CreateBillDenominations()
        {
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 1000M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 500M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 200M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 100M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 50M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 20M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 10M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 5M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 0.20M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 0.05M);
            AddBillDenominationIfNotExists(CurrencyIsoCode.CUP, 0.01M);
            _logger.Info("Bill denominations created.");
        }

        private void AddAccountingClassifierDefinitionIfNotExists(string description)
        {
            var cc = _context.AccountingClassifierDefinitions.IgnoreQueryFilters()
                             .FirstOrDefault(c => c.Description == description);

            if (cc != null) return;
            _context.AccountingClassifierDefinitions.Add(new AccountingClassifierDefinition(description));
            _context.SaveChanges();
        }


        private void AddAccountDefinitionIfNotExists(int account, int subAccount, int subControl, int analysis, string description, AccountKind kind, string reference = null)
        {
            var referenceToSetUp = reference.IsNullOrWhiteSpace() ? $"CTA_{account}_{subAccount}" : reference;

            var cc = _context.AccountDefinitions.IgnoreQueryFilters()
                .FirstOrDefault(c => c.Account == account && c.SubAccount == subAccount && c.SubControl == subControl && c.Analysis == analysis && c.Kind == kind && c.Reference == referenceToSetUp && c.IsActive);

            if (cc != null) return;
            _context.AccountDefinitions.Add(new AccountDefinition(account, subAccount, subControl, analysis, description, kind, referenceToSetUp));
            _context.SaveChanges();
        }

        private void AddAccountingFunctionDefinitionIfNotExists(string name, string description, string reference)
        {
            var script = _context.AccountingFunctionDefinitionStorage.IgnoreQueryFilters()
                                 .FirstOrDefault(c =>
                                     c.Description == "FUNCIÓN CONTABLE VACÍA");

            if (script == null)
            {
                _context.AccountingFunctionDefinitionStorage.Add(new AccountingFunctionDefinitionStorage("FUNCIÓN CONTABLE VACÍA", "<FCs><FC Nombre=\"EMPTY\" /></FCs>"));
                _context.SaveChanges();
            }

            var cc = _context.AccountingFunctionDefinitions.IgnoreQueryFilters()
                             .FirstOrDefault(c =>
                                 c.Name == name.ToUpperInvariant() && c.Reference == reference.ToUpperInvariant() &&
                                 c.Description == description.ToUpperInvariant());

            if (cc != null) return;
            _context.AccountingFunctionDefinitions.Add(new AccountingFunctionDefinition(name, description, reference) {StorageId = 1});
            _context.SaveChanges();
        }


        private void AddWorkPlaceClassificationIfNotExists(string description, int nivel = 1)
        {
            if (_context.WorkPlaceClassifications.IgnoreQueryFilters().Any(s => s.Description == description && s.Level == nivel))
                return;

            _context.WorkPlaceClassifications.Add(new WorkPlaceClassification(description, nivel));
            _context.SaveChanges();
        }

        private void AddDocumentDefinitionIfNotExists(int code, string description, string reference, string referenceGroup, [CanBeNull] string legal, [CanBeNull] string legalTypeAssemblyQualifiedName)
        {
            var ac = _context.DocumentDefinitions.IgnoreQueryFilters()
                .FirstOrDefault(c => c.Code == code && c.Reference == reference && c.ReferenceGroup == referenceGroup);

            if (ac != null) return;

            _context.DocumentDefinitions.Add(new DocumentDefinition(code, description, reference, referenceGroup)
            {
                Legal = legal,
                LegalTypeAssemblyQualifiedName = legalTypeAssemblyQualifiedName
            });
            _context.SaveChanges();
        }

        private void AddBankIfNotExists(string name, string description)
        {
            if (_context.Banks.IgnoreQueryFilters()
                    .FirstOrDefault(e => e.Name == name) != null)
                return;
            _context.Banks.Add(new Bank(name, description));
            _context.SaveChanges();
        }

        private void AddExchangeRateIfNotExists(int bankId, CurrencyIsoCode from, CurrencyIsoCode to, decimal rate, DateTime since, DateTime until, ScopeData scope = ScopeData.Company)
        {
            var exchange = _context.ExchangeRates.IgnoreQueryFilters().OrderByDescending(o => o.Until)
                              .FirstOrDefault(e => e.From == from && e.To == to && e.BankId == bankId);

            if (exchange != null)
            {
                exchange.Until = DateTime.Today.AddTicks(-1);
                _context.ExchangeRates.Update(exchange);
                _context.SaveChanges();
            }

            _context.ExchangeRates.Add(new ExchangeRateInfo(bankId, from, to, rate, since, until, scope));
            _context.SaveChanges();
        }

        private void AddBillDenominationIfNotExists(CurrencyIsoCode currency, decimal bill, bool isActive = true)
        {
            if (_context.BillDenominations.IgnoreQueryFilters()
                    .FirstOrDefault(e => e.Currency == currency && e.Bill == bill) != null)
                return;

            _context.BillDenominations.Add(new BillDenomination(currency, bill, isActive));
            _context.SaveChanges();
        }

        private void AddDynamicPropertyValueIfNotExists(int companyId, DynamicProperty property, string value)
        {
            if (_context.DynamicPropertyValues.IgnoreQueryFilters().Any(p => p.DynamicPropertyId == property.Id && string.Compare(p.Value, value) == 0 && p.CompanyId == companyId))
                return;

            var dynamicPropertyValue = new DynamicPropertyValue(property, value, companyId);
            _context.DynamicPropertyValues.Add(dynamicPropertyValue);
            _context.SaveChanges();
        }

        private class ViewMetadata
        {
            public string TableCatalog { get; set; }
            public string TableSchema { get; set; }
            public string TableName { get; set; }
        }
    }
}
