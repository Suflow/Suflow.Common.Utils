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
using System.Reflection;
using System.Text;

namespace System
{
    public static class TypeExtensions
    {
        
        private static List<Type> _buildInDataTypeHashTable;

        public static bool IsBuiltInDataType(this Type type)
        {
            if (_buildInDataTypeHashTable == null)
            {
                _buildInDataTypeHashTable = new List<Type>
                                                {
                                                    typeof (byte),
                                                    typeof (sbyte),
                                                    typeof (int),
                                                    typeof (uint),
                                                    typeof (short),
                                                    typeof (ushort),
                                                    typeof (long),
                                                    typeof (ulong),
                                                    typeof (float),
                                                    typeof (double),
                                                    typeof (char),
                                                    typeof (bool),
                                                    typeof (string),
                                                    typeof (decimal),
                                                    typeof (DateTime),
                                                    typeof (DateTime?),
                                                };
            }
            return _buildInDataTypeHashTable.Contains(type);
        }

        public static Object GetStaticPropertyValue(this Type type, String propertyName)
        {
            return GetStaticPropertyValue(type, propertyName, true);
        }

        public static Object GetStaticPropertyValue(this Type type, String propertyName, bool throwExceptionIfNotFound)
        {
            var objPropertyInfo = type.GetProperty(propertyName);
            if (objPropertyInfo == null)
            {
                if (throwExceptionIfNotFound)
                    throw new Exception(propertyName + " is invalid property.");
                else return null;
            }
            return objPropertyInfo.GetValue(null, null);
        }

        public static List<MethodInfo> GetAllPublicMethods(this Type type)
        {
            return type.GetMethods().Where(abc => abc.DeclaringType == type && abc.IsPublic && !abc.IsConstructor).ToList();
        }
    }
}
