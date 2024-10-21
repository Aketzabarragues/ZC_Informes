using System.IO;
using System.Text.Json;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using ZC_Informes;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;

public class ReportConfigurationService : IReportConfigurationService
{

    // =============== Variables o propiedades para almacenar los datos
    private readonly string _filePath;



    //  =============== Servicios inyectados
    private readonly ISnackbarService _snackbarService;



    //  =============== Constructor
    public ReportConfigurationService(string filePath, ISnackbarService snackbarService)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _snackbarService = snackbarService ?? throw new ArgumentNullException(nameof(snackbarService));
    }



    //  =============== Metodo para cargar el archivo JSON
    public ReportConfigurationModel LoadConfiguration()
    {

        
        if (!File.Exists(_filePath)) throw new FileNotFoundException($"No se ha encontrado el archivo de configuración: {_filePath}")
        

        var json = File.ReadAllText(_filePath);
        var config = JsonSerializer.Deserialize<ReportConfigurationModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                      ?? throw new InvalidOperationException("La deserialización resultó en un objeto de configuración nulo.");

        Log.Information("Archivo JSON cargado correctamente: {Config}", JsonSerializer.Serialize(config));


        //  Procesar las tablas del JSON
        ValidateAndProcess(config.TableGeneral, nameof(config.TableAnalitics));
        ValidateAndProcess(config.TableAnalitics, nameof(config.TableAnalitics));
        ValidateAndProcess(config.TableProduction, nameof(config.TableProduction));
        ValidateAndProcess(config.TableData, nameof(config.TableData));

        return config;
    }



    //  =============== Metodo para revisar nullabilidad de configuraciones y procesar datos
    private void ValidateAndProcess(TableConfiguration? tableConfig, string tableName)
    {
        if (tableConfig == null)
        {
            throw new ArgumentNullException(tableName);
        }
        ProcessFullTableConfig(tableConfig, tableName);
    }


    //  =============== Metodo para procesar cada configuración de tabla
    /*  1:  Primero miramos si la configuracion no es null
     *  2:  Procesamos los datos de configuracion
     *  3:  Configuramos los datos de header
     *  4:  Configuramos los datos de subheader1
     *  5:  Configuramos los datos de subheader2
     *  6:  Configuramos los datos de subheader3
     *  7:  Configuramos los datos de data
    */
    private void ProcessFullTableConfig(TableConfiguration tableConfig, string tableName)
    {

        //  Revisamos si son null
        _ = tableConfig.Configuration ?? throw new ArgumentNullException(nameof(tableConfig.Configuration), "Configuration no puede ser nula o vacía.");
        _ = tableConfig.Configuration.ColumnsSize ?? throw new ArgumentNullException(nameof(tableConfig.Configuration.ColumnsSize), "Configuration.ColumnsSize no puede ser nula o vacía.");
        _ = tableConfig.Configuration.DataCategory ?? throw new ArgumentNullException(nameof(tableConfig.Configuration.DataCategory), "DataCategory.ColumnsSize no puede ser nula o vacía.");



        // Procesamos los datos de ColumsSize
        tableConfig.Configuration.ColumnsSizeItems = ParseAndValidateInt(
            tableConfig.Configuration.ColumnsSize,
            tableConfig.Configuration.Columns,
            tableName,
            "ColumnsSize"
        );

        //  Procesamos los datos de Data.Category
        tableConfig.Configuration.DataCategoryItems = ParseInt(
            tableConfig.Configuration.DataCategory,
            tableName,
            "DataCategory"
        );

        //  Procesamos los headers y data
        ProcessSubHeaders(tableConfig, tableName);
    }



    //  =============== Metodo para procesar los Headers (Header, SubHeader1, SubHeader2, SubHeader3) y data
    private void ProcessSubHeaders(TableConfiguration tableConfig, string tableName)
    {


        //  Revisamos si son null
        _ = tableConfig.Configuration ?? throw new ArgumentNullException(nameof(tableConfig.Configuration), "Configuration no puede ser nula o vacía.");
        _ = tableConfig.Configuration.DataCategory ?? throw new ArgumentNullException(nameof(tableConfig.Configuration.DataCategory), "DataCategory.ColumnsSize no puede ser nula o vacía.");



        var subHeaders = new[] { tableConfig.Header, tableConfig.SubHeader1, tableConfig.SubHeader2, tableConfig.SubHeader3, tableConfig.Data };
        var subHeaderNames = new[] { "Header", "SubHeader1", "SubHeader2", "SubHeader3", "Data" };


        for (int i = 0; i < subHeaders.Length; i++)
        {
            //  Inicializamos y revisamos si son null
            var subHeader = subHeaders[i] ?? throw new ArgumentNullException(subHeaderNames[i], $"{subHeaderNames[i]} no puede ser nulo.");
            
            string fontStyleToValidate = subHeader.FontStyle ?? throw new ArgumentNullException($"{subHeaderNames[i]} FontStyle", $"{subHeaderNames[i]} FontStyle no puede ser nulo.");
            subHeader.FontStyleItems = ParseAndValidateStrings(fontStyleToValidate, tableConfig.Configuration.Columns, tableName, $"{subHeaderNames[i]} FontStyle");
            string combineColumnsValidate = subHeader.CombineColumn ?? throw new ArgumentNullException($"{subHeaderNames[i]} CombineColumn", $"{subHeaderNames[i]} CombineColumn no puede ser nulo.");
            subHeader.CombineColumnItems = ParseAndValidateInt(subHeader.CombineColumn, tableConfig.Configuration.Columns, tableName, $"{subHeaderNames[i]} CombineColumn");
            string dataToValidate = subHeader.Data ?? throw new ArgumentNullException($"{subHeaderNames[i]} Data", $"{subHeaderNames[i]} Data no puede ser nulo.");
            subHeader.DataItems = ParseAndValidateData(dataToValidate, tableConfig.Configuration.Columns, tableName);
        }
    }



    //  =============== Metodo para validar las cadenas de strings y retornarlos como una lista de Int
    //  Tienen que contengan el numero correcto de valores segun la configuracion de las columnas
    private List<int> ParseAndValidateInt(string data, int expectedColumns, string tableName, string fieldName)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new InvalidDataException($"{fieldName} en {tableName} está vacío.");
        }

        var dataStrings = data.Split(';', StringSplitOptions.RemoveEmptyEntries);
        if (dataStrings.Length != expectedColumns)
        {
            throw new InvalidDataException($"El número de columnas de {fieldName} en {tableName} no coincide. Columnas esperadas: {expectedColumns}, configuradas: {dataStrings.Length}.");
        }

        return dataStrings.Select(int.Parse).ToList();
    }



    //  =============== Metodo para validar las cadenas de strings y retornarlos como una lista de Strings
    //  Tienen que contengan el numero correcto de valores segun la configuracion de las columnas
    private List<string> ParseAndValidateStrings(string data, int expectedColumns, string tableName, string fieldName)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new InvalidDataException($"{fieldName} en {tableName} está vacío.");
        }

        var dataStrings = data.Split(';', StringSplitOptions.RemoveEmptyEntries);
        if (dataStrings.Length != expectedColumns)
        {
            throw new InvalidDataException($"El número de columnas de {fieldName} en {tableName} no coincide. Columnas esperadas: {expectedColumns}, configuradas: {dataStrings.Length}.");
        }

        return dataStrings.ToList();
    }



    //  =============== Metodo para validar las cadenas de strings de data y retornarlos como una lista de Strings
    //  Tienen que contengan el numero correcto de valores segun la configuracion de las columnas, teniendo en cuenta que cada campo tiene 2 valores.
    private List<DataItem> ParseAndValidateData(string data, int expectedColumns, string tableName)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new InvalidDataException($"El valor de Data en {tableName} es nulo o vacío.");
        }

        var dataStrings = data.Split(';', StringSplitOptions.RemoveEmptyEntries);
        if (dataStrings.Length % 2 != 0)
        {
            throw new InvalidDataException($"El número de valores de Data en {tableName} tiene que ser par.");
        }

        int actualColumns = dataStrings.Length / 2;
        if (actualColumns != expectedColumns)
        {
            throw new InvalidDataException($"El número de columnas en {tableName} no coincide. Columnas esperadas: {expectedColumns}, configuradas: {actualColumns}.");
        }

        var dataItems = new List<DataItem>();
        for (int i = 0; i < dataStrings.Length; i += 2)
        {
            dataItems.Add(new DataItem
            {
                Configuration = int.Parse(dataStrings[i]),
                Value = dataStrings[i + 1]
            });
        }

        return dataItems;
    }



    //  =============== Metodo para validar las cadenas de strings y retornarlos como una lista de Int
    //  Tienen que contengan el numero correcto de valores segun la configuracion de las columnas
    private List<int> ParseInt(string data, string tableName, string fieldName)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new InvalidDataException($"{fieldName} en {tableName} está vacío.");
        }
        var dataStrings = data.Split(';', StringSplitOptions.RemoveEmptyEntries);
        return dataStrings.Select(int.Parse).ToList();
    }

}
