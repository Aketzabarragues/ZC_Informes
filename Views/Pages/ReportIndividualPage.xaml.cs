using System.Windows.Controls;
using ZC_Informes.ViewModels.Pages;


namespace ZC_Informes.Views.Pages
{
    /// <summary>
    /// Lógica de interacción para ReportIndividual.xaml
    /// </summary>
    public partial class ReportIndividualPage : Page
    {
        public ReportIndividualPage()
        {
            InitializeComponent();

            DataContext = new ReportIndividualViewModel();
        }
    }
}
