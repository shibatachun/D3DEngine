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
    public static class Serializer
    {
        public static void ToFile<T>(T intstance, string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Create);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fs, intstance);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal static T FromFile<T>(string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                T instance = (T) serializer.ReadObject(fs);
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //TODO: log error
                return default(T);
            }
        }
    }
}
