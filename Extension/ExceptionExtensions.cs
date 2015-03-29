using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System
{
    public static class ExceptionExtensions
    {
        public static string FullMessage(this Exception exception)
        {
            return MessageWithStackTrace(exception);
        }

        public static string MessageWithStackTrace(this Exception exception)
        {
            StringBuilder result = new StringBuilder();
            while (exception != null)
            {
                result.AppendLine(exception.Message);
                result.AppendLine(exception.StackTrace);
                result.AppendLine("");
                exception = exception.InnerException;
            }
            return result.ToString();
        }
    }
}