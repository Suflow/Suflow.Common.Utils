using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Suflow.Common.Utils
{
    public abstract class ConfigurationBase
    {
        private static string _configFilePath;

        protected virtual string ConfigurationFilePath
        {
            get
            {
                if (_configFilePath == null)
                    _configFilePath = Directory.GetCurrentDirectory() + "\\" + Assembly.GetEntryAssembly().GetName().Name + ".Config";
                return _configFilePath;
            }
        }

        public virtual ConfigurationBase Load()
        {            
            if (File.Exists(ConfigurationFilePath))
            {
                var xml = File.ReadAllText(ConfigurationFilePath);
                if (xml.Length > 0)
                {
                    return this.DeserializeFromXml(xml, Encoding.ASCII) as ConfigurationBase;
                }
            }
            return this;
        }

        public virtual void Save()
        {
            var xml = this.SerializeToXml(Encoding.ASCII);
            File.Delete(ConfigurationFilePath);
            File.WriteAllText(ConfigurationFilePath, xml);
        }
    }
}
