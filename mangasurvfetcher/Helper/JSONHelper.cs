using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mangasurvlib.Helper
{
    /// <summary>
    /// Helper for JSON library.
    /// Usefull methods are here shorter.
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// Serialize a object.
        /// </summary>
        /// <param name="FileName">Specifies the location of the file.</param>
        /// <param name="Obj">Object which has to be serialized.</param>
        public static void Serialize(string FileName, object Obj)
        {
            System.IO.File.WriteAllText(FileName, Newtonsoft.Json.JsonConvert.SerializeObject(Obj, Newtonsoft.Json.Formatting.Indented));
        }

        /// <summary>
        /// Serialize a object.
        /// </summary>
        /// <param name="FileName">Specifies the location of the file.</param>
        /// <param name="Obj">Object which has to be serialized.</param>
        public static string Serialize(object Obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Obj, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Serialize a object.
        /// </summary>
        /// <param name="FileName">Specifies the location of the file.</param>
        /// <param name="Obj">Object which has to be serialized.</param>
        /// <param name="Format">How to format, default is "None".</param>
        public static void Serialize(string FileName, object Obj, Newtonsoft.Json.Formatting Format)
        {
            System.IO.File.WriteAllText(FileName, Newtonsoft.Json.JsonConvert.SerializeObject(Obj, Format));
        }

        public static object DeserializeString(string sContent, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(sContent, type);
        }

        public static T DeserializeString<T>(string sContent)
        {
            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(sContent, typeof(T));
        }

        public static object Deserialize(string FileName)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(System.IO.File.ReadAllText(FileName));
        }

        public static object Deserialize(string FileName, Type Type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(System.IO.File.ReadAllText(FileName), Type);
        }

        public static List<T> DeserializeList<T>(string FileName)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(System.IO.File.ReadAllText(FileName));
        }

        public static string SerializeCompressed(object Obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Obj, Newtonsoft.Json.Formatting.None);
        }
    }
}
