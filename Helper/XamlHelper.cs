using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Suflow.Common.Utils {


    /// <summary>
    /// http://xamlgenerator.codeplex.com/ 
    /// Xaml Code Behind Generator makes it easier to generate 
    /// Code Behind designer code from XAML, where code can be used 
    /// at places where Xaml Services are not accessible.
    /// </summary>
    public class XamlHelper {

        public static string GetXamlFromUIElement(UIElement elementToExport) {
            var xamlOfObject = XamlWriter.Save(elementToExport);
            return xamlOfObject;
        }

        #region Import

        public static object ImportFromFile() {
            var fileName = FileHelper.SelectFileToOpen();
            return ImportFromFile(fileName);
        }

        public static object ImportFromFile(string fileName) {
            var fileContent = File.ReadAllBytes(fileName);
            return ImportFromByteArray(fileContent);
        }

        public static object ImportFromString(string content) {
            return ImportFromStream(content.ToMemoryStream());
        }

        public static object ImportFromByteArray(byte[] fileContent) {
            var fileStream = fileContent.ToMemoryStream();
            return ImportFromStream(fileStream);
        }

        public static object ImportFromStream(Stream stream) {
            return System.Windows.Markup.XamlReader.Load(stream);
        }

        #endregion

        #region Export

        public static string ExportToString(UIElement elementToExport) {
            return GetXamlFromUIElement(elementToExport);
        }

        public static void ExportToFile(UIElement elementToExport) {
            var fileName = FileHelper.SelectFileToSave();
            if (fileName != null)
                ExportToFile(elementToExport, fileName);
        }

        public static void ExportToFile(UIElement elementToExport, string fileName) {
            var xamlOfObject = GetXamlFromUIElement(elementToExport);
            File.WriteAllText(fileName, xamlOfObject);
        }

        #endregion
    }
}
