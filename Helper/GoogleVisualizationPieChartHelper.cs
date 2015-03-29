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

namespace Suflow.Common.Utils
{
    public class GoogleVisualizationPieChartHelper
    {
        private static string GetAddRowsScript(DataTable table)
        {
            StringBuilder script = new StringBuilder();
            script.AppendLine("data.addRows([");
            foreach (DataRow row in table.Rows)
            {
                script.Append(" ['" + row[0] + "'");
                script.Append("," + row[1]);
                script.AppendLine("],");
            }
            string result = script.ToString();
            if (result.LastIndexOf(',') > 0)
                result = result.Substring(0, result.LastIndexOf(','));
            return result + "]);" + Environment.NewLine;
        }
        public static string GetScript(DataTable table, int chartWidthInPixel, int chartHeightInPixel, string colors, string backgroundColor)
        {
            using (new CultureHelper("en-US", "en-US"))
            {
                string randomId = new Random().Next(0, 999999999).ToString();
                StringBuilder script = new StringBuilder();
                script.AppendLine("<script type=\"text/javascript\" src=\"http://www.google.com/jsapi\"></script>");
                script.AppendLine("<script type=\"text/javascript\">");
                script.AppendLine("google.load(\"visualization\", \"1\", {packages:[\"corechart\"]});");
                script.AppendLine("google.setOnLoadCallback(drawPieChart" + randomId + ");");
                script.AppendLine("function drawPieChart" + randomId + "() {");
                script.AppendLine("var data = new google.visualization.DataTable();");
                script.AppendLine("data.addColumn('string', 'Entity');");
                script.AppendLine("data.addColumn('number', 'Value'); ");
                script.Append(GetAddRowsScript(table));
                script.AppendLine("var chart = new google.visualization.PieChart(document.getElementById('" + randomId + "'));");
                script.AppendLine("var options = {}; ");
                script.AppendLine("options['width'] = " + chartWidthInPixel + "; ");
                script.AppendLine("options['height'] = " + chartHeightInPixel + "; ");
                script.AppendLine("options['allowHtml'] = true; ");
                if (!string.IsNullOrEmpty(backgroundColor))
                    script.AppendLine("options['backgroundColor'] = '" + backgroundColor + "'; ");
                if (!string.IsNullOrEmpty(colors))
                    script.AppendLine("options['colors'] = " + colors + "; ");

                script.AppendLine("chart.draw(data, options);");
                script.AppendLine("}");
                script.AppendLine("</script>");
                script.AppendLine("<div id=\"" + randomId + "\"></div>");
                return script.ToString();
            }
        }
    }
}
