using System.Windows.Controls;
using ZC_Informes.ViewModels.Pages;

namespace ZC_Informes.Views.Pages
{
    /// <summary>
    /// Lógica de interacción para SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {

        public SettingsPage()
        {            
            InitializeComponent();

            DataContext = new SettingsPageViewModel();
        }      

    }
}
