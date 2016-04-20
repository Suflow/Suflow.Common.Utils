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
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Management;
using System.IO;
using System.Threading;

namespace Suflow.Common.Utils {
    public class ProcessHelper {

        private static List<string> GetEssentialProcesses() {
            return new List<string>
            {
                "idle",
                "system",
                "wininit",
                //"explorer",
                //"taskmgr",
                "spoolsv",
                "lsm",//(Local Session Manager).
                "lsass",//(Local Security Authentication Subsystem Server).
                "csrss",//Client/Server Runtime Subsystem
                "smss",//Session Manager Subsystem
                "winlogon",
                "svchost",
                "services",//(Service Control Manager or SCM).
                //"msmpeng", // Antimalware Service Executable
                //"nissrv"//Microsoft Network Inspection System
                //"nvvsvc", //NVIDIA Driver Helper Service or NVIDIA Display Driver Service
                //"iastordatamgrsvc" // Intel 
                "sihost",
            };
        }

        public class ProcessHelperProcessInfo {
            public string Comment { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public UInt32 PageFaults { get; set; }
            public UInt32 PeakPageFileUsage { get; set; }
            public string Arguments { get; set; }

            public ProcessHelperProcessInfo(Process process) {
                try {
                    Name = process.ProcessName;
                    var searcher = new ManagementObjectSearcher("Select * FROM Win32_Process where ProcessId = " + process.Id);
                    var mObject = searcher.Get();
                    foreach (var obj in mObject) {
                        Name = obj["Name"].ToString();
                        Location = obj["ExecutablePath"].ToString();
                        Arguments = obj["CommandLine"].ToString();
                        PageFaults = (UInt32)obj["PageFaults"];
                        PeakPageFileUsage = (UInt32)obj["PeakPageFileUsage"];
                        break;
                    }
                    Description = process.MainModule.FileVersionInfo.FileDescription;
                }
                catch (Exception) {
                }
            }

        }

        public static List<ProcessHelperProcessInfo> GetProcesses() {
            var result = new List<ProcessHelperProcessInfo>();
            var processes = Process.GetProcesses();
            foreach (var process in processes) {
                var info = new ProcessHelperProcessInfo(process);
                result.Add(info);
            }
            return result;
        }

        public static List<ProcessHelperProcessInfo> KillUnnecessaryProcess(params string[] processesToKeep) {

            var result = new List<ProcessHelperProcessInfo>();

            var processes = Process.GetProcesses();
            var necessaryProcesses = GetEssentialProcesses();
            foreach (var necessaryProcess in processesToKeep)
                necessaryProcesses.Add(necessaryProcess.ToLower());

            var logFile = Path.GetTempPath() + "Suflow.Common.Utils.KillUnnecessaryProcess.txt";
            File.Delete(logFile);

            foreach (var process in processes) {
                var info = new ProcessHelperProcessInfo(process);
                result.Add(info);                
                try {
                    if (!necessaryProcesses.Contains(process.ProcessName.ToLower())) {
                        var message = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " - Lets kill " + process.ProcessName + Environment.NewLine;
                        File.AppendAllText(logFile, message);
                        process.Kill();                        
                        info.Comment = "Success - Killed";

                    }
                    else {
                        info.Comment = "Success - Keep";
                    }
                }
                catch (Exception e) {
                    info.Comment = "Failed - " + e.Message;
                }
            }
            //Process.Start("explorer.exe");
            return result;
        }

        public static bool IsRunningElevated() {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null)
                return false;
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static Process Run(string process, string args, string directory) {
            return null;
        }
    }
}
