using CommunityToolkit.Mvvm.ComponentModel;

namespace ZC_Informes.Models
{
    /// <summary>
    /// Clase que representa la configuración de la aplicación.
    /// Implementa la clase ObservableObject para facilitar la
    /// notificación de cambios en las propiedades.
    /// </summary>
    public partial class AppConfigModel : ObservableObject
    {

        [ObservableProperty]
        private string reportSaveFolder = string.Empty;
        [ObservableProperty]
        private string reportConfigFolder = string.Empty;
        [ObservableProperty]
        private string dataSource = string.Empty;
        [ObservableProperty]
        private string initialCatalog = string.Empty;
        [ObservableProperty]
        private bool enableReportIndividual;
        [ObservableProperty]
        private bool enableReportBetweenDates;
        [ObservableProperty]
        private bool enableProductionSheet;

    }
}
