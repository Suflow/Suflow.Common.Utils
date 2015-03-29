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
