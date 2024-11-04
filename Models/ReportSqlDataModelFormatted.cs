

namespace ZC_Informes.Models
{
    public class ReportSqlDataModelFormatted
    {
        public string Id { get; set; } = string.Empty;
        public int Tipo { get; set; } = 0;
        public string Codigo { get; set; } = string.Empty;        

        // Fechas como string
        public string Fecha_1 => Fecha_1_Date.ToString("yyyy-MM-dd");
        public string Fecha_2 => Fecha_2_Date.ToString("yyyy-MM-dd");
        public string Fecha_3 => Fecha_3_Date.ToString("yyyy-MM-dd");
        public string Fecha_4 => Fecha_4_Date.ToString("yyyy-MM-dd");

        // Fechas internas como DateTime para inicialización
        public DateTime Fecha_1_Date { get; set; }
        public DateTime Fecha_2_Date { get; set; }
        public DateTime Fecha_3_Date { get; set; }
        public DateTime Fecha_4_Date { get; set; }

        // Horas como string
        public string Hora_1 => $"{Hora_1_Time.Hours:D2}:{Hora_1_Time.Minutes:D2}:{Hora_1_Time.Seconds:D2}";
        public string Hora_2 => $"{Hora_2_Time.Hours:D2}:{Hora_2_Time.Minutes:D2}:{Hora_1_Time.Seconds:D2}";
        public string Hora_3 => $"{Hora_3_Time.Hours:D2}:{Hora_3_Time.Minutes:D2}:{Hora_1_Time.Seconds:D2}";
        public string Hora_4 => $"{Hora_4_Time.Hours:D2}:{Hora_4_Time.Minutes:D2}:{Hora_1_Time.Seconds:D2}";

        // Horas internas como TimeSpan para inicialización
        public TimeSpan Hora_1_Time { get; set; }
        public TimeSpan Hora_2_Time { get; set; }
        public TimeSpan Hora_3_Time { get; set; }
        public TimeSpan Hora_4_Time { get; set; }

        public string String_1 { get; set; } = "";
        public string String_2 { get; set; } = "";
        public string String_3 { get; set; } = "";
        public string String_4 { get; set; } = "";
        public string String_5 { get; set; } = "";
        public string String_6 { get; set; } = "";
        public string String_7 { get; set; } = "";
        public string String_8 { get; set; } = "";
        public string String_9 { get; set; } = "";
        public string String_10 { get; set; } = "";

        // Booleanos como string
        public string Bool_1 => Bool_1_Value ? "true" : "false";
        public string Bool_2 => Bool_2_Value ? "true" : "false";
        public string Bool_3 => Bool_3_Value ? "true" : "false";
        public string Bool_4 => Bool_4_Value ? "true" : "false";
        public string Bool_5 => Bool_5_Value ? "true" : "false";
        public string Bool_6 => Bool_6_Value ? "true" : "false";
        public string Bool_7 => Bool_7_Value ? "true" : "false";
        public string Bool_8 => Bool_8_Value ? "true" : "false";
        public string Bool_9 => Bool_9_Value ? "true" : "false";
        public string Bool_10 => Bool_10_Value ? "true" : "false";
        public string Bool_11 => Bool_11_Value ? "true" : "false";
        public string Bool_12 => Bool_12_Value ? "true" : "false";
        public string Bool_13 => Bool_13_Value ? "true" : "false";
        public string Bool_14 => Bool_14_Value ? "true" : "false";
        public string Bool_15 => Bool_15_Value ? "true" : "false";
        public string Bool_16 => Bool_16_Value ? "true" : "false";
        public string Bool_17 => Bool_17_Value ? "true" : "false";
        public string Bool_18 => Bool_18_Value ? "true" : "false";
        public string Bool_19 => Bool_19_Value ? "true" : "false";
        public string Bool_20 => Bool_20_Value ? "true" : "false";
        public string Bool_21 => Bool_21_Value ? "true" : "false";
        public string Bool_22 => Bool_22_Value ? "true" : "false";
        public string Bool_23 => Bool_23_Value ? "true" : "false";
        public string Bool_24 => Bool_24_Value ? "true" : "false";
        public string Bool_25 => Bool_25_Value ? "true" : "false";
        public string Bool_26 => Bool_26_Value ? "true" : "false";
        public string Bool_27 => Bool_27_Value ? "true" : "false";
        public string Bool_28 => Bool_28_Value ? "true" : "false";
        public string Bool_29 => Bool_29_Value ? "true" : "false";
        public string Bool_30 => Bool_30_Value ? "true" : "false";
        public string Bool_31 => Bool_31_Value ? "true" : "false";
        public string Bool_32 => Bool_32_Value ? "true" : "false";

        // Valores internos
        public bool Bool_1_Value { get; set; }
        public bool Bool_2_Value { get; set; }
        public bool Bool_3_Value { get; set; }
        public bool Bool_4_Value { get; set; }
        public bool Bool_5_Value { get; set; }
        public bool Bool_6_Value { get; set; }
        public bool Bool_7_Value { get; set; }
        public bool Bool_8_Value { get; set; }
        public bool Bool_9_Value { get; set; }
        public bool Bool_10_Value { get; set; }
        public bool Bool_11_Value { get; set; }
        public bool Bool_12_Value { get; set; }
        public bool Bool_13_Value { get; set; }
        public bool Bool_14_Value { get; set; }
        public bool Bool_15_Value { get; set; }
        public bool Bool_16_Value { get; set; }
        public bool Bool_17_Value { get; set; }
        public bool Bool_18_Value { get; set; }
        public bool Bool_19_Value { get; set; }
        public bool Bool_20_Value { get; set; }
        public bool Bool_21_Value { get; set; }
        public bool Bool_22_Value { get; set; }
        public bool Bool_23_Value { get; set; }
        public bool Bool_24_Value { get; set; }
        public bool Bool_25_Value { get; set; }
        public bool Bool_26_Value { get; set; }
        public bool Bool_27_Value { get; set; }
        public bool Bool_28_Value { get; set; }
        public bool Bool_29_Value { get; set; }
        public bool Bool_30_Value { get; set; }
        public bool Bool_31_Value { get; set; }
        public bool Bool_32_Value { get; set; }

        // Valores reales formateados
        public string Real_1 => Real_1_Value.ToString("F2");
        public string Real_2 => Real_2_Value.ToString("F2");
        public string Real_3 => Real_3_Value.ToString("F2");
        public string Real_4 => Real_4_Value.ToString("F2");
        public string Real_5 => Real_5_Value.ToString("F2");
        public string Real_6 => Real_6_Value.ToString("F2");
        public string Real_7 => Real_7_Value.ToString("F2");
        public string Real_8 => Real_8_Value.ToString("F2");
        public string Real_9 => Real_9_Value.ToString("F2");
        public string Real_10 => Real_10_Value.ToString("F2");
        public string Real_11 => Real_11_Value.ToString("F2");
        public string Real_12 => Real_12_Value.ToString("F2");
        public string Real_13 => Real_13_Value.ToString("F2");
        public string Real_14 => Real_14_Value.ToString("F2");
        public string Real_15 => Real_15_Value.ToString("F2");
        public string Real_16 => Real_16_Value.ToString("F2");
        public string Real_17 => Real_17_Value.ToString("F2");
        public string Real_18 => Real_18_Value.ToString("F2");
        public string Real_19 => Real_19_Value.ToString("F2");
        public string Real_20 => Real_20_Value.ToString("F2");

        // Valores internos
        public double Real_1_Value { get; set; }
        public double Real_2_Value { get; set; }
        public double Real_3_Value { get; set; }
        public double Real_4_Value { get; set; }
        public double Real_5_Value { get; set; }
        public double Real_6_Value { get; set; }
        public double Real_7_Value { get; set; }
        public double Real_8_Value { get; set; }
        public double Real_9_Value { get; set; }
        public double Real_10_Value { get; set; }
        public double Real_11_Value { get; set; }
        public double Real_12_Value { get; set; }
        public double Real_13_Value { get; set; }
        public double Real_14_Value { get; set; }
        public double Real_15_Value { get; set; }
        public double Real_16_Value { get; set; }
        public double Real_17_Value { get; set; }
        public double Real_18_Value { get; set; }
        public double Real_19_Value { get; set; }
        public double Real_20_Value { get; set; }

        // Valores enteros con signo solo si son negativos
        public string Int_1 => Int_1_Value < 0 ? Int_1_Value.ToString() : Int_1_Value.ToString("D");
        public string Int_2 => Int_2_Value < 0 ? Int_2_Value.ToString() : Int_2_Value.ToString("D");
        public string Int_3 => Int_3_Value < 0 ? Int_3_Value.ToString() : Int_3_Value.ToString("D");
        public string Int_4 => Int_4_Value < 0 ? Int_4_Value.ToString() : Int_4_Value.ToString("D");
        public string Int_5 => Int_5_Value < 0 ? Int_5_Value.ToString() : Int_5_Value.ToString("D");
        public string Int_6 => Int_6_Value < 0 ? Int_6_Value.ToString() : Int_6_Value.ToString("D");
        public string Int_7 => Int_7_Value < 0 ? Int_7_Value.ToString() : Int_7_Value.ToString("D");
        public string Int_8 => Int_8_Value < 0 ? Int_8_Value.ToString() : Int_8_Value.ToString("D");
        public string Int_9 => Int_9_Value < 0 ? Int_9_Value.ToString() : Int_9_Value.ToString("D");
        public string Int_10 => Int_10_Value < 0 ? Int_10_Value.ToString() : Int_10_Value.ToString("D");
        public string Int_11 => Int_11_Value < 0 ? Int_11_Value.ToString() : Int_11_Value.ToString("D");
        public string Int_12 => Int_12_Value < 0 ? Int_12_Value.ToString() : Int_12_Value.ToString("D");
        public string Int_13 => Int_13_Value < 0 ? Int_13_Value.ToString() : Int_13_Value.ToString("D");
        public string Int_14 => Int_14_Value < 0 ? Int_14_Value.ToString() : Int_14_Value.ToString("D");
        public string Int_15 => Int_15_Value < 0 ? Int_15_Value.ToString() : Int_15_Value.ToString("D");
        public string Int_16 => Int_16_Value < 0 ? Int_16_Value.ToString() : Int_16_Value.ToString("D");
        public string Int_17 => Int_17_Value < 0 ? Int_17_Value.ToString() : Int_17_Value.ToString("D");
        public string Int_18 => Int_18_Value < 0 ? Int_18_Value.ToString() : Int_18_Value.ToString("D");
        public string Int_19 => Int_19_Value < 0 ? Int_19_Value.ToString() : Int_19_Value.ToString("D");
        public string Int_20 => Int_20_Value < 0 ? Int_20_Value.ToString() : Int_20_Value.ToString("D");

        // Valores internos
        public int Int_1_Value { get; set; }
        public int Int_2_Value { get; set; }
        public int Int_3_Value { get; set; }
        public int Int_4_Value { get; set; }
        public int Int_5_Value { get; set; }
        public int Int_6_Value { get; set; }
        public int Int_7_Value { get; set; }
        public int Int_8_Value { get; set; }
        public int Int_9_Value { get; set; }
        public int Int_10_Value { get; set; }
        public int Int_11_Value { get; set; }
        public int Int_12_Value { get; set; }
        public int Int_13_Value { get; set; }
        public int Int_14_Value { get; set; }
        public int Int_15_Value { get; set; }
        public int Int_16_Value { get; set; }
        public int Int_17_Value { get; set; }
        public int Int_18_Value { get; set; }
        public int Int_19_Value { get; set; }
        public int Int_20_Value { get; set; }

        



    }
}
