﻿////////////////////////////////////////////////////////////////////////////////
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
using System.Net;
using System.Text;

namespace Suflow.Common.Utils {
    public class FtpHelper {
        public FtpWebResponse GetFtpResponse(string ftpServerIp, string ftpUserId, string ftpPassword, string method, string filePath, bool enableSSl) {
            var uriStr = "ftp://" + ftpServerIp + "/" + filePath;
            var uri = new Uri(uriStr);
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.UseBinary = true;
            request.KeepAlive = false;
            request.EnableSsl = enableSSl;
            request.Method = method;
            request.Credentials = new NetworkCredential(ftpUserId, ftpPassword);
            return (FtpWebResponse)request.GetResponse();
        }
    }
}
