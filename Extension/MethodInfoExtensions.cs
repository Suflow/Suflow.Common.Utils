using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace System
{
    public static class MethodInfoExtensions
    {
        public static bool ParameterMatches(this MethodInfo methodInfo, object[] args)
        {
            var result = true;
            var parameterInfos = methodInfo.GetParameters();
            var argLength = args == null ? 0 : args.Length;
            if (parameterInfos.Length != argLength)
            {
                result = false;
            }
            else
            {
                for (var index = 0; index < argLength; ++index)
                {
                    var parameterInfo = parameterInfos[index];
                    if (args != null)
                    {
                        var arg = args[index];
                        if (!parameterInfo.ParameterType.IsInstanceOfType(arg))
                            return false;
                    }
                    else return false;
                }
            }
            return result;
        }
    }
}