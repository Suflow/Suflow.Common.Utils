//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Suflow.Common.Utils
//{
//    public class RuntimeTestExecuter
//    {
//        public static string Execute<T>() 
//        {
//            var result = new StringBuilder();
//            var methods = typeof(T).GetMethods().Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), true) != null);
//            var instance = Activator.CreateInstance<T>();
//            foreach(var method in methods)
//            {
//                try
//                {
//                    instance.InvokeMethod(method.Name);
//                    result.AppendLine(method.Name + " : SUCCESS ");
//                }
//                catch (Exception exp)
//                {
//                    result.AppendLine(method.Name + " : ERROR : " + exp.Message);
//                }
//            }
//            return result.ToString();
//        }
//    }
//}
