
namespace ZC_Informes.Models
{

    // =============== Modelo de categoria
    public class ReportSqlCategoryModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ruta_Configuracion { get; set; } = string.Empty;
    }



    // =============== Modelo da datos de informe (tiene que coincidir con la tabla de datos de informe de SQL
    public class ReportSqlDataModel
    {
        public string Id { get; set; } = string.Empty;
        public int Id_Categoria { get; set; }
        public string Codigo { get; set; } = string.Empty;

        public DateTime Fecha_1 { get; set; }
        public TimeSpan Hora_1 { get; set; }
        public DateTime Fecha_2 { get; set; }
        public TimeSpan Hora_2 { get; set; }
        public DateTime Fecha_3 { get; set; }
        public TimeSpan Hora_3 { get; set; }
        public DateTime Fecha_4 { get; set; }
        public TimeSpan Hora_4 { get; set; }

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
        public float Real10 { get; set; } = 0;
        public float Real11 { get; set; } = 0;
        public float Real12 { get; set; } = 0;
        public float Real13 { get; set; } = 0;
        public float Real14 { get; set; } = 0;
        public float Real15 { get; set; } = 0;
        public float Real16 { get; set; } = 0;
        public float Real17 { get; set; } = 0;
        public float Real18 { get; set; } = 0;
        public float Real19 { get; set; } = 0;
        public float Real20 { get; set; } = 0;

        public float Int_1 { get; set; } = 0;
        public float Int_2 { get; set; } = 0;
        public float Int_3 { get; set; } = 0;
        public float Int_4 { get; set; } = 0;
        public float Int_5 { get; set; } = 0;
        public float Int_6 { get; set; } = 0;
        public float Int_7 { get; set; } = 0;
        public float Int_8 { get; set; } = 0;
        public float Int_9 { get; set; } = 0;
        public float Int_10 { get; set; } = 0;
        public float Int_11 { get; set; } = 0;
        public float Int_12 { get; set; } = 0;
        public float Int_13 { get; set; } = 0;
        public float Int_14 { get; set; } = 0;
        public float Int_15 { get; set; } = 0;
        public float Int_16 { get; set; } = 0;
        public float Int_17 { get; set; } = 0;
        public float Int_18 { get; set; } = 0;
        public float Int_19 { get; set; } = 0;
        public float Int_20 { get; set; } = 0;
    }



    // =============== Modelo de lista de informes filtrados para mostrar en combobox
    public class ReportSqlReportList
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
    }

}
