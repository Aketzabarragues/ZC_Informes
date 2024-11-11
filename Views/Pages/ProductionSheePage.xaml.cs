using System.Windows.Controls;

namespace ZC_Informes.Views.Pages
{
    /// <summary>
    /// Lógica de interacción para ProductionSheet.xaml
    /// </summary>
    public partial class ProductionSheetPage : Page
    {
        public ProductionSheetPage()
        {
            InitializeComponent();

            DataContext = new ProductionSheetViewModel();
        }
    }
}
