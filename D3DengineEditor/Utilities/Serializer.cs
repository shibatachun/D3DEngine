using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace D3DengineEditor.Utilities
{
    //这是一个序列化器，用于文件的序列化和反序列化
    public static class Serializer
    {
        //序列化
        public static void ToFile<T>(T instance, string path)
        {
            try
            {
                //新建一个文件流，创建
                using var fs = new FileStream(path, FileMode.Create);
                //使用DataContract里的序列器，type中的类必须是有标注有DataContact和Datamember的类和类成员
                var serializer = new DataContractSerializer(typeof(T));
                //通过文件流来吧这个实例序列化并写到文件中去
                serializer.WriteObject(fs, instance);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to serialize {instance} to  {path}");
                throw;
            }
        }

        //反序列化
        internal static T FromFile<T>(string path)
        {
            try
            {
                //文件流，mode为open
                using var fs = new FileStream(path, FileMode.Open);
                //标注要要反序列化的类
                var serializer = new DataContractSerializer(typeof(T));
                //用类型转换来把读取到的object来转换成T类型的实力
                T instance = (T) serializer.ReadObject(fs);
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to deserialize {path}");
                throw;
            }
        }
    }
}
