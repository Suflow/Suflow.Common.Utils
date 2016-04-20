using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Suflow.Common.Utils {
    public class ClipboardHelper {
        public static void CopyStringToClipboard(string text) {
            Clipboard.SetText(text);
        }
    }
}
