using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using D3DengineEditor.GameProject;


namespace D3DengineEditor.GameProject
{
    /// <summary>
    /// NewProjectView.xaml 的交互逻辑
    /// </summary>
    public partial class NewProjectView : UserControl
    {
        public NewProjectView()
        {
            InitializeComponent();

        }

        //绑定点击事件
        private void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            //因为在xaml文件中绑定了new project作为DataContext, 在WPF加载和解析Xaml文件的时候，遇到<local:xxxx/>,会自动实例化New project,因此在这个阶段，new project构造器里的代码会被执行。 这里是将DataContext进行类型转换成new project类。
            var vm = DataContext as NewProject;
            //因为这里已经绑定了New project，我们可以用来进行数据获取和交换了，这里利用了vm里面的new project data context里面create project方法来进行创建project
            var projectPath = vm.CreateProject(templateListBox.SelectedItem as ProjectTemplate);
            //先设置dialogResult为false，因为有可能Create project不成功，或者path空之类的，就只能返回false
            bool dialogResult = false;
            //获取当前窗口
            var win = Window.GetWindow(this);

            if(!string.IsNullOrEmpty(projectPath) )
            {
                //不为空才变成true
                dialogResult = true;
                //打开刚刚创建的project,在里面新建一个project data的实例，并初始化里面的成员变量，传入open里面打开project 
                var project = OpenProject.Open(new ProjectData() { ProjectName = vm.ProjectName,ProjectPath = projectPath });
                //然后把datacontext绑定为新project
                win.DataContext = project;

            }
            //返回result
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
