using D3DengineEditor.Components;
using D3DengineEditor.GameProject;
using D3DengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace D3DengineEditor.Editors
{
    /// <summary>
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChanged = false;
        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
           Loaded -= OnTransformViewLoaded;
            //如果MSTransform里的property有更改，则设置为true
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, Action<(Transform transform, Vector3)> forEachAction) {
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChanged = false;
                return null;
            }

            //获得选择的conmponents
            var selection = vm.SelectedComponents.Select(x=>selector(x)).ToList();
            //给undoaction赋值
            return  new Action(() =>
            {
                //选中给的每个Component一起修改
                selection.ForEach(x => forEachAction(x));
                //边输入边更新数值
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });

        }
        private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x)=>x.transform.Position = x.Item2 );
        private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x)=>x.transform.Rotation = x.Item2 );
        private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x)=>x.transform.Scale = x.Item2 );

        private void RecordActions(Action redoAction, string name)
        {
            //检测到有更新。进行下面逻辑，并且把undo redo action加到总undoredo action里面去
            if (_propertyChanged)
            {
                Debug.Assert(_undoAction != null);
                _propertyChanged = false;
             
                //加入undoredo的列表
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction,name));
            }
        }
        //Position----------------------------------------
        private void OnPosition_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            //这里先设置为默认值为否
            _propertyChanged = false;
            //调用getAction
            _undoAction = GetPositionAction();
            
        }

        private void OnPosition_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {
            
            RecordActions(GetPositionAction(), "Position changed");

        }

        //Rotation---------------------------------------
        private void OnRotation_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            //这里先设置为默认值为否
            _propertyChanged = false;
            //调用getAction
            _undoAction = GetRotationAction();

        }

        private void OnRotation_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {

            RecordActions(GetRotationAction(), "Rotation changed");

        }

        //Scale----------------------------------------
        private void OnScale_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            //这里先设置为默认值为否
            _propertyChanged = false;
            //调用getAction
            _undoAction = GetScaleAction();

        }

        private void OnScale_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
        {

            RecordActions(GetScaleAction(), "Scale changed");

        }

        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnPosition_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnRotation_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }


        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyChanged && _undoAction != null)
            {
                OnScale_VectorBox_PreviewMouse_LBU(sender, null);
            }
        }

    }
}
