
namespace ZC_Informes.Models
{

    // =============== Modelo de categoria
    public class ReportSqlCategoryModel
    {
        public int Id { get; set; } = 0;
        public string Nombre { get; set; } = string.Empty;
        public int Visible { get; set; } = 0;
        public string? Dependencias { get; set; } = string.Empty;

        // Propiedad calculada que convierte la cadena Dependencias en una lista de enteros
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


        public string? ConfigBool { get; set; } = string.Empty;
    }



    // =============== Modelo da datos de informe (tiene que coincidir con la tabla de datos de informe de SQL
    public class ReportSqlDataModel
    {
        public string Id { get; set; } = string.Empty;
        public int Tipo { get; set; } = 0;
        public string Codigo { get; set; } = string.Empty;

        public DateTime Fecha_1 { get; set; } = DateTime.Now;
        public TimeSpan Hora_1 { get; set; } = TimeSpan.Zero;
        public DateTime Fecha_2 { get; set; } = DateTime.Now;
        public TimeSpan Hora_2 { get; set; } = TimeSpan.Zero;
        public DateTime Fecha_3 { get; set; } = DateTime.Now;
        public TimeSpan Hora_3 { get; set; } = TimeSpan.Zero;
        public DateTime Fecha_4 { get; set; } = DateTime.Now;
        public TimeSpan Hora_4 { get; set; } = TimeSpan.Zero;

        public string String_1 { get; set; } = string.Empty;
        public string String_2 { get; set; } = string.Empty;
        public string String_3 { get; set; } = string.Empty;
        public string String_4 { get; set; } = string.Empty;
        public string String_5 { get; set; } = string.Empty;
        public string String_6 { get; set; } = string.Empty;
        public string String_7 { get; set; } = string.Empty;
        public string String_8 { get; set; } = string.Empty;
        public string String_9 { get; set; } = string.Empty;
        public string String_10 { get; set; } = string.Empty;

        public bool Bool_1 { get; set; } = false;
        public bool Bool_2 { get; set; } = false;
        public bool Bool_3 { get; set; } = false;
        public bool Bool_4 { get; set; } = false;
        public bool Bool_5 { get; set; } = false;
        public bool Bool_6 { get; set; } = false;
        public bool Bool_7 { get; set; } = false;
        public bool Bool_8 { get; set; } = false;
        public bool Bool_9 { get; set; } = false;
        public bool Bool_10 { get; set; } = false;
        public bool Bool_11 { get; set; } = false;
        public bool Bool_12 { get; set; } = false;
        public bool Bool_13 { get; set; } = false;
        public bool Bool_14 { get; set; } = false;
        public bool Bool_15 { get; set; } = false;
        public bool Bool_16 { get; set; } = false;
        public bool Bool_17 { get; set; } = false;
        public bool Bool_18 { get; set; } = false;
        public bool Bool_19 { get; set; } = false;
        public bool Bool_20 { get; set; } = false;
        public bool Bool_21 { get; set; } = false;
        public bool Bool_22 { get; set; } = false;
        public bool Bool_23 { get; set; } = false;
        public bool Bool_24 { get; set; } = false;
        public bool Bool_25 { get; set; } = false;
        public bool Bool_26 { get; set; } = false;
        public bool Bool_27 { get; set; } = false;
        public bool Bool_28 { get; set; } = false;
        public bool Bool_29 { get; set; } = false;
        public bool Bool_30 { get; set; } = false;
        public bool Bool_31 { get; set; } = false;
        public bool Bool_32 { get; set; } = false;

        public float Real_1 { get; set; } = 0;
        public float Real_2 { get; set; } = 0;
        public float Real_3 { get; set; } = 0;
        public float Real_4 { get; set; } = 0;
        public float Real_5 { get; set; } = 0;
        public float Real_6 { get; set; } = 0;
        public float Real_7 { get; set; } = 0;
        public float Real_8 { get; set; } = 0;
        public float Real_9 { get; set; } = 0;
        public float Real_10 { get; set; } = 0;
        public float Real_11 { get; set; } = 0;
        public float Real_12 { get; set; } = 0;
        public float Real_13 { get; set; } = 0;
        public float Real_14 { get; set; } = 0;
        public float Real_15 { get; set; } = 0;
        public float Real_16 { get; set; } = 0;
        public float Real_17 { get; set; } = 0;
        public float Real_18 { get; set; } = 0;
        public float Real_19 { get; set; } = 0;
        public float Real_20 { get; set; } = 0;

        public int Int_1 { get; set; } = 0;
        public int Int_2 { get; set; } = 0;
        public int Int_3 { get; set; } = 0;
        public int Int_4 { get; set; } = 0;
        public int Int_5 { get; set; } = 0;
        public int Int_6 { get; set; } = 0;
        public int Int_7 { get; set; } = 0;
        public int Int_8 { get; set; } = 0;
        public int Int_9 { get; set; } = 0;
        public int Int_10 { get; set; } = 0;
        public int Int_11 { get; set; } = 0;
        public int Int_12 { get; set; } = 0;
        public int Int_13 { get; set; } = 0;
        public int Int_14 { get; set; } = 0;
        public int Int_15 { get; set; } = 0;
        public int Int_16 { get; set; } = 0;
        public int Int_17 { get; set; } = 0;
        public int Int_18 { get; set; } = 0;
        public int Int_19 { get; set; } = 0;
        public int Int_20 { get; set; } = 0;
    }



    // =============== Modelo de lista de informes filtrados para mostrar en combobox
    public class ReportSqlReportList
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
    }


    // =============== Modelo de parametros para pasar a los informes
    public class ReportSqlDataParameters
    {
        public List<int>? Tipo { get; set; }
        public string? Codigo { get; set; }
    }
}
