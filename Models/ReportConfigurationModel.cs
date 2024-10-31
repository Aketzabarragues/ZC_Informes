using CommunityToolkit.Mvvm.ComponentModel;


namespace ZC_Informes.Models
{


    // =============== CONFIGURACION GENERAL DEL INFORME, IGUAL QUE EL JSON
    public class ReportConfigurationModel
    {
        public GeneralConfiguration? GeneralConfiguration { get; set; }
        public TableConfiguration? Table1 { get; set; }
        public TableConfiguration? Table2 { get; set; }
        public TableConfiguration? Table3 { get; set; }
        public TableConfiguration? Table4 { get; set; }
        public TableConfiguration? Table5 { get; set; }
    }



    // =============== Modelo de configuracion general
    public class GeneralConfiguration
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



    // =============== Modelo de configuracion de tabla completa
    public class TableConfiguration
    {
        public TableGeneralConfig? Configuration { get; set; }
        public TableTitleConfig? Title { get; set; }
        public TableDataConfiguration? Header { get; set; }
        public TableDataConfiguration? SubHeader1 { get; set; }
        public TableDataConfiguration? SubHeader2 { get; set; }
        public TableDataConfiguration? SubHeader3 { get; set; }
        public TableDataConfiguration? Data { get; set; }
    }



    // =============== Modelo de configuracion general de cada tabla
    public class TableGeneralConfig
    {
        public bool Enable { get; set; } = false;
        public int TittlePaddingTop { get; set; } = 0;
        public int TablePaddingTop { get; set; } = 0;        
        public int Columns { get; set; } = 0;
        public string? ColumnsSize { get; set; } = string.Empty;
        public List<int>? ColumnsSizeItems { get; set; } = new List<int>();
        public int TableType { get; set; } = 0;
        public int TableRow { get; set; } = 0;
        public string? HeaderCategory { get; set; } = string.Empty;
        public List<int>? HeaderCategoryItems { get; set; } = new List<int>();
        public string? DataCategory { get; set; } = string.Empty;
        public List<int>? DataCategoryItems { get; set; } = new List<int>();
    }


    // =============== Modelo de configuracion de titulo de la tabla
    public class TableTitleConfig
    {
        public bool Enable { get; set; } = false;
        public string? Description { get; set; } = string.Empty;
        public string? BackgroundColor { get; set; } = string.Empty;
        public string? FontColor { get; set; } = string.Empty;
        public string? FontStyle { get; set; } = string.Empty;        
    }



    // =============== Modelo de configuracion de cada header/data de la tabla
    public class TableDataConfiguration
    {
        public bool Enable { get; set; } = false;
        public string? BackgroundColor { get; set; } = string.Empty;
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