
namespace ZC_Informes.Models
{

    // =============== Modelo de configuracion general
    public class ReportConfigGeneralModel
    {
        public bool Enable { get; set; } = false;
        public string? PageSize { get; set; } = "A4";
        public bool IsHorizontal { get; set; } = false;
        public string? FontFamily { get; set; } = "Times New Roman";
        public string? HeaderImage1 { get; set; } = string.Empty;
        public string? HeaderImage2 { get; set; } = string.Empty;
        public string? HeaderText1 { get; set; } = string.Empty;
        public string? HeaderText2 { get; set; } = string.Empty;
    }

}