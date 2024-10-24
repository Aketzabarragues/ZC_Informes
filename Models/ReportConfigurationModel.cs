using CommunityToolkit.Mvvm.ComponentModel;


namespace ZC_Informes.Models
{


    // =============== CONFIGURACION GENERAL DEL INFORME, IGUAL QUE EL JSON
    public class ReportConfigurationModel
    {
        public GeneralConfiguration? General { get; set; }
        public TableConfiguration? TableGeneral { get; set; }
        public TableConfiguration? TableAnalitics { get; set; }
        public TableConfiguration? TableProduction { get; set; }
        public TableDataHeaderConfiguration? TableDataHeader { get; set; }
        public TableConfiguration? TableData { get; set; }
    }



    // =============== Modelo de configuracion general
    public class GeneralConfiguration
    {
        public bool Enable { get; set; } = false;
        public string? PageSize { get; set; } = "A4";
        public bool IsHorizontal { get; set; } = false;
        public string? FontFamily { get; set; } = string.Empty;
        public string? HeaderImage1 { get; set; } = string.Empty;
        public string? HeaderImage2 { get; set; } = string.Empty;
        public string? HeaderText1 { get; set; } = string.Empty;
        public string? HeaderText2 { get; set; } = string.Empty;
    }



    // =============== Modelo de configuracion de tabla completa
    public class TableConfiguration
    {
        public TableGeneralConfig? Configuration { get; set; }
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
        public string? Description { get; set; } = string.Empty;
        public string? BackgroundColor { get; set; } = string.Empty;
        public int PaddingTop { get; set; } = 0;
        public int Columns { get; set; } = 0;
        public string? ColumnsSize { get; set; } = string.Empty;
        public List<int>? ColumnsSizeItems { get; set; } = new List<int>();
        public int DataType { get; set; } = 0;
        public int DataRow { get; set; } = 0;
        public int HeaderCategory { get; set; } = 0;
        public string? DataCategory { get; set; } = string.Empty;
        public List<int>? DataCategoryItems { get; set; } = new List<int>();
    }



    // =============== Modelo de configuracion de cada header/data de cada tabla
    public class TableDataConfiguration
    {
        public bool Enable { get; set; } = false;
        public string? BackgroundColor { get; set; } = string.Empty;
        public int FontSize { get; set; } = 0;
        public string? FontStyle { get; set; } = string.Empty;
        public List<string>? FontStyleItems { get; set; } = new List<string>();
        public string? CombineColumn { get; set; } = string.Empty;
        public List<int>? CombineColumnItems { get; set; } = new List<int>();
        public string? Data { get; set; } = string.Empty;
        public List<DataItem>? DataItems { get; set; } = new List<DataItem>();
    }



    // =============== Modelo de configuracion de la tabla "tipo"
    public class TableDataHeaderConfiguration
    {
        public bool Enable { get; set; } = false;
        public string? Description { get; set; } = string.Empty;
        public string? BackgroundColor { get; set; } = string.Empty;
        public int Types { get; set; } = 0;
        public string? Category { get; set; } = string.Empty;
        public List<string>? CategoryItems { get; set; } = new List<string>();
        public string? Data { get; set; } = string.Empty;
    }



    // =============== Modelo para los data items
    public class DataItem : ObservableObject
    {
        private int _configuration;
        public int Configuration
        {
            get => _configuration;
            set => SetProperty(ref _configuration, value);
        }

        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }

}