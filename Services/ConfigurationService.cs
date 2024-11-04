using System.Configuration;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using ZC_Informes.Models;


namespace ZC_Informes.Services
{
    public class ConfigurationService
    {

        //  =============== Servicios inyectados
        private readonly ISnackbarService _snackbarService;



        //  =============== Constructor
        public ConfigurationService()
        {
            if (App.ServiceProvider == null) throw new ArgumentNullException(nameof(App.ServiceProvider));            
            _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();
        }



        //  =============== Metodo para cargar configuración desde AppSettings
        public AppConfigModel LoadConfiguration()
        {
            var config = new AppConfigModel();

            try
            {
                
                config.ReportSaveFolder = ConfigurationManager.AppSettings["ReportSaveFolder"] ?? string.Empty;
                config.DataSource = ConfigurationManager.AppSettings["DataSource"] ?? string.Empty;
                config.InitialCatalog = ConfigurationManager.AppSettings["InitialCatalog"] ?? string.Empty;
                config.UserId = ConfigurationManager.AppSettings["UserId"] ?? string.Empty;
                config.Password = ConfigurationManager.AppSettings["Password"] ?? string.Empty;


                // Parsear valores booleanos
                bool.TryParse(ConfigurationManager.AppSettings["TrustServerCertificate"], out bool trustServerCertificate);
                config.TrustServerCertificate = trustServerCertificate;

                bool.TryParse(ConfigurationManager.AppSettings["EnableReportIndividual"], out bool enableReportIndividual);
                config.EnableReportIndividual = enableReportIndividual;

                bool.TryParse(ConfigurationManager.AppSettings["EnableReportBetweenDates"], out bool enableReportBetweenDates);
                config.EnableReportBetweenDates = enableReportBetweenDates;

                bool.TryParse(ConfigurationManager.AppSettings["EnableProductionSheet"], out bool enableProductionSheet);
                config.EnableProductionSheet = enableProductionSheet;
            }
            catch (ConfigurationErrorsException ex)
            {
                ShowErrorSnackbar("Error al leer la configuración", ex.Message);
            }

            return config;
        }



        //  =============== Metodo para guardar configuración en AppSettings
        public void SaveConfiguration(AppConfigModel config)
        {
            try
            {
                // Acceder a la configuración de la aplicación
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Asignar los valores de configuración desde el objeto config
                AssignConfigurationValues(configuration, config);

                // Guardar la configuración modificada
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings"); // Refrescar la sección para reflejar los cambios
            }
            catch (ConfigurationErrorsException ex)
            {
                ShowErrorSnackbar("Error al guardar la configuración", ex.Message);
            }
        }



        //  =============== Metodo para asignar valores de configuración desde el objeto config
        private void AssignConfigurationValues(Configuration configuration, AppConfigModel config)
        {
            configuration.AppSettings.Settings["ReportSaveFolder"].Value = config.ReportSaveFolder;
            configuration.AppSettings.Settings["DataSource"].Value = config.DataSource;
            configuration.AppSettings.Settings["InitialCatalog"].Value = config.InitialCatalog;
            configuration.AppSettings.Settings["UserId"].Value = config.UserId;
            configuration.AppSettings.Settings["Password"].Value = config.Password;            
            configuration.AppSettings.Settings["TrustServerCertificate"].Value = config.TrustServerCertificate.ToString();
            configuration.AppSettings.Settings["EnableReportIndividual"].Value = config.EnableReportIndividual.ToString();
            configuration.AppSettings.Settings["EnableReportBetweenDates"].Value = config.EnableReportBetweenDates.ToString();
            configuration.AppSettings.Settings["EnableProductionSheet"].Value = config.EnableProductionSheet.ToString();
        }



        // Metodo para obtener la cadena de conexión a la base de datos desde las propiedades
        public string GetDatabaseConnectionString(AppConfigModel config)
        {
            return config.GetConnectionString();
        }



        //  =============== Metodo para mostrar un Snackbar de error
        private void ShowErrorSnackbar(string title, string message)
        {
            _snackbarService.Show(title, message, ControlAppearance.Danger, TimeSpan.FromSeconds(2));
            Log.Error($"{title}. {message}");
        }

    }
}
