using System.Collections.ObjectModel;
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

    //  =============== Servicios inyectados
    private readonly IReportConfigurationService _reportConfigurationService;
    private readonly IPdfGeneratorService _pdfGeneratorService;
    private readonly ISnackbarService _snackbarService;
    private readonly IReportSqlService _reportSqlService;



    //  =============== Propiedades observables
    [ObservableProperty]
    private string filePath = @"C:\Informes\Config\1000.json";
    [ObservableProperty]
    private ReportConfigurationModel reportConfig;
    [ObservableProperty]
    private DateTime? selectedDate;
    [ObservableProperty]
    private ObservableCollection<ReportSqlCategoryModel> reportCategory;
    [ObservableProperty]
    private ObservableCollection<ReportSqlReportList> reportList;
    [ObservableProperty]
    private int selectedReportCategoryId;
    [ObservableProperty]
    private ReportSqlReportList selectedReportDataId;



    //  =============== Comandos
    public IRelayCommand LoadReportListCommand { get; }
    public IRelayCommand ShowReportListId { get; }



    //  =============== Constructor
    public ReportIndividualViewModel()
    {
        _reportConfigurationService = App.ServiceProvider.GetRequiredService<IReportConfigurationService>();
        _pdfGeneratorService = App.ServiceProvider.GetRequiredService<IPdfGeneratorService>();
        _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();
        _reportSqlService = App.ServiceProvider.GetRequiredService<IReportSqlService>();

        LoadReportListCommand = new AsyncRelayCommand(LoadReportList);
        ShowReportListId = new RelayCommand(GenerateAndPrintReport);

        SelectedDate = DateTime.Today;

        LoadCategoriesFromSQL();
    }



    //  =============== Metodo para generar e imprimir un informe
    /*
     * Para generar un informe, tenemos que seguir los siguientes pasos:
     *      1:   Revisamos si hay item en el listview seleccionado
     *      2:   Cargamos el archivo de configuracion JSON y comprobamos si es correcto
     *      3:   Realizamos la consulta a la base de datos para traer todos los datos necesarios para el informe
     *      4:   Generamos el informe
    */
    private void GenerateAndPrintReport()
    {
        try
        {
            if (SelectedReportDataId != null)
            {
                _snackbarService.Show("Informe individual", $"Valor ID seleccionado: {SelectedReportDataId.Id}", ControlAppearance.Success, TimeSpan.FromSeconds(1));
                LoadReportConfiguration();
                LoadReportDataFromSql();
                GeneratePdf();

            }
            else
            {
                _snackbarService.Show("Informe individual", "No se ha seleccionado ningún registro.", ControlAppearance.Danger, TimeSpan.FromSeconds(1));
            }
            
        }
        catch (Exception ex)
        {
            _snackbarService.Show("Informe individual", "Ocurrió un error inesperado.", ControlAppearance.Danger, TimeSpan.FromSeconds(1));
            Log.Information(ex.Message);
        }
    }



    //  =============== Metodo para leer la informacion desde SQL
    private void LoadReportDataFromSql()
    {
    }



    //  =============== Metodo para generar el PDF
    private void GeneratePdf()
    {
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
            System.Windows.MessageBox.Show(ex.Message);
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
                    SelectedReportCategoryId = SelectedReportCategoryId,
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
                    // Mostrar mensaje si no hay registros
                    _snackbarService.Show("Informe individual", "No hay registros para la fecha seleccionada", ControlAppearance.Caution, TimeSpan.FromSeconds(1));
                }

            }
            else
            {
                _snackbarService.Show("Informe individual", "Seleccione primero la categoria de informes", ControlAppearance.Caution, TimeSpan.FromSeconds(1));
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message);
            Log.Information(ex.Message);
        }
    }



    //  =============== Metodo para leer la configuracion del informe desde el JSON
    private void LoadReportConfiguration()
    {
        try
        {
            // Verificar si la ruta del archivo no está vacía
            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
            {
                _snackbarService.Show("Configuracion informe1", "Ruta de archivo de configuracion incorrecta.", ControlAppearance.Danger, TimeSpan.FromSeconds(2));
                return;
            }

            ReportConfig = _reportConfigurationService.LoadConfiguration();
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss"); // Formato: YYYYMMDD_HHMMSS
            string fileName = $"Reporte_{timestamp}.pdf"; // Nombre del archivo
            string filePath = Path.Combine(documentsFolder, fileName); // Combina la ruta del directorio con el nombre del archivo

            _pdfGeneratorService.GeneratePdf(filePath, ReportConfig);
        }
        catch (Exception ex)
        {
            _snackbarService.Show("Configuracion informe", $"Error al cargar la configuracion. Error: {ex.Message}", ControlAppearance.Danger, TimeSpan.FromSeconds(2));
            Log.Information(ex.Message);
        }
    }



}


