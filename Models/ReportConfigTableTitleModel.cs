
namespace ZC_Informes.Models
{

    // =============== Modelo de configuracion de titulo de la tabla
    public class ReportConfigTableTitleModel
    {
        public bool Enable { get; set; } = false;
        public string? Description { get; set; } = string.Empty;
        public string? BackgroundColor { get; set; } = string.Empty;
        public float Border { get; set; } = 0;
        public string? BorderColor { get; set; } = string.Empty;
        public int FontSize { get; set; } = 0;
        public string? FontStyle { get; set; } = string.Empty;
        public string? FontColor { get; set; } = string.Empty;
              
    }

}