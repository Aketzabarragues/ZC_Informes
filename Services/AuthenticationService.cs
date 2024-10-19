using System.ComponentModel;
using System.Timers;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using Timer = System.Timers.Timer;


namespace ZC_Informes.Services
{

    public class AuthenticationService : INotifyPropertyChanged
    {

        // Variables que controlan el estado de autenticación y la visibilidad de UI
        private bool _isAuthenticated = false;



        // Temporizador para el cierre de sesión
        private readonly Timer _logoutTimer;



        // Tiempo de espera antes de cerrar sesión (5 minutos)
        private readonly double _timeoutPeriod = 5 * 60 * 1000;



        // Estado de autenticación con notificación de cambios
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            private set
            {
                if (_isAuthenticated != value)
                {
                    _isAuthenticated = value;
                    OnPropertyChanged(nameof(IsAuthenticated));
                }
            }
        }



        // Constructor que inicializa el temporizador y sus eventos
        public AuthenticationService()
        {
            _logoutTimer = new Timer(_timeoutPeriod)
            {
                AutoReset = false // El temporizador no se reinicia automáticamente
            };
            _logoutTimer.Elapsed += OnLogoutTimerElapsed;
        }



        // Evento para notificar cambios en las propiedades
        public event PropertyChangedEventHandler? PropertyChanged;



        // Llama al evento cuando una propiedad cambia
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        // Valida la contraseña y actualiza los estados de UI
        public bool ValidatePassword(string password)
        {
            const string correctPassword = "zciiyc"; // Contraseña correcta

            if (password == correctPassword)
            {
                IsAuthenticated = true; // Actualiza el estado a autenticado
                return true;
            }

            return false; // Contraseña incorrecta
        }



        // Inicia el temporizador de cierre de sesión
        public void StartLogoutTimer()
        {
            if (IsAuthenticated)
            {
                _logoutTimer.Start(); // Solo iniciar si está autenticado
            }
        }



        // Detiene el temporizador de cierre de sesión
        public void StopLogoutTimer()
        {
            _logoutTimer.Stop(); // Detiene el temporizador
        }



        // Evento cuando el temporizador se completa (cierra sesión)
        private void OnLogoutTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            IsAuthenticated = false; // Cambia el estado a no autenticado
            _logoutTimer.Stop(); // Detiene el temporizador (aunque debería estar parado ya)
        }

    }
}
