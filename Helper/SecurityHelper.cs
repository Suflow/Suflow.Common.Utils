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
using System.Web;

namespace Suflow.Common.Utils
{
    public class SecurityHelper
    {
        public static string RandomSalt()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public static string RandomPassword()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        public static bool IsPotentialXssAttackQuery(HttpRequest request)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(request.QueryString.ToString());
            stringBuilder.Append(request.Form.ToString());
            foreach (HttpCookie cookie in request.Cookies)
                stringBuilder.Append(cookie.Value);
            return stringBuilder.ToString().IsPotentialXssAttack();
        }
    }
}
