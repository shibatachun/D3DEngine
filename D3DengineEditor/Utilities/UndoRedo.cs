using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace D3DengineEditor.Utilities
{
    //这个接口定义了Undo和Redo必须实现的基础功能，操作的名字，他的Undo操作和Redo操作
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }
    //这个UndoRedoAction则是IUndoRedo的实现类
    public class UndoRedoAction : IUndoRedo
    {
        //Action是一个委托类型，可以引用一个不需要返回值的方法，这里我们定义了两个方法，undoAction和redoAction
        //委托可以把方法作为参数传递
        
        private Action _undoAction;
        private Action _redoAction;

        //操作名字
        public string Name { get; set; }
        //把Undo和Redo这两个方法加上了_undoAction和_redoAction的委托，调用这两个方法等于调用这两个委托
        public void Undo() => _undoAction();
        public void Redo() => _redoAction();

        //UndoRedo Action的构造器
        public UndoRedoAction(string name)
        {
            Name = name;
        }
        //也是构造器，不过把两个Action委托传递进来了，包括操作名
        public UndoRedoAction(Action undo, Action redo, string name)
            : this(name)
        {
            Debug.Assert(undo != null && redo != null);
            _undoAction = undo;
            _redoAction = redo;
        }

        //构造器，传入了property， 实例， undoValue和redoValue， 还有操作名, 这里是要构建我们对某个类的实例里的property改变的undo redo Action,比如改scene的名字，这是一种构造函数重载
        public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string name):
        
            this(
                //转换为两个Action委托来进行构造
                () => instance.GetType().GetProperty(property).SetValue(instance,undoValue),
                () => instance.GetType().GetProperty(property).SetValue(instance,redoValue),
                name)
        { }
        
    }

    //这是UndoRedo class， 用于显示在view层和总的管理UndoRedo Action类的
    public class UndoRedo
    {
        private bool _enableAdd = true;
        private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();
        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

        public void Reset()
        {
            _redoList.Clear();
            _undoList.Clear();
        }

        public void Add(IUndoRedo cmd)
        {
            if(_enableAdd) 
             {
                    _undoList.Add(cmd);
                    _redoList.Clear();
              }
           
        }
        public void Undo()
        {
            Console.WriteLine("Undo triggered!");
            if(_undoList.Any())
            {
                var cmd = _undoList.Last();
                _undoList.RemoveAt(_undoList.Count - 1);
                _enableAdd = false;
                cmd.Undo();
                _enableAdd = true;
                _redoList.Insert(0,cmd);
            }
        }
        public void Redo()
        {
            Console.WriteLine("Redo triggered!");
            if (_redoList.Any())
            {
                var cmd = _redoList.First();
                _redoList.RemoveAt(0);
                _enableAdd = false;
                cmd.Redo();
                _enableAdd = true;  
                _undoList.Add(cmd);
            }
        }
        public UndoRedo()
        {
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
        }
    }
}
