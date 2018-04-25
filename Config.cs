using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace MusicBeePlugin
{
    using static Plugin;

    public abstract class ConfigBase
    {
        /// <summary>
        /// デフォルト値を読み込みます。
        /// </summary>
        public abstract void LoadDefault();

        /// <summary>
        /// 指定されたパスに設定をXMLシリアライズして保存します。
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            Type type = GetType();
            var serializer = new XmlSerializer(type);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(fs, this);
            }
        }

        /// <summary>
        /// 指定されたパスからXMLを取得し、デシリアライズして設定を読み込みます。
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (!File.Exists(path))
            {
                LoadDefault();
                return;
            }

            Type type = GetType();
            var serializer = new XmlSerializer(type);
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    var config = (ConfigBase)serializer.Deserialize(fs);

                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (PropertyInfo prop in properties)
                    {
                        object newValue = prop.GetValue(config, null);
                        prop.SetValue(this, newValue, null);
                    }
                }
                catch (InvalidOperationException)
                {
                    LoadDefault();
                }
            }
        }
    }

    [Serializable]
    public class Config
        : ConfigBase
    {
        #region singleton pattern
        private Config() { }
        private static Config instance = new Config();
        public static Config Instance => instance;
        #endregion

        #region public properties
        public int IntProperty { get; set; } = 0xff;
        #endregion

        #region public methods
        public override void LoadDefault() => instance = new Config();
        #endregion
    }
}
