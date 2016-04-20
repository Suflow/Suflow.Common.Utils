using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Suflow.Common.Utils {

    /// <summary>
    /// This helper will create ToolboxBitmapAttribute, from image, by calling private constructor or ToolboxBitmapAttribute, using reflection.
    /// It is necessary because constructor of ToolboxBitmapAttribute that receives images is private.
    /// Why do we need ToolboxBitmapAttribute?
    /// To show the icon in the toolbox.
    /// </summary>
    public class ToolboxBitmapAttributeHelper {

        private static ConstructorInfo _toolboxBitmapAttributePrivateCtor;

        /// <summary>
        /// http://www.dotnetframework.org/default.aspx/4@0/4@0/DEVDIV_TFS/Dev10/Releases/RTMRel/ndp/fx/src/CommonUI/System/Drawing/ToolboxBitmapAttribute@cs/1305376/ToolboxBitmapAttribute@cs    
        /// private ToolboxBitmapAttribute(Image smallImage, Image largeImage) { ... }
        /// </summary>
        public static ToolboxBitmapAttribute CreateToolboxBitmapAttribute(Image smallImage, Image largeImage) {
            if (_toolboxBitmapAttributePrivateCtor == null) {
                _toolboxBitmapAttributePrivateCtor = typeof(ToolboxBitmapAttribute).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Image), typeof(Image) }, null);
            }
            return (ToolboxBitmapAttribute)_toolboxBitmapAttributePrivateCtor.Invoke(new[] { smallImage, largeImage });
        }
    }
}
