using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;


namespace ZC_Informes.Models
{

    public class ReportConfigurationModel
    {
        public GeneralConfiguration? General { get; set; }
        public TableConfiguration? TableGeneral { get; set; }
        public TableConfiguration? TableAnalitics { get; set; }
        public TableConfiguration? TableProduction { get; set; }
        public TableDataHeader? TableDataHeader { get; set; }
        public TableConfiguration? TableData { get; set; }
    }

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

    public class TableConfiguration
    {
        public TableGeneralConfig? Configuration { get; set; }
        public HeaderConfiguration? Header { get; set; }
        public HeaderConfiguration? SubHeader1 { get; set; }
        public HeaderConfiguration? SubHeader2 { get; set; }
        public HeaderConfiguration? SubHeader3 { get; set; }
        public HeaderConfiguration? Data { get; set; }
    }

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
    }

    public class HeaderConfiguration
    {
        public bool Enable { get; set; }
        public string? BackgroundColor { get; set; }
        public int FontSize { get; set; }
        public string? FontStyle { get; set; }
        public List<string>? FontStyleItems { get; set; }
        public string? CombineColumn { get; set; }
        public List<int>? CombineColumnItems { get; set; }
        public string? Category { get; set; }
        public List<int>? CategoryItems { get; set; }
        public string? Data { get; set; }
        public List<DataItem>? DataItems { get; set; }
    }


    public class TableDataHeader
    {
        public bool Enable { get; set; }
        public string? Description { get; set; }
        public string? BackgroundColor { get; set; }
        public int Types { get; set; }
        public string? Category { get; set; }
        public List<string>? CategoryItems { get; set; }
        public string? Data { get; set; }
    }


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