using System.Windows.Controls;
using ZC_Informes.ViewModels.Pages;

namespace ZC_Informes.Views.Pages
{

    public partial class SettingsPage : Page
    {

        private readonly SettingsPageViewModel _viewModel; // Cambiamos a un campo

        public SettingsPage()
        {            
            InitializeComponent();

            _viewModel = new SettingsPageViewModel(); // Instanciamos el ViewModel
            DataContext = _viewModel;

            // Suscribirse al evento Unloaded
            this.Unloaded += SettingsPage_Unloaded;
        }

        private void SettingsPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Llama al método Dispose en el ViewModel
            _viewModel.Dispose();
        }

    }
}
