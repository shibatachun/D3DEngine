using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Accessibility;
using D3DengineEditor.GameProject;
using D3DengineEditor.Utilities;

namespace D3DengineEditor.GameProject
{
    [DataContract]
    //Project Data类，用来记录project的数据
    public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }                 //Project的名字
        [DataMember]
        public string ProjectPath { get; set; }                 //Project所在的路径
        [DataMember]
        public DateTime Date { get; set; }                      //创建日期或者最后活动日期？

        public string FullPath { get=>$"{ProjectPath}{ProjectName}{Project.Extension}";}    //完整的project的.dde的路径

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        //public string IconFilePath { get; set; }
        //public string ScreenshotFilePath { get; set; }
        //public string ProjectFilePath { get; set; }
    }

    //ProjectData List类
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }
    class OpenProject
    {
        //Probably will change later
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\D3DengieEditor\";
        private static readonly string _projectDataPath;
        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();

        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }
        private static void ReadProjectData()
        {
            if(File.Exists(_projectDataPath))
            {
                var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date);
                _projects.Clear();
                foreach (var project in projects)
                {
                    if(File.Exists(project.FullPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.dde\Icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.dde\Screenshot.png");
                        _projects.Add(project);
                    }
                }
            }
        }
        private static void WriteProjectData()
        {
            var projects = _projects.OrderBy(x => x.Date).ToList();
            Serializer.ToFile(new ProjectDataList(){ Projects = projects}, _projectDataPath);
        }
        //从oepn project或者new project进来的，打开project，参数为一个ProjectData类
        public static Project Open(ProjectData data)
        {
            ReadProjectData();
            var project = _projects.FirstOrDefault(x=>x.FullPath == data.FullPath);
            if (project != null)
            {
                project.Date = DateTime.Now;
            }
            else
            {
                project = data;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }
            WriteProjectData();

            return Project.Load(project.FullPath);

        }

     

        static OpenProject()
        {
            try
            {
                if (!Directory.Exists(_applicationDataPath)) Directory.CreateDirectory(_applicationDataPath);
                _projectDataPath = $@"{_applicationDataPath}projectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
                ReadProjectData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Logger.Log(MessageType.Error, $"Failed to read project data ");
                throw;
            }
        }

      
      
    }
}
