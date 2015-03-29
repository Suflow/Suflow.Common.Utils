using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suflow.Common.Utils
{
    public class HtmlGenHelper
    {
        public static string CreateEditForm(string formAction, string sumbmitButtonText, object model)
        {
            var fields = new List<string>();
            var properties = model.GetType().GetProperties();
            IEnumerable<string> propertyNameList = from property in properties select property.Name;
            return CreateEditForm(formAction, sumbmitButtonText, propertyNameList.ToArray());
        }

        public static string CreateEditForm(string formAction, string sumbmitButtonText, params string[] fields)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("<form name=\"input\" action=\"{0}\" method=\"post\" class=\"border\"><table>", formAction));
            foreach (var field in fields)
            {
                var splitedField = field.Split('|');
                var fieldName = splitedField[0];
                string fieldValue = "";
                if (splitedField.Length > 1)
                    fieldValue = splitedField[1];
                stringBuilder.Append(string.Format("<tr><td>{0} : </td><td><input class=\"input_type_text\" type=\"text\" name=\"{0}\" value=\"{1}\" /></td></tr>", fieldName, fieldValue));
            }
            stringBuilder.Append(string.Format("<tr><td></td><td><input type=\"submit\" value=\"{0}\" /></td></table>", sumbmitButtonText));
            stringBuilder.Append("</form>");
            return stringBuilder.ToString();
        }

        public static string CreateDisplayFormWithDiagnosticDetail(object model)
        {
            var objectHashCodes = new List<int>();
            var displayForm = CreateDisplayForm(model, objectHashCodes, true);
            var b = displayForm.Length;
            var kb = b / 1024;
            var mb = kb / 1024;
            return string.Format("{0} byte | {1} KB | {2} MB | {3} Not built in objects | {4}", b, kb, mb, objectHashCodes.Count, displayForm);
        }

        //public static string CreateDisplayForm(object model, List<int> objectHashCodes = null, bool includeDiagnosticDetail = false)
        public static string CreateDisplayForm(object model, List<int> objectHashCodes, bool includeDiagnosticDetail)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<h2>" + model.GetType() + "</h2>");
            result.AppendLine(model.SerializeToHtml(true, 15, objectHashCodes, includeDiagnosticDetail, new List<string> { "ChildNodes", "Owner", "NodeId" }));
            return result.ToString();
        }
    }
}
