using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using ZC_Informes;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;
using ZC_Informes.Services;


public partial class ReportBetweenDatesViewModel : ObservableObject
{


    // =============== Variables o propiedades para almacenar los datos
    private readonly Dictionary<int, (IEnumerable<ReportSqlDataFormattedModel>? Header, IEnumerable<ReportSqlDataFormattedModel>? Data)> _tableData = new();

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
    [ObservableProperty] private DateTime? selectedDateStart;
    [ObservableProperty] private DateTime? selectedDateEnd;
    [ObservableProperty] private ObservableCollection<ReportSqlCategoryFormattedModel>? reportCategory;
    [ObservableProperty] private ObservableCollection<ReportSqlReportListModel>? reportList;
    [ObservableProperty] private int selectedCategoryNumber;
    [ObservableProperty] private int selectedDataNumber;
    [ObservableProperty] private bool isAuthenticated = false;



    //  =============== Comandos
    public IRelayCommand LoadReportListCommand { get; }
    public IRelayCommand GenerateReportCommand { get; }



    //  =============== Constructor
    public ReportBetweenDatesViewModel()
    {

        if (App.ServiceProvider == null) throw new ArgumentNullException(nameof(App.ServiceProvider));

        _configurationService = App.ServiceProvider.GetRequiredService<ConfigurationService>();

        _reportConfigurationService = App.ServiceProvider.GetRequiredService<IReportConfigurationService>();
        _pdfGeneratorService = App.ServiceProvider.GetRequiredService<IPdfGeneratorService>();
        _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();
        _reportSqlService = App.ServiceProvider.GetRequiredService<IReportSqlService>();
        _appConfig = _configurationService.LoadConfiguration();

        LoadReportListCommand = new AsyncRelayCommand(LoadReportListFromSql);
        GenerateReportCommand = new AsyncRelayCommand(GenerateAndPrintReport);

        SelectedDateStart = DateTime.Today.AddMonths(-1);
        SelectedDateEnd = DateTime.Today;
        IsGeneratingPdf = false;
        IsAuthenticated = false;

        Task task = LoadCategoriesFromSql();
    }



    //  =============== Metodo para generar e imprimir un informe al pulsar el boton
    //  
    //  Para generar un informe, tenemos que seguir los siguientes pasos:
    //      1:   Revisamos si hay item en el listview seleccionado
    //      2:   Cargamos el archivo de configuracion JSON y comprobamos si es correcto
    //      3:   Realizamos la consulta a la base de datos para traer todos los datos necesarios para el informe
    //      4:   Generamos el informe
    private async Task GenerateAndPrintReport()
    {

        //  =====================================================================================================================
        //  1. Revisamos si hay item en el listview seleccionado
        if (!IsReportListValid()) return;

        try
        {
            IsGeneratingPdf = true;
            Log.Information($"Inicio generar informe: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Report", $"{ReportCategory![SelectedCategoryNumber].Id}.json");

            //  =====================================================================================================================
            //  2. Cargar archivo de configuración de informe de forma asíncrona
            if (!await LoadReportConfigurationAsync()) return;

            //  =====================================================================================================================
            //  3. Lanzar consultas SQL de forma asíncrona                
            if (!await LoadAllReportTables()) return;

            //  =====================================================================================================================
            //  4. Generar informe PDF de forma asíncrona
            if (!await GeneratePdfAsync()) return;

            ShowMessage("Operación completada con éxito", ControlAppearance.Success);
            Log.Information($"Fin generar informe: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");            

        }
        catch (Exception ex)
        {
            ShowMessage($"Ocurrió un error inesperado: {ex.Message}", ControlAppearance.Danger);
            Log.Error(ex.Message);
        }
        finally
        {
            IsGeneratingPdf = false;
        }

    }



    //  =============== Metodo para leer los datos de todas las tablas
    private async Task<bool> LoadAllReportTables()
    {
        
        for (int i = 1; i <= 5; i++)
        {
            var headerItems = GetReportConfigTable(i)?.Configuration?.HeaderCategoryItems;
            var dataItems = GetReportConfigTable(i)?.Configuration?.DataCategoryItems;

            if (!await LoadTableDataFromSqlAsync(i, headerItems, false) || !await LoadTableDataFromSqlAsync(i, dataItems, true))
            {
                ShowMessage($"Error al leer los datos de la tabla {i} desde SQL.", ControlAppearance.Danger);
                return false;
            }
        }
        return true;
    }



    //  =============== Metodo para devolver la configuracion de las tablas en base a un index
    private ReportConfigTableModel? GetReportConfigTable(int tableNumber) => tableNumber switch
    {
        1 => ReportConfig?.Table1,
        2 => ReportConfig?.Table2,
        3 => ReportConfig?.Table3,
        4 => ReportConfig?.Table4,
        5 => ReportConfig?.Table5,
        _ => null
    };



    //  =============== Metodo para leer los datos de SQL
    private async Task<bool> LoadTableDataFromSqlAsync(int tableNumber, List<int>? categoryItems, bool isData)
    {
        var sqlQuery = "SELECT * FROM ZC_INFORME WHERE Tipo IN @Tipo AND Codigo = @Codigo ORDER BY Fecha_1, Hora_1 ASC";
        var reportParams = new ReportSqlDataParametersModel
        {
            Tipo = categoryItems,
            Codigo = ReportList![SelectedDataNumber].Codigo
        };

        (bool success, var data) = await LoadReportDataFromSqlAsync(sqlQuery, reportParams, isData);
        if (success) _tableData[tableNumber] = !isData ? (Header: data, Data: null) : (_tableData[tableNumber].Header, Data: data);

        return success;
    }



    //  =============== Metodo para verificar si el informe seleccionado es valido
    private bool IsReportListValid()
    {
        if (ReportList?.Count == 0 || ReportList![SelectedDataNumber]?.Codigo == null)
        {
            ShowMessage("Seleccione informe para generar.", ControlAppearance.Danger);
            return false;
        }
        return true;
    }



    //  =============== Metodo para leer la configuracion del informe desde el JSON
    private async Task<bool> LoadReportConfigurationAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
            {
                ShowMessage("Ruta de archivo de configuración incorrecta.", ControlAppearance.Danger);
                return false;
            }
            ReportConfig = await Task.Run(() => _reportConfigurationService.LoadConfiguration(FilePath));
            return ReportConfig != null;
        }
        catch (Exception ex)
        {
            ShowMessage($"Error al cargar la configuración. Error: {ex.Message}", ControlAppearance.Danger);
            Log.Error(ex.Message);
            return false;
        }  
    }



    //  =============== Metodo para leer la informacion desde SQL
    private async Task<(bool, IEnumerable<ReportSqlDataFormattedModel>?)> LoadReportDataFromSqlAsync(string sqlQuery, object parameters, bool firstOrAll)
    {
        // Inicializa una colección vacía para los datos
        IEnumerable<ReportSqlDataFormattedModel> dataNull = new List<ReportSqlDataFormattedModel> { new ReportSqlDataFormattedModel() };

        try
        {
            // Verifica que se haya seleccionado una categoría de informe
            if (ReportList?[SelectedDataNumber].Id == null)
            {
                ShowMessage("Seleccione primero la categoría de informes", ControlAppearance.Caution);
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
            ShowMessage($"Error al cargar los datos desde SQL: {ex.Message}", ControlAppearance.Danger);
            Log.Error(ex.Message);
            return (false, dataNull);
        }

    }



    //  =============== Metodo para generar el PDF
    private async Task<bool> GeneratePdfAsync()
    {
        try
        {
            string folderPath = _appConfig.ReportSaveFolder;

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                ShowMessage("La carpeta de informes no está configurada o no existe.", ControlAppearance.Caution);
                return false;
            }

            string filePath = Path.Combine(folderPath, $"Reporte_{ReportCategory![SelectedCategoryNumber].Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            await Task.Run(() => _pdfGeneratorService.GeneratePdf(filePath, ReportConfig!, _tableData[1].Header!, _tableData[1].Data!, _tableData[2].Header!, _tableData[2].Data!,
                _tableData[3].Header!, _tableData[3].Data!, _tableData[4].Header!, _tableData[4].Data!, _tableData[5].Header!, _tableData[5].Data!));

            return true;
        }
        catch (Exception ex)
        {
            ShowMessage($"Error al generar el PDF: {ex.Message}", ControlAppearance.Danger);
            Log.Error(ex.Message);
            return false;
        }
        
    }



    //  =============== Metodo para leer las categorias de informes
    private async Task LoadCategoriesFromSql()
    {
        try
        {
            var sqlQuery = "SELECT * FROM ZC_INFORME_TIPO WHERE Visible_Individual = 1 ORDER BY Id ASC";
            IEnumerable<ReportSqlCategoryFormattedModel> reportCategory = await _reportSqlService.GetReportCategoryAsync(sqlQuery);
            ReportCategory = new ObservableCollection<ReportSqlCategoryFormattedModel>(reportCategory);
        }
        catch (Exception ex)
        {
            ShowMessage($"Error al cargar las categorías de informes: {ex.Message}", ControlAppearance.Danger);
            Log.Error(ex.Message);
        }
    }



    //  =============== Metodo para leer la lista de informes por categoria y fecha
    private async Task LoadReportListFromSql()
    {
        try
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
                                AND FECHA_1 >= @DateStart
                                AND FECHA_1 <= @DateEnd
                                ORDER BY Fecha_1, Hora_1 ASC";

                // Parámetros para la consulta
                var parameters = new
                {
                    ReportCategory?[SelectedCategoryNumber].Id,
                    DateStart = SelectedDateStart!.Value.ToString("yyyy-MM-dd"), // Asegura el formato correcto
                    DateEnd = SelectedDateEnd!.Value.ToString("yyyy-MM-dd") // Asegura el formato correcto
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
                    // Vaciamos la lista en caso de que se haya generado.
                    ReportList?.Clear();

                    // Mostrar mensaje si no hay registros
                    ShowMessage("No hay registros para la fecha seleccionada", ControlAppearance.Caution);
                }
            
            
        }
        catch (Exception ex)
        {
            ShowMessage($"Error al cargar la lista de informes: {ex.Message}", ControlAppearance.Danger);
            Log.Error(ex.Message);
        }
    }




    //  =============== Metodo para mostrar mensajes
    private void ShowMessage(string error, ControlAppearance appearance)
    {
        _snackbarService.Show("Informe individual", error, appearance, TimeSpan.FromSeconds(1));
        
    }


}


