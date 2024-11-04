using CommunityToolkit.Mvvm.ComponentModel;

namespace ZC_Informes.Models
{

    public partial class AppConfigModel : ObservableObject
    {

        [ObservableProperty] private string reportSaveFolder = string.Empty;
        [ObservableProperty] private bool enableReportIndividual;
        [ObservableProperty] private bool enableReportBetweenDates;
        [ObservableProperty] private bool enableProductionSheet;


        // Propiedades de conexión a base de datos
        [ObservableProperty] private string dataSource = string.Empty;
        [ObservableProperty] private string initialCatalog = string.Empty;
        [ObservableProperty] private string userId = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private bool trustServerCertificate = false;


        // Método para construir la cadena de conexión desde las propiedades
        public string GetConnectionString()
        {
            return $"Server={DataSource};Database={InitialCatalog};User Id={UserId};Password={Password};TrustServerCertificate={TrustServerCertificate};";
        }
    }
}
