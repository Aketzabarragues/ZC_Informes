using System.IO;
using System.Text.Json;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;

public class ReportConfigurationService : IReportConfigurationService
{

    // Campos privados
    private readonly string _filePath;
    private readonly ISnackbarService _snackbarService;



    // Constructor
    public ReportConfigurationService(string filePath, ISnackbarService snackbarService)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _snackbarService = snackbarService ?? throw new ArgumentNullException(nameof(snackbarService));
    }



    // Método para cargar el archivo JSON
    public ReportConfigurationModel LoadConfiguration()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"No se ha encontrado el archivo de configuración: {_filePath}");
        }

        var json = File.ReadAllText(_filePath);
        var config = JsonSerializer.Deserialize<ReportConfigurationModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                      ?? throw new InvalidOperationException("La deserialización resultó en un objeto de configuración nulo.");

        Log.Information("Archivo JSON cargado correctamente: {Config}", JsonSerializer.Serialize(config));

        // Procesar las tablas del JSON
        ProcessTableConfig(config.TableGeneral, nameof(config.TableGeneral));
        ProcessTableConfig(config.TableAnalitics, nameof(config.TableAnalitics));
        ProcessTableConfig(config.TableProduction, nameof(config.TableProduction));
        ProcessTableConfig(config.TableData, nameof(config.TableData));

        return config;
    }



    // Método para procesar cada configuración de tabla
    private void ProcessTableConfig(TableConfiguration tableConfig, string tableName)
    {
        if (tableConfig == null)
        {
            throw new ArgumentNullException(nameof(tableConfig), $"El objeto {tableName} es nulo.");
        }

        tableConfig.Configuration.ColumnsSizeItems = ParseAndValidateInt(tableConfig.Configuration.ColumnsSize, tableConfig.Configuration.Columns, tableName, "ColumnsSize");
        ProcessSubHeaders(tableConfig, tableName);
    }



    // Procesar subheaders (Header, SubHeader1, SubHeader2, SubHeader3)
    private void ProcessSubHeaders(TableConfiguration tableConfig, string tableName)
    {
        var subHeaders = new[] { tableConfig.Header, tableConfig.SubHeader1, tableConfig.SubHeader2, tableConfig.SubHeader3, tableConfig.Data };
        var subHeaderNames = new[] { "Header", "SubHeader1", "SubHeader2", "SubHeader3", "Data" };

        for (int i = 0; i < subHeaders.Length; i++)
        {
            var subHeader = subHeaders[i];
            subHeader.FontStyleItems = ParseAndValidateStrings(subHeader.FontStyle, tableConfig.Configuration.Columns, tableName, $"{subHeaderNames[i]} FontStyle");
            subHeader.CombineColumnItems = ParseAndValidateInt(subHeader.CombineColumn, tableConfig.Configuration.Columns, tableName, $"{subHeaderNames[i]} CombineColumn");
            subHeader.CategoryItems = ParseAndValidateInt(subHeader.Category, subHeader.Category.Split(';', StringSplitOptions.RemoveEmptyEntries).Length, tableName, $"{subHeaderNames[i]} Category");
            subHeader.DataItems = ParseAndValidateData(subHeader.Data, tableConfig.Configuration.Columns, tableName);
        }
    }



    // Métodos para descomponer y validar las columnas
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


}
