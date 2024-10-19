using Serilog;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ZC_Informes.Services;
using ZC_Informes.Models;
using Wpf.Ui;
using ZC_Informes.Interfaces;

namespace ZC_Informes
{

    public partial class App : Application
    {


        // Contenedor de servicios
        public static IServiceProvider? ServiceProvider { get; private set; }
        private static Mutex? _mutex;


        public App()
        {
            // Configuración básica de Serilog para guardar logs en un archivo
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Nivel de logs
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Guardar logs en archivos diarios
                .CreateLogger();
        }



        protected override void OnStartup(StartupEventArgs e)
        {

            const string appName = "ZC_Informes";
            bool createdNew;
            _mutex = new Mutex(true, appName, out createdNew);
            if (!createdNew)
            {
                Application.Current.Shutdown();
            }


            // Configurar el contenedor de servicios
            var serviceCollection = new ServiceCollection();

            // Registrar servicios e interfaces
            serviceCollection.AddSingleton<AuthenticationService>();
            serviceCollection.AddSingleton<ConfigurationService>();
            serviceCollection.AddSingleton<ISnackbarService, SnackbarService>();
            serviceCollection.AddSingleton<IContentDialogService, ContentDialogService>();
            serviceCollection.AddSingleton<IPdfGeneratorService, PdfGeneratorService>();
            serviceCollection.AddSingleton<IReportConfigurationService>(provider =>
            {
                var filePath = @"C:\Informes\Config\1000.json";
                var snackbarService = provider.GetRequiredService<ISnackbarService>();
                return new ReportConfigurationService(filePath, snackbarService);
            });
            serviceCollection.AddSingleton<IReportSqlService, ReportSqlService>();

            serviceCollection.AddSingleton<AppConfigModel>();
            serviceCollection.AddSingleton<ReportConfigurationModel>();

            // Construir el proveedor de servicios
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Registramos un mensaje al iniciar la aplicación
            Log.Information("La aplicación ha iniciado.");

            base.OnStartup(e);

        }



        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Registramos un mensaje al cerrar la aplicación
            Log.Information("La aplicación ha cerrado.");
            Log.CloseAndFlush();
        }

    }

}