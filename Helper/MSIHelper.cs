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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
//using WindowsInstaller.Installer;

namespace Suflow.Common.Utils {
    public class MSIHelper {
        //Complete list: http://support.microsoft.com/kb/290158
        public static Dictionary<int, string> ErrorCodes = new Dictionary<int, string>()
        {
            {1602, "User cancel installation."},
            {1603, "Fatal error during installation."},
            {1618, "Another installation is already in progress. Complete that installation before proceeding with this install."},
            {1622, "Error opening installation log file. Verify that the specified log file location exists and is writable."},
            {1638, "Another version of this product is already installed. Installation of this version cannot continue. To configure or remove the existing version of this product, go to uninstall page"},
        };

        /// <summary>
        /// not tested yet!!
        /// </summary>
        /// <param name="msiPath"></param>
        /// <param name="destinationFolder"></param>
        public static void ExtractMSIToAFolder(string msiPath, string destinationFolder) {
            var args = "/a \"" + msiPath + "\" /qb TARGETDIR=\"" + destinationFolder + "\\";
            var process = ProcessHelper.Run("msiexec.exe", args, Path.GetDirectoryName(msiPath));
        }

        public static void Run(string action, string msiFile, bool installInQuiteMode = false,
         string installationDirectory = null, string logFileLocation = null, string[] selectedFeaturesToInstall = null) {
            var args = string.Format("{0} \"{1}\"", action, msiFile);
            try {
                if (installInQuiteMode) {
                    args += " /qn ";
                }
                if (installationDirectory != null) {
                    args += " INSTALLLOCATION=\"" + installationDirectory + "\" ";
                }
                if (logFileLocation != null) {
                    args += " /l*v \"" + logFileLocation + "\" "; //x - Extra debugging information
                }
                if (selectedFeaturesToInstall != null && selectedFeaturesToInstall.Any()) {
                    args += " ADDLOCAL=";
                    foreach (var selectedFeatureToInstall in selectedFeaturesToInstall) {
                        args += selectedFeatureToInstall + ",";
                    }
                    args = args.Substring(0, args.Length - 1) + " ";
                }

                var process = ProcessHelper.Run("msiexec.exe", args, Path.GetDirectoryName(msiFile));
                if (process.ExitCode != 0) {
                    var message = "";
                    ErrorCodes.TryGetValue(process.ExitCode, out message);
                    Console.WriteLine(string.Format("{0} exited with code: {1}. {2} ", msiFile, process.ExitCode, message));
                }
            }
            catch (Exception e) {
                Console.WriteLine("Error while running msiFile. " + e.Message);
            }
        }

        //public static string Get(string msiFile, string table = "Property", string returnColumnName = "Value", string filterColumnName = "Property", string filterColumnValue = "ProductCode")
        //{
        //    using (var db = new Database(msiFile))
        //    {
        //        var query = String.Format("SELECT `{0}` FROM `{1}` WHERE `{2}` = '{3}'", returnColumnName, table, filterColumnName, filterColumnValue);
        //        return db.ExecuteScalar(query) as string;
        //    }
        //}

        //public static void Set(string msiFile, string table = "Property", string updateColumnName = "Value", string updateColumnValue = null, string filterColumnName = "Property", string filterColumnValue = "ProductCode")
        //{
        //    using (var db = new Database(msiFile, DatabaseOpenMode.Direct))
        //    {
        //        var query = String.Format("UPDATE `{0}` SET `{1}` = '{2}' WHERE `{3}` = '{4}'", table, updateColumnName, updateColumnValue, filterColumnName, filterColumnValue);
        //        db.Execute(query);
        //    }
        //}

        //public static string GetRevisionNumber(string msiFile)
        //{
        //    using (var db = new Database(msiFile, DatabaseOpenMode.Direct))
        //    {
        //        return db.SummaryInfo.RevisionNumber;
        //    }
        //}

        public static void SetRevisionNumber(string msiFile, string revisionNumberGuid) {
            try {
                var args = string.Format(" \"{0}\" /v \"{1}\"", msiFile, revisionNumberGuid);
                var process = Process.Start("msiinfo.exe", args);
                process.WaitForExit(500);
            }
            catch (Exception e) {
                Console.WriteLine("Error while updating revision number", e.Message);
            }
        }

        public static string GetNewGuid() {
            return string.Format("{{{0}}}", Guid.NewGuid().ToString().ToUpper());
        }
    }
}