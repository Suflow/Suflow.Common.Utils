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
