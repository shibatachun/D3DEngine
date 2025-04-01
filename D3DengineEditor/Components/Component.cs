using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace D3DengineEditor.Components
{
    interface IMSComponent { }
    [DataContract]
    abstract class Component : ViewModelBase
    {

     
        [DataMember]
        public GameEntity Owner { get; private set; }
        public abstract IMSComponent GetMultiselectionComponent(MSEntity msEntity);
        public abstract void WriteToBinary(BinaryWriter bw);
        public Component(GameEntity owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }
    }

    abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
    {

        private bool _enableUpdates = true;
        //根据选择的组件来传入，所以用泛型来组建list
        public List<T> SelectedComponents { get; }

        //这里更新component，参数为这个component需要更新的property名字 Exp: transform component的 position rotation 等
        protected abstract bool UpdateComponents(string propertyName);
        //这里收集components的信息
        protected abstract bool UpdateMSComponent();

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSComponent();
            _enableUpdates = true;
        }
        public MSComponent(MSEntity msEntity)
        {
            //判断是否有被选中的entity
            Debug.Assert(msEntity?.SelectedEntities?.Any() == true);
            //把选中的组件转换成list
            SelectedComponents = msEntity.SelectedEntities.Select(entity => entity.GetComponent<T>()).ToList();

            //如果有任何一个property被改变了，后台的property会被同步更新， 使用——enableUpdate的原因是如果我们使用updateMScomponent的话，因为我们要Gathering information\
            //所以这时候不希望继续property继续被更新，如果这时候被更新就无法被收集到正确的信息
            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateComponents(e.PropertyName); };
        }
    }
}
