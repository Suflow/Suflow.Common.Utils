////////////////////////////////////////////////////////////////////////////////
//
//    Suflow, Enterprise Applications
//    Copyright (C) 2015 Suflow
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////
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
