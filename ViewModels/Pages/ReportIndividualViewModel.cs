﻿using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using ZC_Informes;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;


public partial class ReportIndividualViewModel : ObservableObject
{


    // =============== Variables o propiedades para almacenar los datos
    private IEnumerable<ReportSqlDataModel>? _table1HeaderSql;
    private IEnumerable<ReportSqlDataModel>? _table1DataSql;
    private IEnumerable<ReportSqlDataModel>? _table2HeaderSql;
    private IEnumerable<ReportSqlDataModel>? _table2DataSql;
    private IEnumerable<ReportSqlDataModel>? _table3HeaderSql;
    private IEnumerable<ReportSqlDataModel>? _table3DataSql;
    private IEnumerable<ReportSqlDataModel>? _table4HeaderSql;
    private IEnumerable<ReportSqlDataModel>? _table4DataSql;
    private IEnumerable<ReportSqlDataModel>? _table5HeaderSql;
    private IEnumerable<ReportSqlDataModel>? _table5DataSql;


    //  =============== Servicios inyectados
    private readonly IReportConfigurationService _reportConfigurationService;
    private readonly IPdfGeneratorService _pdfGeneratorService;
    private readonly ISnackbarService _snackbarService;
    private readonly IReportSqlService _reportSqlService;



    //  =============== Propiedades observables
    [ObservableProperty]
    private string? filePath = string.Empty;
    [ObservableProperty]
    private ReportConfigurationModel? reportConfig;
    [ObservableProperty]
    private DateTime? selectedDate;
    [ObservableProperty]
    private ObservableCollection<ReportSqlCategoryModel>? reportCategory;
    [ObservableProperty]
    private ObservableCollection<ReportSqlReportList>? reportList;
    [ObservableProperty]
    private int selectedReportCategoryId;
    [ObservableProperty]
    private ReportSqlReportList? selectedReportDataId;



    //  =============== Comandos
    public IRelayCommand LoadReportListCommand { get; }
    public IRelayCommand ShowReportListId { get; }



    //  =============== Constructor
    public ReportIndividualViewModel()
    {

        if (App.ServiceProvider == null) throw new ArgumentNullException(nameof(App.ServiceProvider));

        _reportConfigurationService = App.ServiceProvider.GetRequiredService<IReportConfigurationService>();
        _pdfGeneratorService = App.ServiceProvider.GetRequiredService<IPdfGeneratorService>();
        _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();
        _reportSqlService = App.ServiceProvider.GetRequiredService<IReportSqlService>();

        LoadReportListCommand = new AsyncRelayCommand(LoadReportList);
        ShowReportListId = new AsyncRelayCommand(GenerateAndPrintReport);

        SelectedDate = DateTime.Today;

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
            if (SelectedReportDataId != null)
            {

                Log.Information($"Inicio generar informe: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");

                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\Report\", $"{SelectedReportCategoryId}.json");

                //  2. Leer el archivo de configuración de manera asíncrona
                bool LoadconfigurationResult = false;
                LoadconfigurationResult = await LoadReportConfigurationAsync();

                if (!LoadconfigurationResult)
                {
                    ShowError("Error al leer el archivo de configuración", ControlAppearance.Danger);
                    return;
                }

                //  3. Lanzar consultas SQL de forma asíncrona
                //  Leemos los datos de la tabla General
                bool sqlResult = false;
                (sqlResult, _table1HeaderSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.Table1.Configuration.HeaderCategory} AND ID = '{SelectedReportDataId.Id}'", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table1DataSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME", true);
                //(sqlResult, _tableGeneralDataSqlData) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.TableGeneral.Configuration.DataCategory}", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table2HeaderSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.Table1.Configuration.HeaderCategory} AND ID = '{SelectedReportDataId.Id}'", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table2DataSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME", true);
                //(sqlResult, _tableGeneralDataSqlData) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.TableGeneral.Configuration.DataCategory}", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table3HeaderSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.Table1.Configuration.HeaderCategory} AND ID = '{SelectedReportDataId.Id}'", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table3DataSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME", true);
                //(sqlResult, _tableGeneralDataSqlData) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.TableGeneral.Configuration.DataCategory}", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table4HeaderSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.Table1.Configuration.HeaderCategory} AND ID = '{SelectedReportDataId.Id}'", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table4DataSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME", true);
                //(sqlResult, _tableGeneralDataSqlData) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.TableGeneral.Configuration.DataCategory}", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table5HeaderSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.Table1.Configuration.HeaderCategory} AND ID = '{SelectedReportDataId.Id}'", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }

                (sqlResult, _table5DataSql) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME", true);
                //(sqlResult, _tableGeneralDataSqlData) = await LoadReportDataFromSqlAsync($"SELECT * FROM ZC_INFORME WHERE ID_CATEGORIA = {ReportConfig.TableGeneral.Configuration.DataCategory}", false);
                if (!sqlResult)
                {
                    ShowError("Error al leer los datos desde SQL.", ControlAppearance.Danger);
                    return;
                }


                //  4. Generar informe PDF de forma asíncrona
                bool informeResult = await GeneratePdfAsync();

                if (!informeResult)
                {
                    ShowError("Error al generar el informe PDF", ControlAppearance.Danger);
                    return;
                }

                ShowError("Operación completada con éxito", ControlAppearance.Success);
                Log.Information($"Fin generar informe: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            }
            else
            {
                ShowError("Seleccione informe para generar.", ControlAppearance.Danger);
            }
            
        }
        catch (Exception ex)
        {
            ShowError($"Ocurrió un error inesperado: {ex.Message}",ControlAppearance.Danger);
            Log.Information(ex.Message);
        }
    }



    //  =============== Metodo para mostrar error al generar PDF
    private void ShowError(string error, ControlAppearance appearance)
    {
        _snackbarService.Show("Informe individual", error, appearance, TimeSpan.FromSeconds(1));
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
    private async Task<(bool, IEnumerable<ReportSqlDataModel>?)> LoadReportDataFromSqlAsync(string sqlQuery, bool firstOrAll)
    {
        // Inicializa una colección vacía para los datos
        IEnumerable<ReportSqlDataModel> dataNull = Enumerable.Empty<ReportSqlDataModel>();

        try
        {
            // Verifica que se haya seleccionado una categoría de informe
            if (SelectedReportCategoryId == 0)
            {
                ShowError("Seleccione primero la categoría de informes", ControlAppearance.Caution);
                return (false, dataNull);
            }

            // Ejecutar consulta con Dapper
            IEnumerable<ReportSqlDataModel> reportData = await _reportSqlService.GetReportDataAsync(sqlQuery);

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
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss"); // Formato: YYYYMMDD_HHMMSS
            string fileName = $"Reporte_{timestamp}.pdf"; // Nombre del archivo
            string filePath = Path.Combine(documentsFolder, fileName); // Combina la ruta del directorio con el nombre del archivo

            // Ejecutar la carga de configuración en un hilo en segundo plano
            await Task.Run(() => _pdfGeneratorService.GeneratePdf(filePath, 
                ReportConfig,
                _table1HeaderSql,
                _table1DataSql,
                _table2HeaderSql,
                _table2DataSql,
                _table3HeaderSql,
                _table3DataSql,
                _table4HeaderSql,
                _table4DataSql,
                _table5HeaderSql,
                _table5DataSql));


            return true;
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
            var sqlQuery = "SELECT * FROM ZC_INFORME_CATEGORIA";
            IEnumerable<ReportSqlCategoryModel> reportCategory = await _reportSqlService.GetReportCategoryAsync(sqlQuery);
            ReportCategory = new ObservableCollection<ReportSqlCategoryModel>(reportCategory);
        }
        catch (Exception ex)
        {
            ShowError($"Error al cargar las categorias de informes: {ex.Message}",ControlAppearance.Danger);
            Log.Information(ex.Message);
        }     
    }



    //  =============== Metodo para leer la lista de informes por categoria y fecha
    private async Task LoadReportList()
    {
        try
        {
            if (SelectedReportCategoryId != 0)
            {
                var sqlQuery = @"
                                SELECT 
                                    id, 
                                    CONCAT(CONVERT(varchar, FECHA_1, 23), ' - ', CONVERT(varchar, HORA_1, 8)) AS Titulo 
                                FROM 
                                    ZC_INFORME 
                                WHERE 
                                    ID_CATEGORIA = @SelectedReportCategoryId 
                                    AND FECHA_1 = @SelectedDate";


                // Parámetros para la consulta
                var parameters = new
                {
                    SelectedReportCategoryId,
                    SelectedDate = SelectedDate.Value.ToString("yyyy-MM-dd") // Asegura el formato correcto
                };

                // Ejecutar consulta con Dapper
                IEnumerable<ReportSqlReportList> reportData = await _reportSqlService.GetReportListAsync(sqlQuery, parameters);

                // Verificar si hay registros en la consulta
                if (reportData.Any()) // Si hay registros
                {
                    ReportList = new ObservableCollection<ReportSqlReportList>(reportData);
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



}


