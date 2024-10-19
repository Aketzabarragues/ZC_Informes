using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using ZC_Informes.Models;
using ZC_Informes.Services;


namespace ZC_Informes.ViewModels.Pages
{
    public partial class SettingsPageViewModel : ObservableObject
    {

        //  =============== Servicios inyectados
        private readonly ConfigurationService _configurationService;
        private readonly IContentDialogService _contentDialogService;
        private readonly ISnackbarService _snackbarService;



        //  =============== Propiedades observables
        [ObservableProperty]
        private AppConfigModel? _appConfig;



        //  =============== Comandos
        public IRelayCommand SaveCommand { get; }



        //  =============== Constructor
        public SettingsPageViewModel()
        {
            // Inyectar los servicios desde el contenedor de servicios
            _configurationService = App.ServiceProvider.GetRequiredService<ConfigurationService>();
            _contentDialogService = App.ServiceProvider.GetRequiredService<IContentDialogService>();
            _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();

            // Cargar la configuración inicial
            LoadConfiguration();

            // Inicializar el comando de guardar
            SaveCommand = new RelayCommand(async () => await SaveConfiguration());
        }



        //  =============== Metodo para cargar la configuración de la aplicación
        private void LoadConfiguration()
        {
            AppConfig = _configurationService.LoadConfiguration();
        }



        //  =============== Metodo para guardar la configuración y manejar el resultado del diálogo
        private async Task SaveConfiguration()
        {
            var result = await _contentDialogService.ShowAsync(
                new ContentDialog
                {
                    Title = "Guardar ajustes",
                    Content = "¿Estás seguro de que quieres guardar los ajustes?",
                    PrimaryButtonText = "Aceptar",
                    SecondaryButtonText = "Cancelar",
                    CloseButtonText = "Cerrar"
                }, default);

            // Manejar la respuesta del diálogo
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    // Guardar la configuración si se presiona "Aceptar"
                    _configurationService.SaveConfiguration((AppConfigModel?)AppConfig);

                    // Mostrar Snackbar de éxito
                    _snackbarService.Show("Ajustes globales", "Se ha guardado correctamente.", ControlAppearance.Success, TimeSpan.FromSeconds(2));
                    Log.Information("Ajustes globales. Se ha guardado correctamente.");
                }
                catch (Exception ex)
                {
                    // Mostrar error si algo falla
                    _snackbarService.Show("Ajustes globales", $"Error al guardar: {ex.Message}", ControlAppearance.Danger, TimeSpan.FromSeconds(2));
                    Log.Error($"Ajustes globales. Error al guardar: {ex.Message}.");

                    // Recargar la configuración anterior si ocurre un error
                    LoadConfiguration();
                }
            }
            else
            {
                // Si el usuario cancela, recargar la configuración original
                LoadConfiguration();
            }
        }

    }
}

