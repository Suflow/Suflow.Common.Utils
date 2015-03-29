//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using FtpLib;

//namespace Suflow.Common.Utils
//{
//    public class FtpHelper
//    {
//        public FtpHelper(string host, int port, string username, string password)
//        {
 
//        }

//        void GetFile()
//        {
//            using (FtpConnection ftp = new FtpConnection("ftpserver", "username", "password"))
//            {

//                ftp.Open(); /* Open the FTP connection */
//                ftp.Login(); /* Login using previously provided credentials */

//                if (ftp.DirectoryExists("/incoming")) /* check that a directory exists */
//                    ftp.SetCurrentDirectory("/incoming"); /* change current directory */

//                if (ftp.FileExists("/incoming/file.txt"))  /* check that a file exists */
//                    ftp.GetFile("/incoming/file.txt", false); /* download /incoming/file.txt as file.txt to current executing directory, overwrite if it exists */

//                //do some processing

//                try
//                {
//                    ftp.SetCurrentDirectory("/outgoing");
//                    ftp.PutFile(@"c:\localfile.txt", "file.txt"); /* upload c:\localfile.txt to the current ftp directory as file.txt */
//                }
//                catch (FtpException e)
//                {
//                    Console.WriteLine(String.Format("FTP Error: {0} {1}", e.ErrorCode, e.Message));
//                }

//                foreach (var dir in ftp.GetDirectories("/incoming/processed"))
//                {
//                    Console.WriteLine(dir.Name);
//                    Console.WriteLine(dir.CreationTime);
//                    foreach (var file in dir.GetFiles())
//                    {
//                        Console.WriteLine(file.Name);
//                        Console.WriteLine(file.LastAccessTime);
//                    }
//                }
//            }

//        } 
//    }
//}
