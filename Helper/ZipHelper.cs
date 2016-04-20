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
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;
using Ionic.Zip;
namespace Suflow.Common.Utils {

    /// <summary>
    /// Zip helper
    /// </summary>
    public class ZipHelper {

        /// <summary>
        /// Compress a files
        /// </summary>
        public static byte[] Compress(string fileFullPathToCompress, string password = null) {
            return Compress(new[] { fileFullPathToCompress }, password);
        }

        /// <summary>
        /// Compress the files
        /// </summary>
        public static byte[] Compress(string[] fileFullPathsToCompress, string password = null) {
            using (ZipFile zip = new ZipFile()) {
                if (!string.IsNullOrEmpty(password))
                    zip.Password = password;
                foreach (var fileToCompress in fileFullPathsToCompress) {
                    zip.AddFile(fileToCompress);
                }
                var memoryStream = new MemoryStream();
                zip.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Compress the files
        /// </summary>
        public static byte[] Compress(string fileName, byte[] fileContent, string password = null) {
            var dictionary = new Dictionary<string, byte[]>();
            dictionary.Add(fileName, fileContent);
            return Compress(dictionary, password);
        }

        /// <summary>
        /// Compress the files
        /// </summary>
        public static byte[] Compress(Dictionary<string, byte[]> filesToCompress, string password = null) {
            using (ZipFile zip = new ZipFile()) {
                if (!string.IsNullOrEmpty(password))
                    zip.Password = password;
                foreach (var fileToCompress in filesToCompress) {
                    zip.AddEntry(fileToCompress.Key, fileToCompress.Value);
                }
                var memoryStream = new MemoryStream();
                zip.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Extract zip
        /// </summary>
        public static Dictionary<string, byte[]> Extract(string zipFileToRead, string password = null) {
            var byteArray = File.ReadAllBytes(zipFileToRead);
            return Extract(byteArray, password);
        }


        /// <summary>
        /// Extract zip
        /// </summary>
        public static Dictionary<string, byte[]> Extract(byte[] zipFileToRead, string password = null) {
            var zipStream = zipFileToRead.ToMemoryStream();
            return Extract(zipStream, password);
        }


        /// <summary>
        /// Extract zip
        /// </summary>
        public static Dictionary<string, byte[]> Extract(Stream zipFileToReadStream, string password = null) {
            var result = new Dictionary<string, byte[]>();
            ReadOptions readOptions = new ReadOptions();
            using (var zip = ZipFile.Read(zipFileToReadStream)) {
                if (!string.IsNullOrEmpty(password))
                    zip.Password = password;
                foreach (var entry in zip.Entries) {
                    var memoryStream = new MemoryStream();
                    entry.Extract(memoryStream);
                    result.Add(entry.FileName, memoryStream.ToByteArray());
                }
            }
            return result;
        }

    }
}
