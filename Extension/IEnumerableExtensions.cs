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

namespace System
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> GetListByField<T>(this IEnumerable<T> list, String propertyName, Object propertyValue) where T : class
        {
            var items = from m in list
                        where (m.GetPropertyValue(propertyName) == null && propertyValue == null) ||
                              (m.GetPropertyValue(propertyName) != null && m.GetPropertyValue(propertyName).Equals(propertyValue))
                        select m;
            return items;
        }

        public static T GetByField<T>(this IEnumerable<T> list, String propertyName, Object propertyValue) where T : class
        {
            return GetByField(list, propertyName, propertyValue, false);
        }

        public static T GetByField<T>(this IEnumerable<T> list, String propertyName, Object propertyValue, Boolean throwExceptionIfNotFound) where T : class
        {
            var items = list.GetListByField(propertyName, propertyValue);
            if (items.Count() == 0)
            {
                if (throwExceptionIfNotFound)
                    throw new Exception(string.Format("{0} with value '{1}' not found.", propertyName, propertyValue));
                return null;
            }
            return items.FirstOrDefault();
        }
    }
}
