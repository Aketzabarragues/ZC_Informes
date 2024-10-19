using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using ZC_Informes.ViewModels.Windows;

namespace ZC_Informes.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {               

        public MainWindow()
        {
            InitializeComponent();
            ApplicationThemeManager.Apply(this);
            DataContext = new MainWindowViewModel(RootNavigation, ContentPresenterForDialogs, SnackbarPresenter);
        }

    }
}
