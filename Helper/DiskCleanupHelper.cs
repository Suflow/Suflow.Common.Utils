using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Suflow.Common.Utils
{
    /// <summary>
    /// Clean the internet cache
    /// </summary>
    public class DiskCleanupHelper
    {
        public static void Run()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            var di =  new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true); 
            }            
        }
    }
}
