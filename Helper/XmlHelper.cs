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
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using System.Xml.XPath;

namespace Suflow.Common.Utils
{
    public static class XmlHelper
    {
        public static String ApplyXslt(String xsltStr, String xmlStr)
        {
            var xsltStream = new MemoryStream(xsltStr.ToByteArray());
            var xmlStream = new MemoryStream(xmlStr.ToByteArray());
            var outputStream = new MemoryStream();
            ApplyXslt(xsltStream, xmlStream, outputStream, null);
            return outputStream.ToString();
        }

        public static void ApplyXslt(Stream xsltStr, Stream xmlStr, Stream outputStr, XsltArgumentList args)
        {
            using (XmlReader xsltReader = XmlReader.Create(xsltStr))
            using (XmlReader xmlReader = XmlReader.Create(xmlStr))
            using (XmlWriter xmlWriter = XmlWriter.Create(outputStr))
            {
                var xslCompiledTransform = new XslCompiledTransform();
                xslCompiledTransform.Load(xsltReader);
                if(args == null)
                    xslCompiledTransform.Transform(xsltReader, xmlWriter);
                else xslCompiledTransform.Transform(xsltReader, args, xmlWriter);                
            }
        }
    }
}
