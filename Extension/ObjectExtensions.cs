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
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Routing;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace System
{
    public static class ObjectExtensions
    {
        //public static ExpandoObject ToExpandoObject(this object anonymousObject)
        //{
        //    IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
        //    IDictionary<string, object> expando = new ExpandoObject();
        //    foreach (var item in anonymousDictionary)
        //        expando.Add(item);
        //    return (ExpandoObject)expando;
        //}

        #region  [GetPropertyValue/SetPropertyValue]

        public static void SetPropertyValue(this Object obj, String propertyName, Object value)
        {
            var objPropertyInfo = obj.GetType().GetProperty(propertyName);
            if (objPropertyInfo == null)
                throw new Exception(propertyName + " is invalid property.");
            objPropertyInfo.SetValue(obj, value, null);
        }

        public static Object GetPropertyValue(this Object obj, String propertyName)
        {
            var objPropertyInfo = obj.GetType().GetProperty(propertyName);
            if (objPropertyInfo == null)
                throw new Exception(propertyName + " is invalid property.");
            return objPropertyInfo.GetValue(obj, null);
        }

        #endregion

        #region [InvokeMethod]

        public static Object InvokeMethod(this Object obj, String methodName)
        {
            return InvokeMethod(obj, methodName, null);
        }

        public static Object InvokeMethod(this Object obj, String methodName, Object[] parameters)
        {
            var objMethodInfo = obj.GetType().GetMethod(methodName);
            return objMethodInfo.Invoke(obj, parameters);
        }

        #endregion

        #region [InvokeMethodInSeparateThread]

        public static void InvokoMethodInSeparateThreadProc(Object obj)
        {
            var arg = (InvokoMethodInSeparateThreadArg)obj;
            Thread.Sleep(0);
            arg.Output.Result = arg.Input.Object.InvokeMethod(arg.Input.MethodName, arg.Input.MethodParameters);
            Thread.Sleep(0);
            arg.Output.ManualResetEvent.Set();
        }

        public class InvokoMethodInSeparateThreadArg
        {
            public class InputObject
            {
                public Object Object { get; set; }
                public String MethodName { get; set; }
                public Object[] MethodParameters { get; set; }
            }

            public class OutputObject
            {
                private object _result;
                public ManualResetEvent ManualResetEvent { get; set; }
                public Object Result
                {
                    get
                    {
                        WaitHandle.WaitAll(new[] { ManualResetEvent });
                        return _result;
                    }
                    set { _result = value; }
                }
            }

            public InputObject Input { get; set; }
            public OutputObject Output { get; set; }

            public InvokoMethodInSeparateThreadArg(Object obj, String methodName, Object[] parameters)
            {
                Input = new InputObject { Object = obj, MethodName = methodName, MethodParameters = parameters };
                Output = new OutputObject { ManualResetEvent = new ManualResetEvent(false) };
            }
        }

        public static InvokoMethodInSeparateThreadArg.OutputObject InvokoMethodInSeparateThread(this Object obj, String methodName)
        {
            return InvokoMethodInSeparateThread(obj, methodName, null);
        }

        public static InvokoMethodInSeparateThreadArg.OutputObject InvokoMethodInSeparateThread(this Object obj, String methodName, Object[] parameters)
        {
            var args = new InvokoMethodInSeparateThreadArg(obj, methodName, parameters);
            ThreadPool.QueueUserWorkItem(InvokoMethodInSeparateThreadProc, args);
            return args.Output;
        }

        #endregion

        #region [SerializeToXml/DeserializeFromXml]

        public static String SerializeToXml(this Object obj, Encoding encoding)
        {
            using (var memoryStream = new MemoryStream())
            using (var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, Encoding = encoding }))
            {
                var xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(xmlWriter, obj);
                memoryStream.Position = 0;
                var streamReader = new StreamReader(memoryStream);
                return streamReader.ReadToEnd();
            }
        }

        public static T DeserializeFromXml<T>(this Object obj, String xml, Encoding encoding) where T : class
        {
            return DeserializeFromXml(obj, xml, encoding) as T;
        }

        public static object DeserializeFromXml(this Object obj, String xml, Encoding encoding)
        {
            using (var memoryStream = new MemoryStream(encoding.GetBytes(xml)))
            using (var streamReader = new StreamReader(memoryStream, encoding))
            using (var xmlTextReader = new XmlTextReader(streamReader))
            {
                var xmlSerializer = new XmlSerializer(obj.GetType());
                return xmlSerializer.Deserialize(xmlTextReader);
            }
        }

        #endregion

        #region [SerializeToHtml]

        private static string GetHtmlRow(string trClass, string label, string field)
        {
            const string tableRowTemplate = "<tr class=\"{0}\"><td class=\"Label\">{1}</td><td class=\"Field\">{2}</td></tr>";
            return string.Format(tableRowTemplate, trClass, label, field);
        }

        private static string GetHtmlRow(string label, string field)
        {
            return GetHtmlRow("BuiltInDataType", label, field);
        }

        //public static string SerializeToHtml(this Object obj, bool addHeader = false, int maxDepth = 2, List<int> objectHashCodes = null, bool includeDiagnosticDetail = false, List<string> propertiesToIgnore = null)
        public static string SerializeToHtml(this Object obj, bool addHeader, int maxDepth, List<int> objectHashCodes, bool includeDiagnosticDetail, List<string> propertiesToIgnore)
        {
            if (maxDepth == 0)
                return "[Maximum depth reached]";
            --maxDepth;
            if (obj == null)
                return string.Empty;
            var hashCode = obj.GetHashCode();
            if (obj.GetType().IsBuiltInDataType())
                return obj.ToString();
            if (objectHashCodes == null)
            {
                objectHashCodes = new List<int>();
            }
            if (objectHashCodes.Contains(hashCode))
                return "[Already added to tree]";
            objectHashCodes.Add(hashCode);
            var tableBody = new StringBuilder(addHeader ? "<tr><th>Nome</th><th>Valor</th></tr>" : "");
            if (obj != null)
            {
                var properties = from property in obj.GetType().GetProperties() orderby property.Name select property;
                if (properties.Count() == 0) return obj.ToString();
                foreach (var property in properties)
                {
                    if (propertiesToIgnore != null && propertiesToIgnore.Contains(property.Name))
                        continue;
                    var trClass = "BuiltInDataType";
                    string propertyStringValue = string.Empty;
                    object propertyValue = null;
                    try
                    {
                        propertyValue = obj.GetPropertyValue(property.Name);
                    }
                    catch
                    {
                        //Ignore  if couldnot get attribute value
                    }
                    if (propertyValue != null)
                    {
                        if (!property.PropertyType.IsBuiltInDataType())
                        {
                            trClass = "NotBuiltInDataType";
                            if (propertyValue is IEnumerable)
                                propertyStringValue = SerializeIEnumerableItemsToHtml(addHeader, maxDepth, objectHashCodes, propertyValue, includeDiagnosticDetail, propertiesToIgnore);
                            else propertyStringValue = SerializeToHtml(propertyValue, addHeader, maxDepth, objectHashCodes, includeDiagnosticDetail, propertiesToIgnore);
                        }
                        else
                        {
                            propertyStringValue = propertyValue.ToString();
                        }
                    }
                    string diagnosticDetail = includeDiagnosticDetail ? "(" + propertyStringValue.Length / 1024 + " KB )" : "";
                    tableBody.Append(GetHtmlRow(trClass, property.Name + diagnosticDetail, propertyStringValue));
                }

                if (obj is IEnumerable)
                {
                    var field = SerializeIEnumerableItemsToHtml(addHeader, maxDepth, objectHashCodes, obj, includeDiagnosticDetail, propertiesToIgnore);
                    tableBody.Append(GetHtmlRow("NotBuiltInDataType", "Items", field));
                }
            }
            ++maxDepth;
            return "<table class=\"SerializeToHtmlGrid\">" + tableBody + "</table>";
        }

        //private static string SerializeIEnumerableItemsToHtml(bool addHeader, int maxDepth, List<int> objectHashCodes, object iEnumarableObject, bool includeDiagnosticDetail = false, List<string> propertiesToIgnore = null)
        private static string SerializeIEnumerableItemsToHtml(bool addHeader, int maxDepth, List<int> objectHashCodes, object iEnumarableObject, bool includeDiagnosticDetail, List<string> propertiesToIgnore)
        {
            var stringBuilder = new StringBuilder();
            var items = (IEnumerable)iEnumarableObject;
            int itemCount = 0;
            try { itemCount = Int32.Parse(items.GetPropertyValue("Count").ToString()); }
            catch { }
            try
            {
                foreach (var item in items)
                    stringBuilder.Append(SerializeToHtml(item, addHeader || itemCount > 1, maxDepth, objectHashCodes, includeDiagnosticDetail, propertiesToIgnore));
                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        #endregion

    }
}
