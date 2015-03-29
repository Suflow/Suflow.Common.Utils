using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Suflow.Common.Utils
{
    public class FolderHelper
    {
        public static bool OpenFolderUsingExplorer(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    path = (new FileInfo(path)).DirectoryName;
                }

                if (!Directory.Exists(path))
                    path = Directory.GetCurrentDirectory();

                Process prc = new Process();
                prc.StartInfo.FileName = path;
                prc.Start();
                return true;
            }
            catch
            {
                return false;
            }

        }

      
    }
}
