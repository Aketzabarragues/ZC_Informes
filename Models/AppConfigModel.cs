using CommunityToolkit.Mvvm.ComponentModel;

namespace ZC_Informes.Models
{

    public partial class AppConfigModel : ObservableObject
    {

        //  =============== Propiedades observables
        [ObservableProperty] private string reportSaveFolder = string.Empty;
        [ObservableProperty] private bool enableReportIndividual;
        [ObservableProperty] private bool enableReportBetweenDates;
        [ObservableProperty] private bool enableProductionSheet;
        [ObservableProperty] private string dataSource = string.Empty;
        [ObservableProperty] private string initialCatalog = string.Empty;
        [ObservableProperty] private string userId = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private bool trustServerCertificate = false;



        //  =============== Método para construir la cadena de conexión desde las propiedades
        public string GetConnectionString()
        {
            return $"Server={DataSource};Database={InitialCatalog};User Id={UserId};Password={Password};TrustServerCertificate={TrustServerCertificate};";
        }



        //  =============== Método para actualizar el servicio inyectado
        public void UpdateFrom(AppConfigModel newConfig)
        {
            if (newConfig == null) throw new ArgumentNullException(nameof(newConfig));

            
            this.ReportSaveFolder = newConfig.ReportSaveFolder;
            this.EnableReportIndividual = newConfig.EnableReportIndividual;
            this.EnableReportBetweenDates = newConfig.EnableReportBetweenDates;
            this.EnableProductionSheet = newConfig.EnableProductionSheet;

            this.DataSource = newConfig.DataSource;
            this.InitialCatalog = newConfig.InitialCatalog;
            this.UserId = newConfig.UserId;
            this.Password = newConfig.Password;
            this.TrustServerCertificate = newConfig.TrustServerCertificate;
        }

    }
}
