using CommunityToolkit.Mvvm.ComponentModel;

namespace ZC_Informes.Models
{

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
