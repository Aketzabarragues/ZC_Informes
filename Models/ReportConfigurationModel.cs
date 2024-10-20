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
        public bool Enable { get; set; }
        public string? PageSize { get; set; }
        public bool IsHorizontal { get; set; }
        public string? FontFamily { get; set; }
        public string? HeaderImage1 { get; set; }
        public string? HeaderImage2 { get; set; }
        public string? HeaderText1 { get; set; }
        public string? HeaderText2 { get; set; }
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
        public bool Enable { get; set; }
        public string? Description { get; set; }
        public string? BackgroundColor { get; set; }
        public int Columns { get; set; }
        public string? ColumnsSize { get; set; }
        public List<int>? ColumnsSizeItems { get; set; }
        public int DataType { get; set; }
        public int DataRow { get; set; }
        public string? HeaderCategory { get; set; }
        public List<int>? HeaderCategoryItems { get; set; }
        public string? DataCategory { get; set; }
        public List<int>? DataCategoryItems { get; set; }
    }



    // =============== Modelo de configuracion de cada header/data de cada tabla
    public class TableDataConfiguration
    {
        public bool Enable { get; set; }
        public string? BackgroundColor { get; set; }
        public int FontSize { get; set; }
        public string? FontStyle { get; set; }
        public List<string>? FontStyleItems { get; set; }
        public string? CombineColumn { get; set; }
        public List<int>? CombineColumnItems { get; set; }
        public string? Data { get; set; }
        public List<DataItem>? DataItems { get; set; }
    }



    // =============== Modelo de configuracion de la tabla "tipo"
    public class TableDataHeaderConfiguration
    {
        public bool Enable { get; set; }
        public string? Description { get; set; }
        public string? BackgroundColor { get; set; }
        public int Types { get; set; }
        public string? Category { get; set; }
        public List<string>? CategoryItems { get; set; }
        public string? Data { get; set; }
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

        private string _value;
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }

}