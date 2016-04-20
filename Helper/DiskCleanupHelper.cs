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

namespace Suflow.Common.Utils
{
   

    /// <summary>
    /// Has methods to clean the unnecessary files from disk
    /// </summary>
    public class DiskCleanupHelper
    {
        /// <summary>
        /// C:\Users\[User]\AppData\Local\Microsoft\Windows\History
        /// </summary>
        public static void CleanupHistoryFolder()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.History);
            InnerCleanup(path);
        }

        /// <summary>
        ///  C:\Users\[User]\AppData\Local\Microsoft\Windows\Temporary Internet Files
        /// </summary>
        public static void CleanupInternetCache()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            var di = new DirectoryInfo(path);
            FileAttributes Attr = di.Attributes;
            di.Attributes = FileAttributes.Normal;
            InnerCleanup(path);

            InternetCacheCleanupHelper.RunCleanup();
        }

        /// <summary>
        /// C:\Users\[User]\AppData\Roaming\Microsoft\Windows\Recent
        /// </summary>
        public static void CleanupPrefetcholder()
        {
            String path = Environment.ExpandEnvironmentVariables("%SYSTEMROOT%") + "\\Prefetch";
            InnerCleanup(path);
        }

        /// <summary>
        /// C:\Users\[User]\AppData\Roaming\Microsoft\Windows\Recent
        /// </summary>
        public static void CleanupRecentFolder()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            InnerCleanup(path);
        }

        /// <summary>
        /// The GetTempPath function checks for the existence of environment variables in the following order and uses the first path found:
        ///The path specified by the TMP environment variable.
        ///The path specified by the TEMP environment variable.
        ///The path specified by the USERPROFILE environment variable.
        ///The Windows directory.
        ///
        ///C:\Users\[User]\AppData\Local\Temp\
        /// </summary>
        public static void CleanupTemporaryFolder()
        {
            string path = System.IO.Path.GetTempPath();
            InnerCleanup(path);
        }

        /// <summary>
        /// Will try to clean up as much as possible from the given directory path
        /// </summary>
        private static void InnerCleanup(string directoryPath)
        {
            var di = new DirectoryInfo(directoryPath);
            var files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                }
            }
            var directories = di.GetDirectories();
            foreach (DirectoryInfo dir in directories)
            {
                try
                {
                    dir.Delete(true);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static class InternetCacheCleanupHelper
        {
            public static void RunCleanup()
            {
                try { InnerKillProcess("iexplore"); }
                catch { }//Need to stop incase they have locked the files we want to delete
                try { InnerKillProcess("FireFox"); }
                catch { }//Need to stop incase they have locked the files we want to delete
                string RootPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToLower().Replace("documents", "");
                InnerCleanupFolder(RootPath, @"AppData\Roaming\Macromedia\Flash Player\#SharedObjects", false);
                InnerCleanupFolder(RootPath, @"AppData\Roaming\Macromedia\Flash Player\macromedia.com\support\flashplayer\sys\#local", false);
                //InnerCleanupFolder(RootPath, @"AppData\Local\Temporary Internet Files", false);//Not working
                InnerCleanupFolder("", Environment.GetFolderPath(Environment.SpecialFolder.Cookies), true);
                InnerCleanupFolder("", Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), true);
                InnerCleanupFolder("", Environment.GetFolderPath(Environment.SpecialFolder.History), true);
                InnerCleanupFolder(RootPath, @"AppData\Local\Microsoft\Windows\Wer", true);
                InnerCleanupFolder(RootPath, @"AppData\Local\Microsoft\Windows\Caches", false);
                InnerCleanupFolder(RootPath, @"AppData\Local\Microsoft\WebsiteCache", false);
                InnerCleanupFolder(RootPath, @"AppData\Local\Temp", false);
                InnerCleanupFolder(RootPath, @"AppData\LocalLow\Microsoft\CryptnetUrlCache", false);
                InnerCleanupFolder(RootPath, @"AppData\LocalLow\Apple Computer\QuickTime\downloads", false);
                InnerCleanupFolder(RootPath, @"AppData\Local\Mozilla\Firefox\Profiles", false);
                InnerCleanupFolder(RootPath, @"AppData\Roaming\Microsoft\Office\Recent", false);
                InnerCleanupFolder(RootPath, @"AppData\Roaming\Adobe\Flash Player\AssetCache", false);
                if (Directory.Exists(RootPath + @"\AppData\Roaming\Mozilla\Firefox\Profiles"))
                {
                    string FireFoxPath = RootPath + @"AppData\Roaming\Mozilla\Firefox\Profiles\";
                    InnerCleanupFirefoxFiles(FireFoxPath);
                    foreach (string SubPath in Directory.GetDirectories(FireFoxPath))
                    {
                        InnerCleanupFirefoxFiles(SubPath + "\\");
                    }
                }
            }

            private static void InnerCleanupFolder(string RootPath, string Path, bool Recursive)
            {
                string FullPath = RootPath + Path + "\\";
                if (Directory.Exists(FullPath))
                {
                    DirectoryInfo DInfo = new DirectoryInfo(FullPath);
                    FileAttributes Attr = DInfo.Attributes;
                    DInfo.Attributes = FileAttributes.Normal;
                    foreach (string FileName in Directory.GetFiles(FullPath))
                    {
                        InnerCleanupFile(FileName);
                    }
                    if (Recursive)
                    {
                        foreach (string DirName in Directory.GetDirectories(FullPath))
                        {
                            InnerCleanupFolder("", DirName, true);
                            try { Directory.Delete(DirName); }
                            catch { }
                        }
                    }
                    DInfo.Attributes = Attr;
                }
            }

            private static void InnerCleanupFile(string FileName)
            {
                if (File.Exists(FileName))
                {
                    try { File.Delete(FileName); }
                    catch { }//Locked by something and you can forget trying to delete index.dat files this way
                }
            }

            private static void InnerCleanupFirefoxFiles(string FireFoxPath)
            {
                InnerCleanupFile(FireFoxPath + "cookies.sqlite");
                InnerCleanupFile(FireFoxPath + "content-prefs.sqlite");
                InnerCleanupFile(FireFoxPath + "downloads.sqlite");
                InnerCleanupFile(FireFoxPath + "formhistory.sqlite");
                InnerCleanupFile(FireFoxPath + "search.sqlite");
                InnerCleanupFile(FireFoxPath + "signons.sqlite");
                InnerCleanupFile(FireFoxPath + "search.json");
                InnerCleanupFile(FireFoxPath + "permissions.sqlite");
            }

            private static void InnerKillProcess(string ProcessName)
            {
                //We ned to kill Internet explorer and Firefox to stop them locking files
                ProcessName = ProcessName.ToLower();
                foreach (Process P in Process.GetProcesses())
                {
                    if (P.ProcessName.ToLower().StartsWith(ProcessName))
                        P.Kill();
                }
            }
        }
    }
}
