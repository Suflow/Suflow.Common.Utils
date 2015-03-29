using System;
using System.Diagnostics;
using System.Text;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Suflow.Common.Utils
{
    // HWND -> int
    // LPTSTR  -> string
    // UINT -> uint
    // HANDLE -> IntPtr
    // DWORD -> unint
    // LPDWORD -> IntPtr
    public class NativeMethodsHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern int GetParent(int hWnd);

        /// <summary>
        /// GetWindowModuleFileName is restricted to the calling process, it will not work if you pass a HWND belonging to another.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetWindowModuleFileName(int hwnd, StringBuilder lpszFileName, uint cchFileNameMax);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(int hWnd, out uint lpdwProcessId);

        // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(int hWnd, int processId);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        //[DllImportAttribute("user32.dll")]
        //public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);


        public static void GetActiveWindow(out string activeWindowAppName, out string activeWindowTitle, out int handle)
        {
            const int nChars = 256;
            var buff = new StringBuilder(nChars);

            //Handle and activeWindowTitle
            handle = GetForegroundWindow();
            activeWindowTitle = "";
            while (GetParent(handle) != 0)
            {
                handle = GetParent(handle);
                if (!string.IsNullOrEmpty(buff.ToString()))
                    activeWindowTitle = string.Format("{0}#{1}", buff, activeWindowTitle);
            }

            if (GetWindowText(handle, buff, nChars) > 0)
            {
                activeWindowTitle = buff + activeWindowTitle;
            }

            //activeWindowAppName
            uint processId = 0;


            GetWindowThreadProcessId(handle, out processId);
            try
            {
                activeWindowAppName = Process.GetProcessById((int)processId).MainModule.FileName;
            }
            catch (Exception e)
            {
                activeWindowAppName = processId.ToString();
            }
        }
    }
}