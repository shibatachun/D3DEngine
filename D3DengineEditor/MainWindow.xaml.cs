using D3DengineEditor.GameProject;
using System.ComponentModel;
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

namespace D3DengineEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
           Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnMainWindowLoaded;
            OpenProjectBrowsewrDialog();
        }

        private void OpenProjectBrowsewrDialog()
        {
            var projectBrowswer = new ProjectBrowserDialog();
            if(projectBrowswer.ShowDialog() == false || projectBrowswer.DataContext ==null)
            {
                Application.Current.Shutdown();

            }
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowswer.DataContext;
            }
        }
    }
}