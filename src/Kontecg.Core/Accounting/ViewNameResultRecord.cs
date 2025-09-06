namespace Kontecg.Accounting
{
    public class ViewNameResultRecord
    {
        public string Trans { get; set; }
        public string DocType { get; set; }
        public string DocCod { get; set; }
        public int? Cc { get; set; }
        public int? Cnt { get; set; }
        public int? Exp { get; set; }
        public string Tp { get; set; }
        public string Fc { get; set; }
        public bool? Contrato { get; set; }
        public bool? Jornalero { get; set; }
        public bool? Caja { get; set; }
        public string Currency { get; set; }
        public decimal Imp { get; set; }
        public decimal? ImpVac { get; set; }
        public decimal? AporteSegSoc { get; set; }
        public decimal? AporteFuerzaLaboral { get; set; }
        public decimal? ProvisionSegSoc { get; set; }

        public ViewNameResultRecord(
            string trans,
            string docType,
            string docCod,
            int cc,
            int cnt,
            int exp,
            string tp,
            string fc,
            bool contrato,
            bool jornalero,
            bool caja,
            string currency,
            decimal imp,
            decimal impVac,
            decimal aporteSegSoc,
            decimal aporteFuerzaLaboral,
            decimal provisionSegSoc)
        {
            Trans = trans;
            DocType = docType;
            DocCod = docCod;
            Cc = cc;
            Cnt = cnt;
            Exp = exp;
            Tp = tp;
            Fc = fc;
            Contrato = contrato;
            Jornalero = jornalero;
            Caja = caja;
            Currency = currency;
            Imp = imp;
            ImpVac = impVac;
            AporteSegSoc = aporteSegSoc;
            AporteFuerzaLaboral = aporteFuerzaLaboral;
            ProvisionSegSoc = provisionSegSoc;
        }

        public ViewNameResultRecord(string trans, string docType, string docCod, string fc, string currency, decimal imp)
        {
            Trans = trans;
            DocType = docType;
            DocCod = docCod;
            Fc = fc;
            Currency = currency;
            Imp = imp;
        }

        // Constructor sin parámetros para Bogus
        public ViewNameResultRecord() { }
    }
}