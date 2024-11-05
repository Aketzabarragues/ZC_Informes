
namespace ZC_Informes.Models
{
    
    // =============== Modelo de configuracion general de cada tabla
    public class ReportConfigTableGeneralModel
    {
        public bool Enable { get; set; } = false;
        public int TittlePaddingTop { get; set; } = 0;
        public int TablePaddingTop { get; set; } = 0;        
        public int Columns { get; set; } = 0;
        public string? ColumnsSize { get; set; } = string.Empty;
        public List<int>? ColumnsSizeItems { get; set; } = new List<int>();
        public int TableType { get; set; } = 0;
        public int Rows { get; set; } = 0;
        public int FixedColumnSize { get; set; } = 100;        
        public string? HeaderCategory { get; set; } = string.Empty;
        public List<int>? HeaderCategoryItems { get; set; } = new List<int>();
        public string? DataCategory { get; set; } = string.Empty;
        public List<int>? DataCategoryItems { get; set; } = new List<int>();
    } 

}