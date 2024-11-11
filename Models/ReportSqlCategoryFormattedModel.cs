
namespace ZC_Informes.Models
{

    // =============== Modelo de categoria
    public class ReportSqlCategoryFormattedModel
    {

        public int Id { get; set; } = 0;
        public string Nombre { get; set; } = string.Empty;
        public int VisibleIndividual { get; set; } = 0;
        public int VisibleFechas { get; set; } = 0;
        public int VisibleHojaProduccion { get; set; } = 0;
        public string? Dependencias { get; set; } = string.Empty;
        public string? ConfigBool { get; set; } = string.Empty;
        public List<int> DependenciasItems => ParseStringToIntList(Dependencias);
        public List<int> ConfigBoolItems => ParseStringToIntList(ConfigBool, 32);



        // =============== Constructor
        public ReportSqlCategoryFormattedModel()
        {

        }



        // =============== Metodo para parsear los items en lista
        private List<int> ParseStringToIntList(string? input, int? expectedCount = null)
        {
            var parsedList = string.IsNullOrWhiteSpace(input)
                ? new List<int>()
                : input.Split(';')
                       .Where(x => int.TryParse(x, out _))
                       .Select(int.Parse)
                       .ToList();

            if (expectedCount.HasValue && parsedList.Count < expectedCount.Value)
            {
                parsedList.AddRange(Enumerable.Repeat(0, expectedCount.Value - parsedList.Count));
            }

            return parsedList;
        }

    }

}
