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
using Suflow.Common.Utils;

namespace System {
    public static class IEnumerableExtensions {

        /// <summary>
        /// Creates a subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.
        /// </summary>
        /// <typeparam name="T">The type of object the collection should contain.</typeparam>
        /// <param name="superset">The collection of objects to be divided into subsets. If the collection implements <see cref="IQueryable{T}"/>, it will be treated as such.</param>
        /// <param name="pageNumber">The one-based index of the subset of objects to be contained by this instance.</param>
        /// <param name="pageSize">The maximum size of any individual subset.</param>
        /// <returns>A subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.</returns>
        /// <seealso cref="PagedList{T}"/>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> superset, int pageNumber, int pageSize) {
            return new PagedList<T>(superset, pageNumber, pageSize);
        }

        /// <summary>
        /// Creates a subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.
        /// </summary>
        /// <typeparam name="T">The type of object the collection should contain.</typeparam>
        /// <param name="superset">The collection of objects to be divided into subsets. If the collection implements <see cref="IQueryable{T}"/>, it will be treated as such.</param>
        /// <param name="pageNumber">The one-based index of the subset of objects to be contained by this instance.</param>
        /// <param name="pageSize">The maximum size of any individual subset.</param>
        /// <returns>A subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.</returns>
        /// <seealso cref="PagedList{T}"/>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> superset, int pageNumber, int pageSize) {
            return new PagedList<T>(superset, pageNumber, pageSize);
        }

        /// <summary>
        /// Splits a collection of objects into n pages with an (for example, if I have a list of 45 shoes and say 'shoes.Split(5)' I will now have 4 pages of 10 shoes and 1 page of 5 shoes.
        /// </summary>
        /// <typeparam name="T">The type of object the collection should contain.</typeparam>
        /// <param name="superset">The collection of objects to be divided into subsets.</param>
        /// <param name="numberOfPages">The number of pages this collection should be split into.</param>
        /// <returns>A subset of this collection of objects, split into n pages.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> superset, int numberOfPages) {
            return superset
                .Select((item, index) => new { index, item })
                .GroupBy(x => x.index % numberOfPages)
                .Select(x => x.Select(y => y.item));
        }

        /// <summary>
        /// Splits a collection of objects into an unknown number of pages with n items per page (for example, if I have a list of 45 shoes and say 'shoes.Partition(10)' I will now have 4 pages of 10 shoes and 1 page of 5 shoes.
        /// </summary>
        /// <typeparam name="T">The type of object the collection should contain.</typeparam>
        /// <param name="superset">The collection of objects to be divided into subsets.</param>
        /// <param name="pageSize">The maximum number of items each page may contain.</param>
        /// <returns>A subset of this collection of objects, split into pages of maximum size n.</returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> superset, int pageSize) {
            if (superset.Count() < pageSize)
                yield return superset;
            else {
                var numberOfPages = Math.Ceiling(superset.Count() / (double)pageSize);
                for (var i = 0; i < numberOfPages; i++)
                    yield return superset.Skip(pageSize * i).Take(pageSize);
            }
        }

        /// <summary>
        /// Determines whether an enumerable is null or empty.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) {
            if (collection == null)
                return true;
            return !collection.Any();
        }

        public static IEnumerable<T> GetListByField<T>(this IEnumerable<T> list, String propertyName, Object propertyValue) where T : class {
            var items = from m in list
                        where (m.GetPropertyValue(propertyName) == null && propertyValue == null) ||
                              (m.GetPropertyValue(propertyName) != null && m.GetPropertyValue(propertyName).Equals(propertyValue))
                        select m;
            return items;
        }

        public static T GetByField<T>(this IEnumerable<T> list, String propertyName, Object propertyValue) where T : class {
            return GetByField(list, propertyName, propertyValue, false);
        }

        public static T GetByField<T>(this IEnumerable<T> list, String propertyName, Object propertyValue, Boolean throwExceptionIfNotFound) where T : class {
            var items = list.GetListByField(propertyName, propertyValue);
            if (items.Count() == 0) {
                if (throwExceptionIfNotFound)
                    throw new Exception(string.Format("{0} with value '{1}' not found.", propertyName, propertyValue));
                return null;
            }
            return items.FirstOrDefault();
        }

       
    }
}
