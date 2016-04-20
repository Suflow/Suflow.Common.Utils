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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Suflow.Common.Utils {
    public class ImageHelper {

        public static DrawingBrush IconToDrawingBrush(Icon icon, Rect rect) {
            return new DrawingBrush(IconToImageDrawing(icon, rect));
        }

        public static ImageDrawing IconToImageDrawing(Icon icon, Rect rect) {
            return new ImageDrawing(IconToBitmapSource(icon), rect);
        }

        public static BitmapSource IconToBitmapSource(Icon icon) {
            return Bitmap2BitmapSource(icon.ToBitmap());
        }

        public static Bitmap IconToBitmap(Icon icon) {
            return icon.ToBitmap();
        }

        public static BitmapSource Bitmap2BitmapSource(Bitmap bitmap) {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            NativeMethodsHelper.DeleteObject(hBitmap);
            return bitmapSource;
        }

        public static void SaveThumbnail(Bitmap bitmap, string fileName) {
            var thumbnail = bitmap.GetThumbnailImage(100, 100, null, IntPtr.Zero);
            thumbnail.Save(fileName);
        }

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage) {
            using (MemoryStream outStream = new MemoryStream()) {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage ImageToBitmapImage(Image image) {
            var result = new BitmapImage();
            result.BeginInit();
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Bmp);
            memoryStream.Seek(0, SeekOrigin.Begin);
            result.StreamSource = memoryStream;
            result.EndInit();
            return result;
        }
    }
}
