﻿////////////////////////////////////////////////////////////////////////////////
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

namespace Suflow.Common.Utils
{
    public class RuntimeTestExecuter
    {
        public static string Execute<T>()
        {
            var result = new StringBuilder();
            var methods = typeof(T).GetMethods().Where(m => m.Name.StartsWith("Test"));
            var instance = Activator.CreateInstance<T>();
            foreach (var method in methods)
            {
                try
                {
                    instance.InvokeMethod(method.Name);
                    result.AppendLine(method.Name + " : SUCCESS ");
                }
                catch (Exception exp)
                {
                    result.AppendLine(method.Name + " : ERROR : " + exp.Message);
                }
            }
            return result.ToString();
        }
    }
}
