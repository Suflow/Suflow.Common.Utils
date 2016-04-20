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
using System.Data.OleDb;
using System.Data;
using System.Globalization;

namespace Suflow.Common.Utils
{

    /// <summary>
    /// http://csharphelper.com/blog/2016/02/make-an-excel-chart-in-c/
    /// </summary>
    public static class ExcelHelper
    {
        public static string ExcelVersion = "12.0";

        #region Private Functions

        private static OleDbConnection CreateConnection(string file, bool hasHeaders)
        {
            string connection = "Provider=Microsoft.ACE.OLEDB." + ExcelVersion + ";" +
                               "Data Source=" + file + ";" +
                               "Extended Properties=\"Excel " + ExcelVersion + ";" +
                               "HDR=" + (hasHeaders ? "Yes" : "No") + ";" +
                               "\"";
            OleDbConnection result = new OleDbConnection(connection);
            result.Open();
            return result;
        }

        private static List<String> LoadSheetNames(OleDbConnection connection)
        {
            List<String> result = new List<String>();
            DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt == null)
                return result;
            IDataReader dr = dt.CreateDataReader();
            try
            {
                while (dr.Read())
                    try
                    {
                        result.Add(((string)dr["TABLE_NAME"]).Trim());
                    }
                    catch
                    {
                    }
                return result;
            }
            finally
            {
                dr.Close();
            }
        }

        private static DataTable CreateTable(OleDbConnection connection, string cmdText, string tableName)
        {
            OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter();
            DataTable result = new DataTable(tableName.Replace("$", ""));
            OleDbCommand excelCommand = new OleDbCommand(cmdText, connection);
            excelDataAdapter.SelectCommand = excelCommand;
            excelDataAdapter.Fill(result);
            return result;
        }

        private static DataTable ImportSheet(OleDbConnection connection, string sheet)
        {
            DataTable result = CreateTable(connection, String.Format("SELECT * FROM [{0}]", sheet), sheet);
            foreach (DataRow row in result.Rows)
            {
                bool empty = true;
                foreach (DataColumn col in result.Columns)
                {
                    string columnValue = row[col.ColumnName].ToString();
                    if (!String.IsNullOrEmpty(row[col.ColumnName].ToString()))
                    {
                        row[col.ColumnName] = columnValue.Replace("'", "''");
                        empty = false;
                    }
                }
                if (empty)
                    row.Delete();
            }
            return result;
        }

        private static string GetDropTableQuery(string sheet)
        {
            return "Drop table " + sheet;
        }

        private static string GetTruncateTableQuery(string sheet)
        {
            return "TRUNCATE table " + sheet;
        }

        private static string GetCreateTableQuery(string sheet, DataTable table)
        {
            StringBuilder createTableQuery = new StringBuilder();
            createTableQuery.Append("CREATE TABLE [" + sheet + "] (");
            foreach (DataColumn column in table.Columns)
            {
                createTableQuery.Append("[" + column.ColumnName + "]");
                string columnDataType = column.DataType.ToString().ToLower();
                if (columnDataType.Contains("int"))
                    createTableQuery.Append(" int");
                else if (columnDataType.Contains("decimal") || columnDataType.Contains("double") || columnDataType.Contains("float"))
                    createTableQuery.Append(" float");
                else if (columnDataType.Contains("datetime"))
                    createTableQuery.Append(" datetime");
                else createTableQuery.Append(" text");
                createTableQuery.Append(", ");
            }
            createTableQuery.Remove(createTableQuery.Length - 2, 2);
            createTableQuery.Append(") ");
            return createTableQuery.ToString();
        }

        private static string GetInsertQuery(string sheet, DataRow row)
        {
            var insertCommandQuery = "INSERT INTO [" + sheet + "] ({0}) VALUES ({1})";
            var firstPart = new StringBuilder();
            var secondPart = new StringBuilder();
            foreach (DataColumn column in row.Table.Columns)
            {
                firstPart.Append("[" + column.ColumnName + "]" + ", ");
                var columnDataType = column.DataType.ToString().ToLower();
                if (columnDataType.Contains("int"))
                    secondPart.Append(row[column].ToString());
                //else if (columnDataType.Contains("single"))
                //    secondPart.Append(((Single)row[column]).ToString(CultureInfo.GetCultureInfo("pt-PT")));                
                else if (columnDataType.Contains("decimal") || columnDataType.Contains("double") ||
                         columnDataType.Contains("float"))
                    secondPart.Append(((double)row[column]).ToString(CultureInfo.GetCultureInfo("pt-PT")));
                else if (columnDataType.Contains("datetime"))
                    secondPart.Append("'" + ((DateTime)row[column]).ToString(CultureInfo.GetCultureInfo("pt-PT")) +
                                      "'");
                else secondPart.Append("'" + row[column].ToString().Replace("''", "'").Replace("'", "''") + "'");
                secondPart.Append(", ");
            }
            firstPart.Remove(firstPart.Length - 2, 2);
            secondPart.Remove(secondPart.Length - 2, 2);

            return string.Format(insertCommandQuery, firstPart, secondPart);
        }

        private static void ExecuteNonQuery(string query, OleDbConnection conn, bool throwException)
        {
            try
            {
                var oleDbCommand = new OleDbCommand(query, conn);
                oleDbCommand.ExecuteNonQuery();
                return;
            }
            catch (Exception e)
            {
                if (throwException)
                    throw new Exception("ExcelHelper:: Error in ExecuteNonQuery. Query is: " + query + ". " + e.Message);
            }
            return;
        }

        #endregion

        public static DataSet ImportFile(string file)
        {
            return ImportFile(file, true);
        }

        public static DataSet ImportFile(string file, bool hasHeaders)
        {
            using (OleDbConnection conn = CreateConnection(file, hasHeaders))
            {
                DataSet result = new DataSet(file);
                foreach (string sheet in LoadSheetNames(conn))
                {
                    try
                    {
                        result.Tables.Add(ImportSheet(conn, sheet));
                    }
                    catch
                    {
                    }
                }
                return result;
            }
        }

        public static DataTable ImportSheet(string file, int sheet)
        {
            return ImportSheet(file, sheet, true);
        }

        public static DataTable ImportSheet(string file, int sheet, bool hasHeaders)
        {
            using (OleDbConnection conn = CreateConnection(file, hasHeaders))
                return ImportSheet(conn, LoadSheetNames(conn)[sheet]);
        }

        public static DataTable ImportSheet(string file, string sheet)
        {
            return ImportSheet(file, sheet, true);
        }

        public static DataTable ImportSheet(string file, string sheet, bool hasHeaders)
        {
            using (OleDbConnection conn = CreateConnection(file, hasHeaders))
                return ImportSheet(conn, sheet);
        }

        public static void ExportToSheet(string file, string sheet, DataTable table)
        {
            ExportToSheet(file, sheet, table, true);
        }

        private static string NumberToExcelColumnChar(int columnIndex)
        {
            string result = "";
            while (columnIndex > 26)
            {
                var mod = columnIndex % 26;
                result = ((Char)('A' + mod - 1)) + result;
                columnIndex = columnIndex / 26;
            }
            result = ((Char)('A' + columnIndex - 1)) + result;
            return result;
        }

        public static string CreateSheetXml(DataTable table, string locale)
        {
            StringBuilder builder = new StringBuilder();

            //header
            builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" + Environment.NewLine);
            builder.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><dimension ref=\"A1:AJ5\"/><sheetViews><sheetView tabSelected=\"1\" workbookViewId=\"0\" rightToLeft=\"false\"><selection activeCell=\"C5\" sqref=\"C5\"/></sheetView></sheetViews><sheetFormatPr defaultRowHeight=\"15\"/><sheetData>");

            //theader
            builder.Append("<row outlineLevel=\"0\" r=\"1\">");
            for (int columnIndex = 0; columnIndex < table.Columns.Count; ++columnIndex)
            {
                var currentExcelColumn = columnIndex + 1;
                string cell = NumberToExcelColumnChar(currentExcelColumn) + "1";
                builder.Append("<c r=\"" + cell + "\" s=\"9\" t=\"inlineStr\"><is><t>" + table.Columns[columnIndex].Caption + "</t></is></c>");
            }
            builder.Append("</row>");
            //tbody
            for (int rowIndex = 0; rowIndex < table.Rows.Count; ++rowIndex)
            {
                var currentExcelRow = rowIndex + 2;
                builder.Append("<row outlineLevel=\"0\" r=\"" + currentExcelRow + "\">");
                for (int columnIndex = 0; columnIndex < table.Columns.Count; ++columnIndex)
                {
                    var currentExcelColumn = columnIndex + 1;
                    string cell = NumberToExcelColumnChar(currentExcelColumn) + currentExcelRow;
                    var cellValueObj = table.Rows[rowIndex][columnIndex];
                    var columnString = "";
                    var cellValue = "";

                    DataColumn column = table.Columns[columnIndex];
                    var columnDataType = column.DataType.ToString().ToLower();
                    switch (columnDataType)
                    {
                        case "system.int16":
                        case "system.int64":
                        case "system.int32":
                            columnString = "<c r=\"{0}\" s=\"9\"><v>{1}</v></c>";
                            cellValue = cellValueObj.ToString();
                            break;

                        case "system.decimal":
                        case "system.double":
                        case "system.float":
                            columnString = "<c r=\"{0}\" s=\"9\"><v>{1}</v></c>";
                            cellValue = ((double)cellValueObj).ToString(new CultureInfo(locale));
                            break;

                        case "system.datetime":
                            columnString = "<c r=\"{0}\" s=\"10\"><v>{1}</v></c>";
                            cellValue = ((DateTime)cellValueObj).ToOADate().ToString();
                            break;

                        default:
                            columnString = "<c r=\"{0}\" s=\"9\" t=\"inlineStr\"><is><t>{1}</t></is></c>";
                            cellValue = cellValueObj.ToString().Replace("&", "&amp;");//The character '&' is reserved in XML and needs to be encoded to "&amp;"
                            break;
                    }

                    builder.Append(string.Format(columnString, cell, cellValue));
                }
                builder.Append("</row>");
            }
            //footer
            builder.Append("</sheetData><pageMargins left=\"0.7\" right=\"0.7\" top=\"0.75\" bottom=\"0.75\" header=\"0.3\" footer=\"0.3\"/><pageSetup paperSize=\"9\" orientation=\"portrait\" r:id=\"rId1\"/></worksheet>");

            return builder.ToString();
        }

        public static void ExportToSheet(string file, string sheet, DataTable table, bool hasHeaders)
        {
            using (OleDbConnection conn = CreateConnection(file, hasHeaders))
            {
                ExecuteNonQuery(GetDropTableQuery(sheet), conn, false);
                ExecuteNonQuery(GetCreateTableQuery(sheet, table), conn, false);
                ExecuteNonQuery(GetTruncateTableQuery(sheet), conn, false);
                foreach (DataRow row in table.Rows)
                    ExecuteNonQuery(GetInsertQuery(sheet, row), conn, true);
            }
        }
    }
}
