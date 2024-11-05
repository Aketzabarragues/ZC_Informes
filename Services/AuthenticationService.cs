using System.ComponentModel;
using System.Timers;
using Timer = System.Timers.Timer;


namespace ZC_Informes.Services
{

    public class AuthenticationService : INotifyPropertyChanged
    {

        // =============== Variables o propiedades para almacenar los datos
        private bool _isAuthenticated = false;        
        private readonly Timer _logoutTimer;
        private readonly double _timeoutPeriod = 5 * 60 * 1000;



        //  =============== Estado de autenticación con notificación de cambios
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



        //  =============== Constructor
        public AuthenticationService()
        {
            _logoutTimer = new Timer(_timeoutPeriod)
            {
                AutoReset = false // El temporizador no se reinicia automáticamente
            };
            _logoutTimer.Elapsed += OnLogoutTimerElapsed;
        }



        //  =============== Evento para notificar cambios en las propiedades
        public event PropertyChangedEventHandler? PropertyChanged;



        //  =============== Metodo que llama al evento cuando una propiedad cambia
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        //  =============== Valida la contraseña y actualiza los estados de UI
        public bool ValidatePassword(string password)
        {
            const string correctPassword = "zciiyc";

            if (password == correctPassword)
            {
                IsAuthenticated = true;
                return true;
            }

            return false;
        }



        //  =============== Metodo para cerrar sesion
        public void Logout()
        {
            IsAuthenticated = false;
        }



        //  =============== Metodo para iniciar el temporizador de cierre de sesión
        public void StartLogoutTimer()
        {
            if (IsAuthenticated)
            {
                _logoutTimer.Start();
            }
        }



        //  =============== Metodo para detener el temporizador de cierre de sesión
        public void StopLogoutTimer()
        {
            _logoutTimer.Stop();
        }



        //  =============== Metodo para cuando el temporizador se completa (cierra sesión)
        private void OnLogoutTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            IsAuthenticated = false;
            _logoutTimer.Stop();
        }

    }
}
