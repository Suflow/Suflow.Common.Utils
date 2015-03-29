using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using Suflow.Common.Utils;

namespace System
{
    public static class DataTableExtensions
    {      
        public static string SerializeToHtml(this DataTable table, int widthInPixel, string tableId, bool addcaption, string caption)
        {
            using (new CultureHelper("en-US", "en-US"))
            {
                if (tableId == "") tableId = new Guid().ToString();
                StringBuilder script = new StringBuilder();
                script.AppendLine("");
                script.AppendLine("<table style=\"width:" + widthInPixel + "px;\" id=\"" + tableId + "\">");
                if (addcaption)
                    script.AppendLine("<caption>" + caption + "</caption>");
                script.AppendLine("    <thead>");
                script.Append("        <tr>");
                for (int columnId = 0; columnId < table.Columns.Count; ++columnId)
                {
                    script.Append("<th>");
                    script.Append(table.Columns[columnId].Caption);
                    script.Append("</th>");
                }
                script.AppendLine("</tr>");
                script.AppendLine("    </thead>");
                script.AppendLine("    <tbody>");
                for (int rowId = 0; rowId < table.Rows.Count; ++rowId)
                {
                    script.Append("        <tr>");
                    for (int columnId = 0; columnId < table.Columns.Count; ++columnId)
                    {
                        script.Append("<td>");
                        script.Append(table.Rows[rowId][columnId]);
                        script.Append("</td>");
                    }
                    script.AppendLine("</tr>");
                }
                script.AppendLine("    </tbody>");
                script.AppendLine("</table>");
                return script.ToString();
            }
        }

    }
}
