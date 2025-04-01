﻿using D3DengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace D3DengineEditor.Components
{
    [DataContract]
   class Transform : Component
    {
        private Vector3 _position ;
        [DataMember]
        public Vector3 Position
        {
            get => _position;
            set
            {
                if(_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }


        private Vector3 _rotation;
        [DataMember]
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }


        private Vector3 _scale;
        [DataMember]
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }
        public override IMSComponent GetMultiselectionComponent(MSEntity msEntity) => new MSTransform(msEntity);

        public override void WriteToBinary(BinaryWriter bw)
        {
            bw.Write(_position.X); bw.Write(_position.Y); bw.Write(_position.Z);
            bw.Write(_rotation.X); bw.Write(_rotation.Y); bw.Write(_rotation.Z);
            bw.Write(_scale.X); bw.Write(_scale.Y); bw.Write(_scale.Z);
        }

        public Transform(GameEntity owner) : base(owner)
        {
        }

     
    }
    //sealed表示该类不能被继承，减少虚数表开销
    sealed class MSTransform : MSComponent<Transform>
    {
        //如果是多个transform component并表示不同的值可能会表示为null
        private float? _posX = 0;
        public float? PosX
        {
            get => _posX;
            set
            {
                if (!_posX.IsTheSameAs(value))
                {
                        _posX = value;
                        OnPropertyChanged(nameof(PosX));
                    
                }
            }
        }

        private float? _posY = 0;
        public float? PosY
        {
            get => _posY;
            set
            {
                if (!_posY.IsTheSameAs(value))
                {
                  
                        _posY = value;
                        OnPropertyChanged(nameof(PosY));
                    
                }
            }
        }

        private float? _posZ = 0;
        public float? PosZ
        {
            get => _posZ;
            set
            {
                if (!_posZ.IsTheSameAs(value))
                {
                   
                        _posZ = value;
                        OnPropertyChanged(nameof(PosZ));
                    
                }
            }
        }

        private float? _rotX = 0;
        public float? RotX
        {
            get => _rotX;
            set
            {
                if (!_rotX.IsTheSameAs(value))
                {
                    
                        _rotX = value;
                       OnPropertyChanged(nameof(RotX));
                    
                }
            }
        }
        private float? _rotY = 0;
        public float? RotY
        {
            get => _rotY;
            set
            {
                if (!_rotY.IsTheSameAs(value))
                {
                    _rotY = value;
                    OnPropertyChanged(nameof(RotY));

                }
            }
        }
        private float? _rotZ = 0;
        public float? RotZ
        {
            get => _rotZ;
            set
            {
                if (!_rotZ.IsTheSameAs(value))
                {
                    
                        _rotZ = value;
                        OnPropertyChanged(nameof(RotZ));
                    
                }
            }
        }

        private float? _scaleX = 0;
        public float? ScaleX
        {
            get => _scaleX;
            set
            {
                if (!_scaleX.IsTheSameAs(value))
                {
                    
                        _scaleX = value;
                        OnPropertyChanged(nameof(ScaleX));
                    
                }
            }
        }
        private float? _scaleY = 0;
        public float? ScaleY
        {
            get => _scaleY;
            set
            {
                if (!_scaleY.IsTheSameAs(value))
                {
                    
                        _scaleY = value;
                        OnPropertyChanged(nameof(ScaleY));
                    
                }
            }
        }
        private float? _scaleZ = 0;
        public float? ScaleZ
        {
            get => _scaleZ;
            set
            {
                if (!_scaleZ.IsTheSameAs(value))
                {
                    if (_scaleZ != value)
                    {
                        _scaleZ = value;
                        OnPropertyChanged(nameof(ScaleZ));
                    }
                }
            }
        }
        protected override bool UpdateComponents(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(PosX):
                case nameof(PosY):
                case nameof(PosZ):
                    //作用是修改 SelectedComponents 列表中每个组件的 Position 属性
                    SelectedComponents.ForEach(c=> c.Position = new Vector3(_posX ?? c.Position.X,_posY?? c.Position.Y,_posZ ??  c.Position.Z));
                    return true;
                case nameof(RotX):
                case nameof(RotY):
                case nameof(RotZ):
                    SelectedComponents.ForEach(c => c.Rotation = new Vector3(_rotX ?? c.Rotation.X, _rotY ?? c.Rotation.Y, _rotZ ?? c.Rotation.Z));
                    return true;
                case nameof(ScaleX):
                case nameof(ScaleY):
                case nameof(ScaleZ):
                    SelectedComponents.ForEach(c => c.Scale = new Vector3(_scaleX ?? c.Scale.X, _scaleY ?? c.Scale.Y, _scaleZ ?? c.Scale.Z));
                    return true;
            }
            return false;
        }

        protected override bool UpdateMSComponent()
        {
            PosX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.X));
            PosY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.Y));
            PosZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Position.Z));

            RotX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.X));
            RotY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.Y));
            RotZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Rotation.Z));

            ScaleX = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.X));
            ScaleY = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.Y));
            ScaleZ = MSEntity.GetMixedValue(SelectedComponents, new Func<Transform, float>(x => x.Scale.Z));

            return true;
        }

        public MSTransform(MSEntity msEntity) : base(msEntity)
        {
            Refresh();
        }
    }
}
