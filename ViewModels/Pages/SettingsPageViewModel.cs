using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using ZC_Informes.Helpers;
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
        [ObservableProperty] public bool enableSave = false;
        [ObservableProperty] public string userPassword = string.Empty;



        //  =============== Comandos
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand ChangePassword { get; }
        

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
            UserPassword = ConfigEncryptorHelper.GetUserPassword();
            LoadConfiguration();

            if (IsAuthenticated)
            {
                _authenticationService.StopLogoutTimer();
            }

            // Inicializar el comando de guardar
            SaveCommand = new RelayCommand(async () => await SaveConfiguration());
            ChangePassword = new RelayCommand(InitializePassword);

        }



        private void InitializePassword()
        {
            try
            {
                ConfigEncryptorHelper.InitializeMasterPassword(UserPassword);
                ConfigEncryptorHelper.UpdateUserPassword(UserPassword);
                _snackbarService.Show("Contraseña", "Se ha actualizado correctamente.", ControlAppearance.Success, TimeSpan.FromSeconds(2));
                Log.Information("Contraseña. Se ha actualizado correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex.Message);
                _snackbarService.Show("Contraseña", "Error al actualizar la contraseña.", ControlAppearance.Danger, TimeSpan.FromSeconds(2));
                Log.Information("Contraseña. Error al actualizar la contraseña.");
            }
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

            // Validar campos antes de guardar
            ValidateFields();

            // Mostrar diálogo de confirmación
            if (EnableSave)
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
        }



        //  =============== Metodo para validación
        private void ValidateFields()
        {
            EnableSave = !string.IsNullOrEmpty(AppConfig!.ReportSaveFolder) &&
                !string.IsNullOrEmpty(AppConfig!.DataSource) &&
                !string.IsNullOrEmpty(AppConfig!.InitialCatalog) &&
                !string.IsNullOrEmpty(AppConfig!.UserId) &&
                !string.IsNullOrEmpty(AppConfig!.Password);
        }



        //  =============== Metodo al cerrar la imagen, volver a iniciar el temporizador
        public void Dispose()
        {
            _authenticationService.StartLogoutTimer();
        }

    }
}

