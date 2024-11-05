
namespace ZC_Informes.Models
{

    // =============== CONFIGURACION GENERAL DEL INFORME, IGUAL QUE EL JSON
    public class ReportConfigFullModel
    {
        public ReportConfigGeneralModel? GeneralConfiguration { get; set; }
        public ReportConfigTableModel? Table1 { get; set; }
        public ReportConfigTableModel? Table2 { get; set; }
        public ReportConfigTableModel? Table3 { get; set; }
        public ReportConfigTableModel? Table4 { get; set; }
        public ReportConfigTableModel? Table5 { get; set; }
    }

}