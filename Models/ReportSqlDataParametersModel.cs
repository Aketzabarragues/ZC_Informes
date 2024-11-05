
namespace ZC_Informes.Models
{    
    // =============== Modelo de parametros para pasar a los informes
    public class ReportSqlDataParametersModel
    {
        public List<int>? Tipo { get; set; }
        public string? Codigo { get; set; }
    }
}
