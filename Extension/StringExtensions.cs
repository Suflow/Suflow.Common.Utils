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
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System;
using System.IO.Compression;
using System.Security.Cryptography;

namespace System
{
    public static class StringExtensions
    {
        #region Regex

        private const string _regexUrlExpression = @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$";
        private const string _regexPotentialXssAttackExpression = "(http(s)*(%3a|:))|(ftp(s)*(%3a|:))|(javascript)|(alert)|(((\\%3C) <)[^\n]+((\\%3E) >))";
        private const string _regexNumberExpression = "^[+-]?([0-9]*.)?[0-9]+$";
        private const string _regexIntegerExpression = "^[+-]?[0-9]+$";
        private const string _regexEmailExpression = @"^(([^<>()[\]\\.,:\s@\""]+"
                                                  + @"(\.[^<>()[\]\\.,:\s@\""]+)*)|(\"".+\""))@"
                                                  + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                                  + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                                  + @"[a-zA-Z]{2,}))$";

        private static readonly Regex _regexUrl = new Regex(_regexUrlExpression, RegexOptions.IgnoreCase);
        private static readonly Regex _regexPotentialXssAttack = new Regex(_regexPotentialXssAttackExpression, RegexOptions.IgnoreCase);
        private static readonly Regex _regexNumber = new Regex(_regexNumberExpression, RegexOptions.IgnoreCase);
        private static readonly Regex _regexInteger = new Regex(_regexIntegerExpression, RegexOptions.IgnoreCase);
        private static readonly Regex _regexEmail = new Regex(_regexEmailExpression, RegexOptions.IgnoreCase);

        #endregion

        public static byte[] ToByteArray(this string str)
        {
            var encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static string ToHexString(this string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public static MemoryStream ToMemoryStream(this string str, Encoding encoding)
        {
            var encodedString = encoding.GetBytes(str);
            var memoryStream = new MemoryStream(encodedString);
            memoryStream.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        public static bool IsUrl(this string str)
        {
            return _regexUrl.IsMatch(str);
        }

        public static bool IsPotentialXssAttack(this string str)
        {
            return _regexPotentialXssAttack.IsMatch(str);
        }

        public static bool IsNumber(this string str)
        {
            return _regexNumber.IsMatch(str);
        }

        public static bool IsInteger(this string str)
        {
            return _regexInteger.IsMatch(str);
        }

        public static bool IsEmail(this string str)
        {
            return _regexEmail.IsMatch(str);
        }

        public static string Quote(this string str)
        {
            return "\"" + str + "\"";
        }

        public static string Truncate(this string str, int maxLength)
        {
            if (str.Length > maxLength)
                return str.Substring(0, maxLength);
            else
                return str;
        }

        public static string TruncateWithEllipsis(this string str, int maxLength)
        {
            if (str.Length > (maxLength - 3))
                return String.Format("{0}...", Truncate(str, maxLength - 3));
            else
                return str;
        }

        /// <summary>
        /// Subdomain complaint name should be:
        /// Be between 1 and 63 characters long (64 or more is too long)
        /// Not contain any whitespace at all
        /// Contain only lowercase** characters a-z or the dash character -
        /// Not start with or end with a dash -
        /// </summary>
        public static string GetSubdomainComplainName(this string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
                if ((str[i] >= 'A' && str[i] <= 'z') || (str[i] == '-') || (str[i] >= '0' && str[i] <= '9'))
                    sb.Append(str[i]);
            var result = sb.ToString();
            result = result.Trim(new[] { '-' });
            result = result.Substring(0, Math.Min(63, result.Length)).ToLower();
            return result;
        }


        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
                if ((str[i] >= '0' && str[i] <= '9') || (str[i] >= 'A' && str[i] <= 'z' || (str[i] == '.' || str[i] == '_')))
                    sb.Append(str[i]);
            return sb.ToString();
        }

        public static string Zip(this string str)
        {
            using (var mStream = new MemoryStream())
            {
                using (var dStream = new DeflateStream(mStream, CompressionMode.Compress))
                {
                    using (var sWRiter = new StreamWriter(dStream, Encoding.UTF8))
                    {
                        sWRiter.Write(str);
                    }
                }
                return Convert.ToBase64String(mStream.ToArray());
            }
        }

        public static string UnZip(this string str)
        {
            var inputBuffer = Convert.FromBase64String(str);
            using (var mStream = new MemoryStream(inputBuffer))
            {
                using (var dStream = new DeflateStream(mStream, CompressionMode.Decompress))
                {
                    using (var sReader = new StreamReader(dStream, Encoding.UTF8))
                    {
                        return sReader.ReadToEnd();
                    }
                }
            }
        }

        public static string CalculateMD5Hash(this string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
