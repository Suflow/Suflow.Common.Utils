//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Suflow.Common.Utils.Extension
//{
//    public static class WebBrowserExtensions
//    {
//        private static void InjectDisableScript(System.Windows.Controls.WebBrowser webBrowser)
//        {
//            var document = webBrowser.Document;
//            var script = document.InvokeMethod("createElement", new object[] { "SCRIPT" });
//            script.SetPropertyValue("type", "text/javascript");
//            script.SetPropertyValue("text", "function noError(){return true;} window.onerror = noError;");
//            var heads = document.InvokeMethod("getElementsByTagName", new object[] { "head" });
//            foreach (var head in heads as IEnumerable)
//            {
//                head.InvokeMethod("appendChild", new object[] { script });
//            }
//        }
//    }
//}
