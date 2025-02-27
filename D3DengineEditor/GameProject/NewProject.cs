using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using D3DengineEditor.GameProject;
using D3DengineEditor.Utilities;


namespace D3DengineEditor.GameProject
{
    
    //因为需要这个数据被序列化，所以这里使用了DataContract标注
    [DataContract]
    //这个是project Template类，包括了一系列的template创建 ： 类
    public class ProjectTemplate
    {
        [DataMember]
        
        public string ProjectType { get; set; }                 //这个template的类型
        [DataMember]
        public string ProjectFile { get; set; }                 //这个关于这个template里的project的实际文件，是.p
        [DataMember]
        public List<string> Folders { get; set; }               //是各种template 里面包含的实际project文件，包括game code，等等，生成之后的实际project能够生成里面包含的文件夹
        public byte[] Icon { get; set; }                        //此template的Icon，图标
        public byte[] Screenshot { get; set; }                  //此template的预览图
        public string IconFilePath { get; set; }                //此template的Icon路径
        public string ScreenshotFilePath { get; set; }          //此template的预览图路径
        public string ProjectFilePath { get; set; }             //此template的project文件路径

        public string TemplatePath {  get; set; }

    }

    //这是new project类，继承自ViewModel，属于一个ViewMode. 来进行model层和view层的交互逻辑：类
    class NewProject : ViewModelBase
    {
        //TODO: get the path from the installation lcoation
        private readonly string _templatePath = @"..\..\D3DengineEditor\ProjectTemplates";
        
        //新project的名字
        private string _projectName = "NewProject";
        //设置一个property，可以使得能更改这project名字
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName =  value;
                    ValidateProjectPathAndName();
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }
        private string _projectPath =$@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\D3DengineProjects\";

        //新project的路径
        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    ValidateProjectPathAndName();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private string _errorMsg;
        public string ErrorMsg
        {
            get => _errorMsg;
            //set会自动捕获变量设置的值，这里主要是用来更新用户新输入的变量
            set
            {
                if (_errorMsg != value)
                {
                    _errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }

        //这是一个collection，用来展示在view界面上，这是一个泛型类，可以存入Project类，observable表示这个是可发现的，当collection里存的东西发生变化时，可以通知view来更新view绑定的数据，仅限private
        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        //是一个对于上面的集合的一个只读包装器，用于外部对内部数据集合的只读访问，只用于获取数据，对外开放。
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }


        //一个简单的对路径和名字进行检查
        private bool ValidateProjectPathAndName()
        {
            //先是获取project的路径
            var path = ProjectPath;

            //如果用户输入的路径后面不带\的，上一个\，然后接一个Project name来组成full path
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            path += $@"{ProjectName}\";
            
            //先把Valid设置为false
            IsValid = false;
            //非空检查: Project Name
            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMsg = "Type in a project name";

            }
            //非法字符检查:Project Name
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invalid character(s) used in project name.";
            }
            //移除头部和尾部所有空格，这是.Trim()的用法，然后再检测一边非空: Project Path
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Select a valid project folder.";
            }
            //检测路径是否包含非法字符： Project Path
            else if(ProjectPath.IndexOfAny(Path.GetInvalidPathChars())!=-1)
            {
                ErrorMsg = "Invalid characater(s) used in project path.";
            }
            //用来检测是当前目录含有同名project
            else if(Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Selected project folder already exists and is not empty.";

            }
            else
            {
                ErrorMsg = string.Empty;
                IsValid = true;
            }
            Console.WriteLine(ErrorMsg);
            return IsValid;
        }
        //开始创建Project， 参数为一个ProjectTemplate
        public string CreateProject(ProjectTemplate template)
        {
            //首先再次检查一边输入的项目名字路径等是否合规
            ValidateProjectPathAndName();

            //检测完之后不过的话，则返回一个空的字符串
            if (!IsValid)
            {
                return string.Empty;
            }

            //组合成完整的path
            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath+= @"\";
            var path = $@"{ProjectPath}{ProjectName}\";
            //开始创建过程
            try
            {
                //如果当前Path不存在，以当前的path创建
                if(!Directory.Exists(path)) Directory.CreateDirectory(path);

                //遍历template里面包含的实际project需要的文件夹，一般在template.xml中定义
                foreach (var folder in template.Folders)
                {
                    //在目标目录创建文件夹
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path),folder)));

                }
                //创建一个.dde文件夹来记录隐藏文件，一般是目录自带的文件.类似于.vs
                var dirInfo = new DirectoryInfo(path+ @".dde\");
                //设置属性为隐藏
                dirInfo.Attributes |= FileAttributes.Hidden;
                //把screenshot和icon一起复制到目标.dde目录
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                //获得project.dde
                var projectXml = File.ReadAllText(template.ProjectFilePath);
                //在项目文件中有{0} {1}等站位服会被Format()函数里的projectName和projectpath替代
                projectXml = string.Format(projectXml, ProjectName, ProjectPath);

                //新的projectPth路径
                var projectPath = Path.GetFullPath(Path.Combine(path,$"{ProjectName}{Project.Extension}"));
                //往这个路径写入project xml的所有内容，至此，项目创建完毕
                File.WriteAllText(projectPath,projectXml);
                    
                CreateMSVCSolution(template, path);
                return path;

                
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create {ProjectName}");
                throw;
            }
        }

        private void CreateMSVCSolution(ProjectTemplate template, string projectPath)
        {
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "MSVCSolution")));
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "MSVCProject")));

            var engineAPIPath = Path.Combine(MainWindow.D3DPath, @"D3DEngine\EngineAPI\");
            Debug.Assert(Directory.Exists(engineAPIPath));

            var _0 = ProjectName;
            var _1 = "{"+ Guid.NewGuid().ToString().ToUpper()+"}";

            var _2 = engineAPIPath;
            var _3 = MainWindow.D3DPath;
            var solution = File.ReadAllText(Path.Combine(template.TemplatePath, "MSVCSolution"));
            solution = string.Format(solution, _0,_1, "{" + Guid.NewGuid().ToString().ToUpper() + "}");
            File.WriteAllText(Path.GetFullPath(Path.Combine(projectPath, $"{_0}.sln")),solution);

            var project = File.ReadAllText(Path.Combine(template.TemplatePath, "MSVCProject"));
            project = string.Format(project, _0, _1,_2,_3);

            File.WriteAllText(Path.GetFullPath(Path.Combine(projectPath, $"Gamecode/{_0}.vcxproj")), project);


        }

        //New project类的构造器
        public NewProject()
        {
            //新建一个对外的只读访问器，数据是_projectTemplates是私有的可观测集合，用于程序内部的数据存储
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                //初始化先得到模板文件数据，通过传入模板路径，寻找各自预先创建好的template.xml文件，里面记录了模板project里的信息，然后返回其文件数据
                var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templatesFiles.Any());
                //然后通过遍历里面的template.xml反序列化，来获得每个template project里面的数据
                foreach (var file in templatesFiles)
                {
                    //通过反序列化获得数据，类型是ProjectTemplate类型,这里就返回了一个projectTemplate类型的实例
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    //绑定这个template里的Icon
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    //绑定这个template里的screenshot
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    //获得这个template里的project file的full path
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    template.TemplatePath = Path.GetDirectoryName(file);
                    //把这个template project的实例添加到observable collection里，以备在view中进行展示,因为序列化的过程中没有把这些文件的path写进去
                    _projectTemplates.Add(template);
                }
                ValidateProjectPathAndName();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to read project templates");
                throw;
            }
        }


    } 
}
