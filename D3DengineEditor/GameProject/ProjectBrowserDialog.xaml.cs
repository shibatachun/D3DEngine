using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using D3DengineEditor.GameProject;

namespace D3DengineEditor.GameProject
{
    /// <summary>
    /// ProjectBrowswerDialg.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            
            Loaded += OnProjectBrowserDialogLoaded;
        }


        private void OnProjectBrowserDialogLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnProjectBrowserDialogLoaded;
            //检测是否存在project，如果没有的话，禁用open project tab，并且跳转到create new project tab
            if(!OpenProject.Projects.Any())
            {
                openProjectButton.IsEnabled = false;
                openProjectView.Visibility = Visibility.Hidden;
                OnToggleButton_Click(createProjectButton,new RoutedEventArgs());
            }

        }

        private void OnToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //如果触发的是open project 的button的话，进行以下逻辑
            if (sender == openProjectButton)
            {
                //如果目前打开的是openproject页面而新建project还是可以被选中的话，为了避免来回切换的时候都是true,则需要把create project那个Button uncheck。
                if (createProjectButton.IsChecked == true)
                {
                    //uncheck the create project button
                    createProjectButton.IsChecked = false;
                    //切换到open project的页面, thickness是用来设置margin，包含一些参数，分别是left top right bottom
                    browserContent.Margin = new Thickness(0);
                }
                openProjectButton.IsChecked = true;
            }
            else
            {
                if (openProjectButton.IsChecked == true)
                {
                    openProjectButton.IsChecked = false;
                    //切换到New project的页面
                    browserContent.Margin = new Thickness(-800,0,0,0);
                }
                createProjectButton.IsChecked = true;
            }
        }
    }
}
