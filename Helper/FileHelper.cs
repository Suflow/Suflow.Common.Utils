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
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace Suflow.Common.Utils {

    public class FileHelper {
        public static bool OpenFileUsingExplorer(string path) {
            if (File.Exists(path)) {
                Process prc = new Process();
                prc.StartInfo.FileName = path;
                prc.Start();
                return true;
            }
            return false;
        }

        public static string SelectFileToSave(string propsedFileName = "untitled", string filter = "All files (*.*)|*.*", string defaultExt = "*.*") {
            var saveFileDialog = new SaveFileDialog {
                FileName = propsedFileName,
                Filter = filter,
                DefaultExt = defaultExt,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = saveFileDialog.ShowDialog();
            if (result != null && result == true) {
                return saveFileDialog.FileName;
            }
            return null;
        }

        public static string SelectFileToOpen(string propsedFileName = "untitled", string filter = "All files (*.*)|*.*", string defaultExt = "*.*") {
            var saveFileDialog = new OpenFileDialog {
                FileName = propsedFileName,
                Filter = filter,
                DefaultExt = defaultExt,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = saveFileDialog.ShowDialog();
            if (result != null && result == true) {
                return saveFileDialog.FileName;
            }
            return null;
        }

        public static string ReadAllText(string fileToOpen) {
            return File.ReadAllText(fileToOpen);
        }

        public static void WriteAllText(string fileToSave, string text, bool createIfNotExists) {
            if (createIfNotExists) {
                if (!File.Exists(fileToSave)) {
                    var directory = Path.GetDirectoryName(fileToSave);
                    if (!Directory.Exists(directory)) {
                        Directory.CreateDirectory(directory);
                    }
                }
            }
            File.WriteAllText(fileToSave, text);
        }

        public static void Watch(string path, bool includeSubDirectories, Action<FileSystemEventArgs> changed = null, Action<FileSystemEventArgs> created = null, Action<FileSystemEventArgs> deleed = null, Action<RenamedEventArgs> renamed = null) {
            var watcher = new System.IO.FileSystemWatcher();

            watcher.Path = path;

            watcher.NotifyFilter = NotifyFilters.LastAccess |
                         NotifyFilters.LastWrite |
                         NotifyFilters.FileName |
                         NotifyFilters.DirectoryName;

            watcher.IncludeSubdirectories = includeSubDirectories;

            if (changed != null)
                watcher.Changed += (s, e) => changed(e);

            if (created != null)
                watcher.Created += (s, e) => created(e);

            if (deleed != null)
                watcher.Deleted += (s, e) => deleed(e);

            if (renamed != null)
                watcher.Renamed += (s, e) => renamed(e);

            watcher.EnableRaisingEvents = true;
        }
    }
}
