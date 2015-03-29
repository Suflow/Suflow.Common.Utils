using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Suflow.Common.Utils
{
    public class FileHelper
    {
        public static bool OpenFileUsingExplorer(string path)
        {
            if (File.Exists(path))
            {
                Process prc = new Process();
                prc.StartInfo.FileName = path;
                prc.Start();
                return true;
            }
            return false;
        }
    }
}
