using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZC_Informes.Models
{

    public class ReportSqlCategoryModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ruta_Configuracion { get; set; } = string.Empty;
    }


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



        public string[] Strings { get; set; } = new string[10]; // Para String_1 a String_10
        public string[] Bools { get; set; } = new string[32]; // Para Bool_1 a Bool_32
        public float[] Reales { get; set; } = new float[20]; // Para Real_1 a Real_20
        public int[] Enteros { get; set; } = new int[20]; // Para Int_1 a Int_20       


    }


    public class ReportSqlReportList
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
    }

}
