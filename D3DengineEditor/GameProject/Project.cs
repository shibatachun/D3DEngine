using D3DengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace D3DengineEditor.GameProject
{
    [DataContract(Name = "Game")]
    //Project类，总的一个大类，囊括整个project,因为需要序列化到文件中，所以需要DataContract
    class Project : ViewModelBase
    {
        //Project文件的后缀
        public static string Extension { get; } = ".dde";
        [DataMember]
        public string Name { get; private set; } = "New Project";                   //默认名字
        [DataMember]
        public string Path { get; private set; }                                    //路径

        public string FullPath => $@"{Path}{Name}\{Name}{Extension}";               //Project.dde文件的路径

        //新建一个序列化的分类
        [DataMember(Name = "Scenes")]
        //使用ObservableCollection是因为需要显示在打开project的界面中，往后的组件等同理
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();

        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        private Scene _actriveScene;
        
        //确认是否是当前激活的Scene
        public Scene ActiveScene
        {
            get => _actriveScene;
            set
            {
                if (_actriveScene != value)
                {
                    _actriveScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }
        //简化表达式和方法，如果后面的强制转换成功的话则返回Project类型，DataContext转化为Project类数据
        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        public static UndoRedo UndoRedo {  get;  } = new UndoRedo();

        public ICommand UndoCommand {  get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand AddSceneCommand {  get; private set; }
        public ICommand RemoveSceneCommand { get; private set; } 
        public ICommand SaveCommand { get; private set; } 
        private void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(this, sceneName));
        }

        private void RemoveScene(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }
       public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }
        //当前project unload也就是说退出当前Project是，重设undoredo的list
        public void Unload()
        {
            UndoRedo.Reset();
        }

        public static void Save(Project project)
        {
            Serializer.ToFile(project,project.FullPath);
            Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_scenes !=null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            AddSceneCommand = new RelayCommand<object>(x =>
            {
                Console.WriteLine("添加了");
                AddScene($"New Scene {_scenes.Count}");
                var newScene =  _scenes.Last();
                var sceneIndex = _scenes.Count - 1;
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"));
            });

            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {

                Console.WriteLine("删除了");
                var sceneIndex = _scenes.IndexOf(x);
                RemoveScene(x);
                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveScene(x),
                    $"Remove {x.Name}"));
            }, x=>!x.IsActive);
            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo());
            SaveCommand = new RelayCommand<object>(x => Save(this));
        }
        public Project(string name, string path)
        {
            Name = name; 
            Path = path;

            OnDeserialized(new StreamingContext());
        }
    }
}
