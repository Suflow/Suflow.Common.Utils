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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;

namespace Suflow.Common.Utils {

    /// <summary>
    /// http://csharphelper.com/blog/2016/02/make-an-excel-chart-in-c/
    /// </summary>
    public class ExcelHelperEPPlus {

        #region Import

        /// <summary>
        /// Get datatable from excel.
        /// If worksheetName is not provided, first worksheet will be taken into consideration
        /// </summary>
        public static DataTable GetDataTableFromExcel(string filePath, string workSheetName, bool hasHeader = true, string password = null) {
            var fileContent = File.ReadAllBytes(filePath);
            return GetDataTableFromExcel(fileContent, workSheetName, hasHeader, password);
        }

        /// <summary>
        /// Receive byte[] that represents excel file
        /// If worksheetName is not provided, first worksheet will be taken into consideration
        /// </summary>        
        public static DataTable GetDataTableFromExcel(byte[] excelFileContent, string workSheetName, bool hasHeader = true, string password = null) {
            using (var memoryStream = excelFileContent.ToMemoryStream())
                return GetDataTableFromExcel(memoryStream, workSheetName, hasHeader, password);
        }

        /// <summary>
        /// If worksheetName is not provided, first worksheet will be taken into consideration
        /// </summary>
        public static DataTable GetDataTableFromExcel(MemoryStream excelFileContent, string workSheetName, bool hasHeader = true, string password = null) {
            using (var package = string.IsNullOrEmpty(password) ? new ExcelPackage(excelFileContent) : new ExcelPackage(excelFileContent, password)) {
                ExcelWorksheet workSheet = null;
                if (string.IsNullOrEmpty(workSheetName)) {
                    workSheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (workSheet == null)
                        throw new Exception("There are not any worksheet!");
                }
                else {
                    workSheetName = workSheetName.Trim();
                    workSheet = package.Workbook.Worksheets.FirstOrDefault(abc => abc.Name.ToLower() == workSheetName.ToLower());
                    if (workSheet == null)
                        throw new Exception("There are not any worksheet named" + workSheetName);
                }
                var result = InnerGetDataTable(workSheet, hasHeader);
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DataSet GetDataSetFromExcel(string filePath, bool hasHeader = true, string password = null) {
            var fileContent = File.ReadAllBytes(filePath);
            return GetDataSetFromExcel(fileContent, hasHeader, password);
        }

        /// <summary>
        /// 
        /// </summary>
        public static DataSet GetDataSetFromExcel(byte[] excelFileContent, bool hasHeader = true, string password = null) {
            using (var memoryStream = excelFileContent.ToMemoryStream())
                return GetDataSetFromExcel(memoryStream, hasHeader, password);
        }

        /// <summary>
        /// 
        /// </summary>
        public static DataSet GetDataSetFromExcel(MemoryStream excelFileContent, bool hasHeader = true, string password = null) {
            var dataSet = new DataSet();
            using (var package = string.IsNullOrEmpty(password) ? new ExcelPackage(excelFileContent) : new ExcelPackage(excelFileContent, password)) {
                foreach (var workSheet in package.Workbook.Worksheets) {
                    DataTable table = InnerGetDataTable(workSheet, hasHeader);
                    dataSet.Tables.Add(table);
                }
            }
            return dataSet;
        }

        /// <summary>
        /// Get cell value
        /// </summary>
        private static string InnerGetCellValue(ExcelWorksheet workSheet, int row, int column) {
            var cell = workSheet.Cells[row, column];
            object value = null;
            if (cell.Merge) {
                value = cell.Value;
                var i = row - 1;
                while (i > 0 && string.IsNullOrWhiteSpace(value == null ? string.Empty : value.ToString()) && workSheet.Cells[i, column, row, column].Merge) {
                    value = workSheet.Cells[i, column].Value;
                    i--;
                }
            }
            else {
                value = cell.Value;
            }
            return value == null ? string.Empty : value.ToString().Trim();
        }

        /// <summary>
        /// Get data table
        /// </summary>
        private static DataTable InnerGetDataTable(ExcelWorksheet workSheet, bool hasHeader) {
            //Variables
            var result = new DataTable(workSheet.Name);
            var columnCount = workSheet.Dimension.End.Column;
            var rowCount = workSheet.Dimension.End.Row;
            //Header
            for (var columnIndex = 1; columnIndex <= columnCount; ++columnIndex) {
                var columnName = hasHeader ? InnerGetCellValue(workSheet, 1, columnIndex) : string.Format("Column {0}", columnIndex);
                result.Columns.Add(columnName);
            }
            //Content
            var startRowIndex = hasHeader ? 2 : 1;
            for (var rowIndex = startRowIndex; rowIndex <= rowCount; ++rowIndex) {
                var row = result.Rows.Add();
                for (var columnIndex = 1; columnIndex <= columnCount; ++columnIndex) {
                    var value = InnerGetCellValue(workSheet, rowIndex, columnIndex);
                    if (!string.IsNullOrEmpty(value))
                        row[columnIndex - 1] = value;
                }
            }
            return result;
        }

        #endregion

        #region Export

        public static byte[] GetExcelFromDataSet(DataSet dataSet, bool addHeader = true, string password = null) {
            using (var package = new ExcelPackage()) {
                foreach (DataTable dataTable in dataSet.Tables) {
                    InnerSaveDataTable(package, dataTable, addHeader);
                }
                if (!string.IsNullOrEmpty(password))
                    package.GetAsByteArray(password);
                return package.GetAsByteArray();
            }
        }

        public static byte[] GetExcelFromDataTable(DataTable dataTable, bool addHeader = true, string password = null) {
            using (var package = new ExcelPackage()) {
                InnerSaveDataTable(package, dataTable, addHeader);
                if (!string.IsNullOrEmpty(password))
                    package.GetAsByteArray(password);
                return package.GetAsByteArray();
            }
        }

        private static void InnerSaveDataTable(ExcelPackage package, DataTable dataTable, bool addHeader) {
            var workSheetName = dataTable.TableName;
            var workSheet = package.Workbook.Worksheets.Add(workSheetName);
            workSheet.Cells.LoadFromDataTable(dataTable, addHeader);
        }

        #endregion

    }
}
