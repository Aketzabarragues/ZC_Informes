using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using ZC_Informes.Models;
using ZC_Informes.Services;
using ZC_Informes.Views.Pages;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PasswordBox = Wpf.Ui.Controls.PasswordBox;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace ZC_Informes.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {

        //  =============== Servicios inyectados
        private readonly AuthenticationService _authenticationService;
        private readonly ConfigurationService _configurationService;
        private readonly IContentDialogService _contentDialogService;
        private readonly ISnackbarService _snackbarService;
        private AppConfigModel _appConfig;
        private readonly NavigationView _navigationView;



        //  =============== Propiedades observables
        [ObservableProperty] private Visibility _reportIndividualItemVisibility = Visibility.Visible;
        [ObservableProperty] private Visibility _reportBetweenDatesItemVisibility = Visibility.Visible;
        [ObservableProperty] private Visibility _productionSheetItemVisibility = Visibility.Visible;



        //  =============== Comandos
        public IRelayCommand CheckAuthenticationCommand { get; }
        public IRelayCommand OpenReportSaveFolderCommand { get; }


        //  =============== Constructor
        public MainWindowViewModel(NavigationView navigationView, ContentPresenter contentDialogPresenter, SnackbarPresenter snackbarPresenter)
        {

            _authenticationService = App.ServiceProvider.GetRequiredService<AuthenticationService>();
            _configurationService = App.ServiceProvider.GetRequiredService<ConfigurationService>();
            _contentDialogService = App.ServiceProvider.GetRequiredService<IContentDialogService>();
            _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();

            //  Inyectar la configuración desde el contenedor de servicios, actualizarla y cargarla
            _appConfig = App.ServiceProvider.GetRequiredService<AppConfigModel>();
            var loadedConfig = _configurationService.LoadConfiguration();
            _appConfig.UpdateFrom(loadedConfig);

            _contentDialogService.SetDialogHost(contentDialogPresenter);
            _snackbarService.SetSnackbarPresenter(snackbarPresenter);
            _navigationView = navigationView;

            CheckAuthenticationCommand = new RelayCommand(OnCheckAuthentication);
            OpenReportSaveFolderCommand = new RelayCommand(OpenReportSaveFolder);

            CargarConfiguracion();
        }



        //  =============== Metodo para guardar la configuración
        public void GuardarConfiguracion()
        {
            _configurationService.SaveConfiguration(_appConfig);
            ActualizarMenu();
        }



        //  =============== Metodo para actualizar la visibilidad del menú
        public void ActualizarMenu()
        {
            ReportIndividualItemVisibility = _appConfig.EnableReportIndividual ? Visibility.Visible : Visibility.Collapsed;
            ReportBetweenDatesItemVisibility = _appConfig.EnableReportBetweenDates ? Visibility.Visible : Visibility.Collapsed;
            ProductionSheetItemVisibility = _appConfig.EnableProductionSheet ? Visibility.Visible : Visibility.Collapsed;
        }



        //  =============== Metodo para cargar la configuración
        private void CargarConfiguracion()
        {
            _appConfig = _configurationService.LoadConfiguration();
            ActualizarMenu();
        }



        //  =============== Metodo para comprobar autenticación
        private void OnCheckAuthentication()
        {
            if (!_authenticationService.IsAuthenticated)
            { 
                ShowLogin();
            }
            else
            {
                ShowLogout();
            }
        }



        // =============== Método para abrir la carpeta de Reportes
        private void OpenReportSaveFolder()
        {
            if (!string.IsNullOrEmpty(_appConfig.ReportSaveFolder))
            {
                try
                {
                    // Intenta abrir la carpeta en la ruta especificada
                    System.Diagnostics.Process.Start("explorer.exe", _appConfig.ReportSaveFolder);
                }
                catch (Exception ex)
                {
                    // Muestra un mensaje de error si no se puede abrir la carpeta
                    _snackbarService.Show("Carpera de informes", $"Error al abrir la carpeta de informes: {ex.Message}", ControlAppearance.Danger, TimeSpan.FromSeconds(1));
                    Log.Error($"Error al abrir la carpeta de informes: {ex.Message}");
                }
            }
            else
            {
                // Muestra un mensaje si la ruta está vacía o no se ha configurado
                _snackbarService.Show("Carpera de informes", "La carpeta de informes no está configurada.", ControlAppearance.Danger, TimeSpan.FromSeconds(1));
                Log.Error("La carpeta de informes no está configurada.");
            }
        }



        //  =============== Metodo para mostrar diálogo de inicio de sesión
        private async void ShowLogin()
        {
            PasswordBox customPasswordBox = new PasswordBox { Margin = new Thickness(0, 0, 0, 10) };

            var result = await _contentDialogService.ShowAsync(
                new ContentDialog
                {
                    Title = "Iniciar sesión",
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock { Text = "Introduzca la contraseña:", Margin = new Thickness(0, 0, 0, 10) },
                            customPasswordBox
                        }
                    },
                    PrimaryButtonText = "Aceptar",
                    SecondaryButtonText = "Cancelar",                    
                    CloseButtonText = "Cerrar"
                }, default);

            if (result == ContentDialogResult.Primary)
            {
                var password = customPasswordBox.Password;
                _authenticationService.ValidatePassword(password);

                if (_authenticationService.IsAuthenticated)
                {
                   
                    _snackbarService.Show("Inicio de sesión", "Contraseña correcta. Sesión iniciada.", ControlAppearance.Success, TimeSpan.FromSeconds(2));
                    Log.Information("Inicio de sesion correcto.");
                }
                else
                {
                    _snackbarService.Show("Inicio de sesión", "Contraseña incorrecta. Inténtelo de nuevo.", ControlAppearance.Danger, TimeSpan.FromSeconds(2));
                    Log.Warning("Fallo de inicio de sesion.");
                }
            }
        }



        //  =============== Metodo para mostrar diálogo de inicio de sesión
        private async void ShowLogout()
        {
           

            var result = await _contentDialogService.ShowAsync(
                new ContentDialog
                {
                    Title = "Iniciar sesión",
                    Content = "¿Desea cerrar sesión?",
                    PrimaryButtonText = "Aceptar",
                    SecondaryButtonText = "Cancelar",
                    CloseButtonText = "Cerrar"
                }, default);

            if (result == ContentDialogResult.Primary)
            {
                _authenticationService.Logout();               
            }
        }



        //  =============== Metodo para mostrar página de configuración
        private void ShowSettingsPage()
        {
            _navigationView.Navigate(typeof(SettingsPage)); // Cargar SettingsPage
        }

    }
}
