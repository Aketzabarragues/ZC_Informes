
namespace ZC_Informes.Models
{

    // =============== Modelo de configuracion de cada header/data de la tabla
    public class ReportConfigTableDataModel
    {
        public bool Enable { get; set; } = false;
        public string? BackgroundColor { get; set; } = string.Empty;
        public float Border { get; set; } = 0;
        public string? BorderColor { get; set; } = string.Empty;
        public int FontSize { get; set; } = 0;
        public string? FontStyle { get; set; } = string.Empty;
        public List<string>? FontStyleItems { get; set; } = new List<string>();
        public string? FontColor { get; set; } = string.Empty;
        public string? CombineColumn { get; set; } = string.Empty;
        public List<int>? CombineColumnItems { get; set; } = new List<int>();
        public string? DataType { get; set; } = string.Empty;
        public List<int>? DataTypeItems { get; set; } = new List<int>();
        public string? DataSource { get; set; } = string.Empty;
        public List<string>? DataSourceItems { get; set; } = new List<string>();
        public string? DataUnits { get; set; } = string.Empty;
        public List<string>? DataUnitsItems { get; set; } = new List<string>();
    }

}