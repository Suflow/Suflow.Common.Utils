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
