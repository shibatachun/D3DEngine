using D3DengineEditor.GameProject;
using D3DengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace D3DengineEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
    {

        private bool _isEnable = true;
        [DataMember]
        public bool IsEnabled
        {
            get => _isEnable;
            set
            {
                if (_isEnable != value)
                {
                    _isEnable = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();

        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public ICommand RenameCommand { get;private set; }
        public ICommand IsEnableCommand { get;private set; }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if(_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }

            RenameCommand = new RelayCommand<string>(x =>
            {
                var oldNmae = _name;
                Name = x;
                Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldNmae, x, $"Rename entity '{oldNmae}' to '{x}'"));
            }, x=>x !=_name);


            IsEnableCommand= new RelayCommand<bool>(x =>
            {
                var oldNmae = _name;
                Name = x;
                Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldNmae, x, $"Rename entity '{oldNmae}' to '{x}'"));
            });

        }

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }
    }
}
