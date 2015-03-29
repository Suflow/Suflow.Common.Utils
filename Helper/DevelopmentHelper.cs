using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suflow.Common.Utils
{
    public class DevelopmentHelper
    {
        public class Error
        {
            public string ErrorMessage { get; set; }
            public string Cause { get; set; }
            public string Solution { get; set; }            
        }

        public static List<Error> Errors = new List<Error>
        {
            new Error()
            {
                ErrorMessage = "Could not load file or assembly 'System.Web.Razor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)",
                Cause = "Dll in bin was 2, I as referencing to 2. But in web.config, in runtime assemblyBinding, it was referencing to 3.",
                Solution = "Fix the web.config and user version 2.0.0.0"
            }
        };
    }
}
