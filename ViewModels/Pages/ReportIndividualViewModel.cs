﻿using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using ZC_Informes;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;
using ZC_Informes.Services;
using static QuestPDF.Helpers.Colors;


public partial class ReportIndividualViewModel : ObservableObject
{


    // =============== Variables o propiedades para almacenar los datos
    private IEnumerable<ReportSqlDataFormattedModel>? _table1HeaderSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table1DataSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table2HeaderSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table2DataSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table3HeaderSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table3DataSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table4HeaderSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table4DataSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table5HeaderSql = [];
    private IEnumerable<ReportSqlDataFormattedModel>? _table5DataSql = [];


    //  =============== Servicios inyectados
    private readonly ConfigurationService _configurationService;
    private readonly IReportConfigurationService _reportConfigurationService;
    private readonly IPdfGeneratorService _pdfGeneratorService;
    private readonly ISnackbarService _snackbarService;
    private readonly IReportSqlService _reportSqlService;
    private readonly AppConfigModel _appConfig;



    //  =============== Propiedades observables
    [ObservableProperty] private string? filePath = string.Empty;
    [ObservableProperty] private bool isGeneratingPdf = false;
    [ObservableProperty] private ReportConfigFullModel? reportConfig;
    [ObservableProperty] private DateTime? selectedDate;
    [ObservableProperty] private ObservableCollection<ReportSqlCategoryModel>? reportCategory;
    [ObservableProperty] private ObservableCollection<ReportSqlReportListModel>? reportList;
    [ObservableProperty] private int selectedCategoryNumber;
    [ObservableProperty] private int selectedDataNumber;
    [ObservableProperty] private bool isAuthenticated = false;



    //  =============== Comandos
    public IRelayCommand LoadReportListCommand { get; }
    public IRelayCommand ShowReportListId { get; }



    //  =============== Constructor
    public ReportIndividualViewModel()
    {

        if (App.ServiceProvider == null) throw new ArgumentNullException(nameof(App.ServiceProvider));

        _configurationService = App.ServiceProvider.GetRequiredService<ConfigurationService>();

        _reportConfigurationService = App.ServiceProvider.GetRequiredService<IReportConfigurationService>();
        _pdfGeneratorService = App.ServiceProvider.GetRequiredService<IPdfGeneratorService>();
        _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();
        _reportSqlService = App.ServiceProvider.GetRequiredService<IReportSqlService>();
        _appConfig = _configurationService.LoadConfiguration();

        LoadReportListCommand = new AsyncRelayCommand(LoadReportList);
        ShowReportListId = new AsyncRelayCommand(GenerateAndPrintReport);

        SelectedDate = DateTime.Today;
        IsGeneratingPdf = false;
        IsAuthenticated = false;

        Task task = LoadCategoriesFromSQL();
    }



    //  =============== Metodo para generar e imprimir un informe al pulsar el boton
    /*
     * Para generar un informe, tenemos que seguir los siguientes pasos:
     *      1:   Revisamos si hay item en el listview seleccionado
     *      2:   Cargamos el archivo de configuracion JSON y comprobamos si es correcto
     *      3:   Realizamos la consulta a la base de datos para traer todos los datos necesarios para el informe
     *      4:   Generamos el informe
    */
    private async Task GenerateAndPrintReport()
    {
        try
        {
            if (ReportList?[SelectedDataNumber].Codigo != null && ReportList.Count != 0)
            {

                IsGeneratingPdf = true;                                
                Log.Information($"Inicio generar informe: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\Report\", $"{ReportCategory?[SelectedCategoryNumber].Id}.json");


                //  =====================================================================================================================
                //  2. Cargar archivo de configuración de informe de forma asíncrona
                bool LoadconfigurationResult = false;
                LoadconfigurationResult = await LoadReportConfigurationAsync();
                if (!LoadconfigurationResult)
                {
                    ShowError("Error al leer el archivo de configuración", ControlAppearance.Danger);
                    IsGeneratingPdf = false;
                    return;
                }


                //  =====================================================================================================================
                //  3. Lanzar consultas SQL de forma asíncrona                
                var sqlResult = false;
                var sqlQuery = "SELECT * FROM ZC_INFORME WHERE Tipo IN @Tipo AND Codigo = @Codigo";
                var reportParams = new ReportSqlDataParametersModel
                {
                    Tipo = ReportConfig?.Table1?.Configuration?.HeaderCategoryItems,
                    Codigo = ReportList[SelectedDataNumber].Codigo
                };

                //  Leemos los datos de la tabla 1
                (sqlResult, _table1HeaderSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 1 Header");
                    IsGeneratingPdf = false;
                    return;
                }
                reportParams.Tipo = ReportConfig?.Table1?.Configuration?.DataCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table1DataSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 1 Data");
                    IsGeneratingPdf = false;
                    return;
                }

                //  Leemos los datos de la tabla 2
                reportParams.Tipo = ReportConfig?.Table2?.Configuration?.HeaderCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table2HeaderSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 2 Header");
                    IsGeneratingPdf = false;
                    return;
                }
                reportParams.Tipo = ReportConfig?.Table2?.Configuration?.DataCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table2DataSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);                
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 2 Data");
                    IsGeneratingPdf = false;
                    return;
                }

                //  Leemos los datos de la tabla 3
                reportParams.Tipo = ReportConfig?.Table3?.Configuration?.DataCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table3HeaderSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 3 Header");
                    IsGeneratingPdf = false;
                    return;
                }
                reportParams.Tipo = ReportConfig?.Table3?.Configuration?.DataCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table3DataSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 3 Data");
                    IsGeneratingPdf = false;
                    return;
                }

                //  Leemos los datos de la tabla 4
                reportParams.Tipo = ReportConfig?.Table4?.Configuration?.HeaderCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table4HeaderSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 4 Header");
                    IsGeneratingPdf = false;
                    return;
                }
                reportParams.Tipo = ReportConfig?.Table4?.Configuration?.DataCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table4DataSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 4 Data");
                    IsGeneratingPdf = false;
                    return;
                }

                //  Leemos los datos de la tabla 5
                reportParams.Tipo = ReportConfig?.Table5?.Configuration?.HeaderCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table5HeaderSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 5 Header");
                    IsGeneratingPdf = false;
                    return;
                }
                reportParams.Tipo = ReportConfig?.Table5?.Configuration?.DataCategoryItems;
                reportParams.Codigo = ReportList[SelectedDataNumber].Codigo;
                (sqlResult, _table5DataSql) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, true);                
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    Log.Information("Error al leer los datos desde SQL. Tabla 5 Data");
                    IsGeneratingPdf = false;
                    return;
                }

                //  =====================================================================================================================
                //  4. Generar informe PDF de forma asíncrona
                bool informeResult = await GeneratePdfAsync();
                if (!informeResult)
                {
                    ShowError("Error al generar el informe PDF", ControlAppearance.Danger);
                    IsGeneratingPdf = false;
                    return;
                }

                ShowError("Operación completada con éxito", ControlAppearance.Success);
                Log.Information($"Fin generar informe: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                IsGeneratingPdf = false;
            }
            else
            {
                ShowError("Seleccione informe para generar.", ControlAppearance.Danger);
                IsGeneratingPdf = false;
            }

        }
        catch (Exception ex)
        {
            ShowError($"Ocurrió un error inesperado: {ex.Message}", ControlAppearance.Danger);
            Log.Information(ex.Message);
            IsGeneratingPdf = false;
        }
    }



    //  =============== Metodo para leer la configuracion del informe desde el JSON
    private async Task<bool> LoadReportConfigurationAsync()
    {
        try
        {
            // Verificar si la ruta del archivo no está vacía
            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
            {
                ShowError("Ruta de archivo de configuracion incorrecta.", ControlAppearance.Danger);
                return false;
            }
            // Ejecutar la carga de configuración en un hilo en segundo plano
            ReportConfig = await Task.Run(() => _reportConfigurationService.LoadConfiguration(FilePath));
            if (ReportConfig != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception ex)
        {
            ShowError($"Error al cargar la configuracion. Error: {ex.Message}", ControlAppearance.Danger);
            Log.Information(ex.Message);
            return false;
        }
    }



    //  =============== Metodo para leer la informacion desde SQL
    private async Task<(bool, IEnumerable<ReportSqlDataFormattedModel>?)> LoadReportDataFromSqlAsync(string sqlQuery, object parameters, bool firstOrAll)
    {
        // Inicializa una colección vacía para los datos
        IEnumerable<ReportSqlDataFormattedModel> dataNull = Enumerable.Empty<ReportSqlDataFormattedModel>();

        try
        {
            // Verifica que se haya seleccionado una categoría de informe
            if (ReportList?[SelectedDataNumber].Id == null)
            {
                ShowError("Seleccione primero la categoría de informes", ControlAppearance.Caution);
                return (false, dataNull);
            }

            // Ejecutar consulta con Dapper
            IEnumerable<ReportSqlDataFormattedModel> reportData = await _reportSqlService.GetReportDataAsync(sqlQuery, parameters);

            // Verificar si hay registros en la consulta
            if (reportData.Any())
            {
                return firstOrAll
                    ? (true, reportData)
                    : (true, new[] { reportData.First() }); // Retornar solo el primer registro
            }

            // Si no hay datos, retorna verdadero con la colección vacía
            return (true, dataNull);
        }
        catch (Exception ex)
        {
            ShowError($"Error al cargar los datos desde SQL: {ex.Message}", ControlAppearance.Danger);
            Log.Information(ex.Message);
            return (false, dataNull);
        }
    }



    //  =============== Metodo para generar el PDF
    private async Task<bool> GeneratePdfAsync()
    {
        try
        {

            // Verifica que ReportSaveFolder no esté vacío o nulo
            if (!string.IsNullOrEmpty(_appConfig.ReportSaveFolder))
            {
                // Obtén la ruta completa y verifica que exista
                string folderPath = Path.GetFullPath(_appConfig.ReportSaveFolder);

                if (Directory.Exists(folderPath))
                {
                    try
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss"); // Formato: YYYYMMDD_HHMMSS
                        string fileName = $"Reporte_{ReportCategory[SelectedCategoryNumber].Id}_{timestamp}.pdf"; // Nombre del archivo
                        string filePath = Path.Combine(folderPath, fileName); // Combina la ruta del directorio con el nombre del archivo

                        // Ejecutar la carga de configuración en un hilo en segundo plano
                        await Task.Run(() => _pdfGeneratorService.GeneratePdf(filePath,
                            ReportConfig!,
                            _table1HeaderSql!,
                            _table1DataSql!,
                            _table2HeaderSql!,
                            _table2DataSql!,
                            _table3HeaderSql!,
                            _table3DataSql!,
                            _table4HeaderSql!,
                            _table4DataSql!,
                            _table5HeaderSql!,
                            _table5DataSql!));
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ShowError("No se pudo abrir la carpeta de informes.", ControlAppearance.Danger);
                        Log.Error($"Error al abrir la carpeta de informes: {ex.Message}");
                        return false;
                    }
                }
                else
                {
                    ShowError("La carpeta especificada no existe.", ControlAppearance.Caution);
                    return false;
                }
            }
            else
            {
                // Mensaje de advertencia si la ruta está vacía
                ShowError("La carpeta de informes no está configurada.", ControlAppearance.Caution);
                return false;
            }



            
        }
        catch (Exception ex)
        {
            ShowError($"Error al generar el PDF: {ex.Message}", ControlAppearance.Danger);
            Log.Information(ex.Message);
            return false;
        }
    }



    //  =============== Metodo para leer las categorias de informes
    private async Task LoadCategoriesFromSQL()
    {
        try
        {
            var sqlQuery = "SELECT * FROM ZC_INFORME_TIPO WHERE Visible_Individual = 1";
            IEnumerable<ReportSqlCategoryModel> reportCategory = await _reportSqlService.GetReportCategoryAsync(sqlQuery);
            ReportCategory = new ObservableCollection<ReportSqlCategoryModel>(reportCategory);
        }
        catch (Exception ex)
        {
            ShowError($"Error al cargar las categorias de informes: {ex.Message}", ControlAppearance.Danger);
            Log.Information(ex.Message);
        }
    }



    //  =============== Metodo para leer la lista de informes por categoria y fecha
    private async Task LoadReportList()
    {

        try
        {
            if (ReportCategory?[SelectedCategoryNumber].Id != 0)
            {
                var sqlQuery = @"
                                SELECT 
                                    Id, 
                                    CONCAT(CONVERT(varchar, Fecha_1, 23), ' - ', CONVERT(varchar, Hora_1, 8)) AS Titulo,
                                    Codigo
                                FROM 
                                    ZC_INFORME 
                                WHERE 
                                    TIPO = @Id 
                                    AND FECHA_1 = @Date";


                // Parámetros para la consulta
                var parameters = new
                {
                    ReportCategory?[SelectedCategoryNumber].Id,
                    Date = SelectedDate!.Value.ToString("yyyy-MM-dd") // Asegura el formato correcto
                };
                
                // Ejecutar consulta con Dapper
                IEnumerable<ReportSqlReportListModel> reportData = await _reportSqlService.GetReportListAsync(sqlQuery, parameters);

                // Verificar si hay registros en la consulta
                if (reportData.Any()) // Si hay registros
                {
                    ReportList = new ObservableCollection<ReportSqlReportListModel>(reportData);
                }
                else
                {
                    //  Vaciamos la lista en caso de que se haya generado.
                    ReportList?.Clear();

                    // Mostrar mensaje si no hay registros
                    ShowError("No hay registros para la fecha seleccionada", ControlAppearance.Caution);
                }

            }
            else
            {
                ShowError("Seleccione primero la categoria de informes", ControlAppearance.Caution);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error al cargar la lista de informes: {ex.Message}", ControlAppearance.Danger);
            Log.Information(ex.Message);
        }

    }



    //  =============== Metodo para mostrar error al generar PDF
    private void ShowError(string error, ControlAppearance appearance)
    {
        _snackbarService.Show("Informe individual", error, appearance, TimeSpan.FromSeconds(1));
        
    }


}


