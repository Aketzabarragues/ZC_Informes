
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
        public string? Dependencias { get; set; } = string.Empty;
        public string? ConfigBool { get; set; } = string.Empty;
        public List<int> DependenciasItems
        {
            get
            {
                return string.IsNullOrWhiteSpace(Dependencias)
                    ? new List<int>()
                    : Dependencias.Split(';')
                                  .Where(x => int.TryParse(x, out _))
                                  .Select(int.Parse)
                                  .ToList();
            }
        }
        public List<int> ConfigBoolItems
        {
            get
            {
                return string.IsNullOrWhiteSpace(ConfigBool)
                    ? new List<int>()
                    : ConfigBool.Split(';')
                                  .Where(x => int.TryParse(x, out _))
                                  .Select(int.Parse)
                                  .ToList();
            }
        }
    }
    
}
