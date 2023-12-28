using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            Closing += MainWindow_Closing;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Sprawdź, czy ViewModel implementuje IDisposable, a następnie wywołaj Dispose
            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void TextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel)?.BrowseSourceCommand.Execute(sender);
        }

        private void TextBox_MouseDoubleClick_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel)?.BrowseDestinationCommand.Execute(sender);
        }
    }
}