
using Serilog;

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
        public string? Config_Bool { get; set; } = string.Empty;
        public List<int> ConfigBoolItems => ParseStringToIntList(Config_Bool, 32);

        // =============== Metodo para parsear los items en lista
        private List<int> ParseStringToIntList(string? input, int? expectedCount = null)
        {
            // Inicializa la lista resultante como vacía
            List<int> parsedList = new List<int>();

            // Verifica que la cadena no esté vacía o en blanco
            if (!string.IsNullOrWhiteSpace(input))
            {
                // Divide la cadena y convierte cada parte en entero
                parsedList = input.Split(';')
                                  .Select(part =>
                                  {
                                      // Intenta convertir cada parte a un entero; si falla, asigna 0
                                      return int.TryParse(part, out int result) ? result : 0;
                                  })
                                  .ToList();
            }

            // Rellena con ceros si la lista es menor al tamaño esperado
            if (expectedCount.HasValue && parsedList.Count < expectedCount.Value)
            {
                parsedList.AddRange(Enumerable.Repeat(0, expectedCount.Value - parsedList.Count));
            }

            Log.Information("Parsed list: {parsedList}", parsedList);
            return parsedList;
        }

    }

}
