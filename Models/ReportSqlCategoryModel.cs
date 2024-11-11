
namespace ZC_Informes.Models
{

    // =============== Modelo de categoria
    public class ReportSqlCategoryModel
    {

        public int Id { get; set; } = 0;
        public string Nombre { get; set; } = string.Empty;
        public int VisibleIndividual { get; set; } = 0;
        public int VisibleFechas { get; set; } = 0;
        public int VisibleHojaProduccion { get; set; } = 0;
        public string? ConfigBool { get; set; } = string.Empty;

    }
    
}
