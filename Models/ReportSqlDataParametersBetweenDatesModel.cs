
namespace ZC_Informes.Models
{

    // =============== Modelo de parametros para pasar a los informes
    public class ReportSqlDataParametersBetweenDatesModel
    {
        public List<int>? Tipo { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }


}
