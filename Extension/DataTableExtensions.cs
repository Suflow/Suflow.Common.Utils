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
using System.Data;
using System.Globalization;
using Suflow.Common.Utils;

namespace System {
    public static class DataTableExtensions {
        public static string SerializeToHtml(this DataTable table, int widthInPixel, string tableId, bool addcaption, string caption) {
            using (new CultureHelper("en-US", "en-US")) {
                if (tableId == "") tableId = new Guid().ToString();
                StringBuilder script = new StringBuilder();
                script.AppendLine("");
                script.AppendLine("<table style=\"width:" + widthInPixel + "px;\" id=\"" + tableId + "\">");
                if (addcaption)
                    script.AppendLine("<caption>" + caption + "</caption>");
                script.AppendLine("    <thead>");
                script.Append("        <tr>");
                for (int columnId = 0; columnId < table.Columns.Count; ++columnId) {
                    script.Append("<th>");
                    script.Append(table.Columns[columnId].Caption);
                    script.Append("</th>");
                }
                script.AppendLine("</tr>");
                script.AppendLine("    </thead>");
                script.AppendLine("    <tbody>");
                for (int rowId = 0; rowId < table.Rows.Count; ++rowId) {
                    script.Append("        <tr>");
                    for (int columnId = 0; columnId < table.Columns.Count; ++columnId) {
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
