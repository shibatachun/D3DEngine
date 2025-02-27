using D3DengineEditor.GameProject;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace D3DengineEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    //这里是partial class是因为要结合MainWindow.xaml，这两个结合才是一个完整的MainWindow类，所以使用partial
    public partial class MainWindow : Window
    {

        public static string D3DPath { get; private set; } = @"E:\Schoolwork\D3Dengine\D3DEngine";
        //开始加载组件
        public MainWindow()
        {
            //初始化组件
            InitializeComponent();
            // TORECORD: Loaed是一个event, 在初始化之后会被调用, 使用+=因为可以把+=后面的东西附加给这个event以此来执行，后面的东西需要后面的方法和函数要和前面的事件相同的签名（参数类型和返回类型）
            Loaded += OnMainWindowLoaded;
            //关闭时要发生的一个东西
            Closing += OnMainWindowClosing;
        }

       

        //在关闭window使用的方法，需注意要跟Closing的接受参数相同
        private void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
            //TORECORD: 通过取消事件订阅来避免重复触发相同的方法，因为我们一开始已经进来这个方法了，所以为了避免多次触发这个方法，我们第一步就要取消这个事件订阅。然后执行后面的操作。
           Closing -= OnMainWindowClosing;
            //进行Project的一个方法调用，用来重设RedoUndo list
            Project.Current?.Unload();
        }

        //同上
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            //同上
            this.Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            //在开始的时候，我们选择打开projectBrowser,这个是下面定义的一个方法。
            OpenProjectBrowserDialog();
        }

        private void GetEnginePath()
        {
            var enginePath = Environment.GetEnvironmentVariable("D3D_ENGINE",EnvironmentVariableTarget.User);
            if (enginePath == null || !Directory.Exists(Path.Combine(enginePath, @"D3DEngine\EngineAPI")))
            {
                var dlg = new EnginePathDialog();
                if (dlg.ShowDialog() == true)
                {
                    D3DPath = dlg.D3DPath;
                    Environment.SetEnvironmentVariable("D3D_ENGINE",D3DPath.ToUpper(), EnvironmentVariableTarget.User);
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                D3DPath = enginePath;
            }
        }

        //自定方法，目的是创建一个新的ProjectBrowserDialog实例，是一个window wpf。
        private void OpenProjectBrowserDialog()
        {
            //新建ProjectBrowserDialog实例
            
            var projectBrowser = new ProjectBrowserDialog();

            //使用projectBroswer的ShowDialog打开一个新的window窗口，根据其返回值判断接下来的操作
            //这里的打开ShowDialog是一个线程阻塞的函数，他会持续等待用户进行输入，完了之后才回进行返回，这里多加了个判断DataContext是否为空的，如果在project browser中没有任何绑定的ViewModel， 也就是context的话也会关闭当前程序
            if(projectBrowser.ShowDialog() == false || projectBrowser.DataContext ==null)
            {
                Application.Current.Shutdown();

            }
            else
            {
                //打开项目操作之前会进行一个Unload，目前只是重设UndoRedo list
                Project.Current?.Unload();
                Debug.WriteLine($"DataContext is: {projectBrowser.DataContext}");
                //把从projectBrowser得到的DataContext传回给MainWindow的DataCoantext，这样就可以在MainWindow展示相应的ViewModel的内容了
                DataContext = projectBrowser.DataContext;
            }
        }
    }
}