﻿using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wpf.Ui;
using ZC_Informes;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;

public class ReportConfigurationService : IReportConfigurationService
{

    //  =============== Servicios inyectados
    private readonly ISnackbarService _snackbarService;



    //  =============== Constructor
    public ReportConfigurationService()
    {
        _snackbarService = App.ServiceProvider!.GetRequiredService<ISnackbarService>();

    }



    //  =============== Metodo para cargar el archivo JSON
    public ReportConfigFullModel LoadConfiguration(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"No se ha encontrado el archivo de configuración: {filePath}");

            var json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<ReportConfigFullModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = false })
                          ?? throw new InvalidOperationException("La deserialización resultó en un objeto de configuración nulo.");
            
            //  Procesar las tablas del JSON
            ValidateAndProcess(config.Table1, nameof(config.Table1));
            ValidateAndProcess(config.Table2, nameof(config.Table2));
            ValidateAndProcess(config.Table3, nameof(config.Table3));
            ValidateAndProcess(config.Table4, nameof(config.Table4));
            ValidateAndProcess(config.Table5, nameof(config.Table5));

            return config;
        }
        catch (Exception ex)
        {
            var config = new ReportConfigFullModel();
            Log.Information($"Error al deserializar el JSON: {ex.Message}");
            return config;
        }
    }



    public List<ConfigBoolModel> LoadConfigBool(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var configBool = JsonSerializer.Deserialize<List<ConfigBoolModel>>(json);
        return configBool ?? new List<ConfigBoolModel> { new ConfigBoolModel() };

    }



    //  =============== Metodo para revisar nullabilidad de configuraciones y procesar datos
    private void ValidateAndProcess(ReportConfigTableModel? tableConfig, string tableName)
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
    private void ProcessFullTableConfig(ReportConfigTableModel? tableConfig, string tableName)
    {
        //  Creamos una configuracion vacia
        var reportConfigTableNull = new ReportConfigTableModel();

        _ = tableConfig!.Configuration ?? throw new ArgumentNullException(nameof(tableConfig.Configuration), "Configuration no puede ser nula o vacía.");

        //  En caso de que no este habilitada la tabla, no la procesamos y devolvemos valores vacios
        if (tableConfig!.Configuration!.Enable == false)
        {
            tableConfig = reportConfigTableNull;
            return;
        }                

        _ = tableConfig.Title!.Description ?? throw new ArgumentNullException(nameof(tableConfig.Title.Description), "Title.Description no puede ser nula o vacía.");
        _ = tableConfig.Title.BackgroundColor ?? throw new ArgumentNullException(nameof(tableConfig.Title.BackgroundColor), "Title.BackgroundColor no puede ser nula o vacía.");
        _ = tableConfig.Title.FontColor ?? throw new ArgumentNullException(nameof(tableConfig.Title.FontColor), "Title.FontColor no puede ser nula o vacía.");
        _ = tableConfig.Title.FontStyle ?? throw new ArgumentNullException(nameof(tableConfig.Title.FontStyle), "Title.FontColor no puede ser nula o vacía.");
        

        if (tableConfig.Configuration.Columns == 0)
        {
            throw new ArgumentNullException(nameof(tableConfig.Configuration.Columns), "Configuration.Columns no puede ser nula o vacía.");
        }        

        _ = tableConfig.Configuration.ColumnsSize ?? throw new ArgumentNullException(nameof(tableConfig.Configuration.ColumnsSize), "Configuration.ColumnsSize no puede ser nula o vacía.");
        //  Procesamos los datos de ColumsSize
        tableConfig!.Configuration!.ColumnsSizeItems = ParseAndValidateInt(
            tableConfig!.Configuration!.ColumnsSize,
            tableConfig!.Configuration!.Columns,
            tableName,
            "ColumnsSize"
        );

        if (tableConfig!.Configuration!.HeaderCategory == null)
        {
            throw new ArgumentNullException(nameof(tableConfig.Configuration.HeaderCategory), "Configuration.HeaderCategory no puede ser nula o vacía.");
        }
        _ = tableConfig!.Configuration!.HeaderCategoryItems ?? throw new ArgumentNullException(nameof(tableConfig.Configuration.HeaderCategoryItems), "Configuration.HeaderCategoryItems no puede ser nula o vacía.");
        _ = tableConfig!.Configuration!.DataCategory ?? throw new ArgumentNullException(nameof(tableConfig.Configuration.DataCategory), "Configuration.DataCategory no puede ser nula o vacía.");
        _ = tableConfig!.Configuration!.DataCategoryItems ?? throw new ArgumentNullException(nameof(tableConfig.Configuration.DataCategoryItems), "Configuration.DataCategoryItems no puede ser nula o vacía.");

        //  Procesamos los datos de HeaderCategory
        tableConfig!.Configuration!.HeaderCategoryItems = ParseInt(
            tableConfig!.Configuration!.HeaderCategory,
            tableName,
            "HeaderCategory"
        );

        //  Procesamos los datos de DataCategory
        tableConfig!.Configuration!.DataCategoryItems = ParseInt(
            tableConfig!.Configuration!.DataCategory,
            tableName,
            "DataCategory"
        );

        //  Procesamos los headers y data
        ProcessSubHeaders(tableConfig, tableName);
    }



    //  =============== Metodo para procesar los Headers (Header, SubHeader1, SubHeader2, SubHeader3) y data
    private void ProcessSubHeaders(ReportConfigTableModel tableConfig, string tableName)
    {

        var subHeaders = new[] { tableConfig.Header, tableConfig.SubHeader1, tableConfig.SubHeader2, tableConfig.SubHeader3, tableConfig.Data };
        var subHeaderNames = new[] { "Header", "SubHeader1", "SubHeader2", "SubHeader3", "Data" };

        //  Creamos una configuracion vacia
        var reportConfigTableDataNull = new ReportConfigTableDataModel();

        for (int i = 0; i < subHeaders.Length; i++)
        {
            //  Inicializamos y revisamos si son null
            var subHeader = subHeaders[i] ?? throw new ArgumentNullException(subHeaderNames[i], $"{subHeaderNames[i]} no puede ser nulo.");

            if (!subHeader.Enable)
            {
                subHeader = reportConfigTableDataNull;
                continue;
            }


            string combineColumnsValidate = subHeader.CombineColumn ?? throw new ArgumentNullException($"{subHeaderNames[i]} CombineColumn", $"{subHeaderNames[i]} CombineColumn no puede ser nulo.");
            subHeader.CombineColumnItems = ParseInt(subHeader.CombineColumn, tableName, $"{subHeaderNames[i]} CombineColumn");



            var columnTotalNumber = tableConfig!.Configuration!.Columns * tableConfig.Configuration.Rows;
            var totalSum = subHeader.CombineColumnItems.Sum();

            if (subHeaderNames[i] == "Data")
            {
                if (tableConfig!.Configuration.TableType == 0)
                {

                    columnTotalNumber = tableConfig!.Configuration!.Columns;
                    totalSum = subHeader.CombineColumnItems.Sum();
                }
                else
                {
                    columnTotalNumber = tableConfig!.Configuration!.Columns * tableConfig.Configuration.Rows;
                    totalSum = subHeader.CombineColumnItems.Sum();
                }                
            }
            else
            {
                columnTotalNumber = tableConfig!.Configuration!.Columns;
                totalSum = subHeader.CombineColumnItems.Sum();
            }

            if (totalSum != columnTotalNumber)
            {
                throw new ArgumentNullException($"El número de columnas de CombineColumn en {subHeaderNames[i]} no coincide. Columnas esperadas: {columnTotalNumber}, configuradas: {totalSum}.");

            }

            if (subHeaderNames[i] == "Data")
            {
                if (tableConfig!.Configuration.TableType == 0)
                {
                    columnTotalNumber = tableConfig!.Configuration!.Columns;
                }
                else
                {
                    columnTotalNumber = tableConfig!.Configuration!.Columns * tableConfig.Configuration.Rows;
                }
            }
            else
            {
                columnTotalNumber = subHeader.CombineColumnItems.Count();
            }


            string fontStyleToValidate = subHeader.FontStyle ?? throw new ArgumentNullException($"{subHeaderNames[i]} FontStyle", $"{subHeaderNames[i]} FontStyle no puede ser nulo.");
            subHeader.FontStyleItems = ParseAndValidateStrings(fontStyleToValidate, columnTotalNumber, tableName, $"{subHeaderNames[i]} FontStyle");
            

            subHeader.DataTypeItems = ParseAndValidateInt(
                subHeader!.DataType!,
                columnTotalNumber,
                tableName,
                "DataType"
            );
            string dataSourceToValidate = subHeader.DataSource ?? throw new ArgumentNullException($"{subHeaderNames[i]} DataSource", $"{subHeaderNames[i]} DataSource no puede ser nulo.");
            subHeader.DataSourceItems = ParseAndValidateStrings(dataSourceToValidate, columnTotalNumber, tableName, $"{subHeaderNames[i]} DataSource");

            string dataUnitsToValidate = subHeader.DataUnits ?? throw new ArgumentNullException($"{subHeaderNames[i]} DataSource", $"{subHeaderNames[i]} DataSource no puede ser nulo.");
            subHeader.DataUnitsItems = ParseAndValidateStrings(dataUnitsToValidate, columnTotalNumber, tableName, $"{subHeaderNames[i]} DataUnits");
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

        var dataStrings = data.Split(';', StringSplitOptions.None);
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

        var dataStrings = data.Split(';', StringSplitOptions.None);
        if (dataStrings.Length != expectedColumns)
        {
            throw new InvalidDataException($"El número de columnas de {fieldName} en {tableName} no coincide. Columnas esperadas: {expectedColumns}, configuradas: {dataStrings.Length}.");
        }

        return dataStrings.ToList();
    }



    //  =============== Metodo para validar las cadenas de strings y retornarlos como una lista de Int
    //  Tienen que contengan el numero correcto de valores segun la configuracion de las columnas
    private List<int> ParseInt(string data, string tableName, string fieldName)
    {
        if (string.IsNullOrEmpty(data))
        {
            throw new InvalidDataException($"{fieldName} en {tableName} está vacío.");
        }
        var dataStrings = data.Split(';', StringSplitOptions.None);
        return dataStrings.Select(int.Parse).ToList();
    }

}
