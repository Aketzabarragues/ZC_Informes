
namespace ZC_Informes.Models
{

    // =============== Modelo de configuracion de tabla completa
    public class ReportConfigTableModel
    {
        public ReportConfigTableGeneralModel? Configuration { get; set; }
        public ReportConfigTableTitleModel? Title { get; set; }
        public ReportConfigTableDataModel? Header { get; set; }
        public ReportConfigTableDataModel? SubHeader1 { get; set; }
        public ReportConfigTableDataModel? SubHeader2 { get; set; }
        public ReportConfigTableDataModel? SubHeader3 { get; set; }
        public ReportConfigTableDataModel? Data { get; set; }
    }
    
}