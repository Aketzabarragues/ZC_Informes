using System.ComponentModel;
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
    public partial class SettingsPageViewModel : ObservableObject, IDisposable
    {

        //  =============== Servicios inyectados
        private readonly ConfigurationService _configurationService;
        private readonly IContentDialogService _contentDialogService;
        private readonly ISnackbarService _snackbarService;
        private readonly AuthenticationService _authenticationService;
        


        //  =============== Propiedades observables
        [ObservableProperty] private AppConfigModel? _appConfig;
        [ObservableProperty] private bool isAuthenticated;

        //  =============== Comandos
        public IRelayCommand SaveCommand { get; }



        //  =============== Constructor
        public SettingsPageViewModel()
        {
            // Inyectar los servicios desde el contenedor de servicios
            _configurationService = App.ServiceProvider.GetRequiredService<ConfigurationService>();
            _contentDialogService = App.ServiceProvider.GetRequiredService<IContentDialogService>();
            _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();
            _authenticationService = App.ServiceProvider.GetRequiredService<AuthenticationService>();
            _authenticationService.PropertyChanged += OnAuthenticationServicePropertyChanged;
            _appConfig = App.ServiceProvider.GetRequiredService<AppConfigModel>();

            LoadConfiguration();

            if (IsAuthenticated)
            {
                _authenticationService.StopLogoutTimer();
            }

            // Inicializar el comando de guardar
            SaveCommand = new RelayCommand(async () => await SaveConfiguration());

        }



        private void OnAuthenticationServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AuthenticationService.IsAuthenticated))
            {
                IsAuthenticated = _authenticationService.IsAuthenticated;
            }

            if (IsAuthenticated) 
            {
                _authenticationService.StopLogoutTimer();
            }

        }



        //  =============== Metodo para cargar la configuración de la aplicación
        private void LoadConfiguration()
        {
            var loadedConfig = _configurationService.LoadConfiguration();
            AppConfig?.UpdateFrom(loadedConfig);
            IsAuthenticated = _authenticationService.IsAuthenticated;
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
                    _configurationService.SaveConfiguration(AppConfig!);

                    // Mostrar Snackbar de éxito
                    _snackbarService.Show("Ajustes globales", "Se ha guardado correctamente.", ControlAppearance.Success, TimeSpan.FromSeconds(2));
                    Log.Information("Ajustes globales. Se ha guardado correctamente.");
                    LoadConfiguration();
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



        //  =============== Metodo al cerrar la imagen, volver a iniciar el temporizador
        public void Dispose()
        {
            _authenticationService.StartLogoutTimer();
        }

    }
}

