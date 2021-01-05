using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace gesobrowser
{
    class WinApi
    {
        // ==========================================================
        [DllImport("KERNEL32.DLL")]
        public static extern uint
            GetPrivateProfileString(string lpAppName,
            string lpKeyName, string lpDefault,
            StringBuilder lpReturnedString, uint nSize,
            string lpFileName);

        [DllImport("KERNEL32.DLL")]
        public static extern uint
            GetPrivateProfileInt(string lpAppName,
            string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern int WritePrivateProfileString(
            string lpApplicationName,
            string lpKeyName,
            string lpstring,
            string lpFileName);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [DllImport("User32.Dll")]
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT rc);
        [DllImport("User32.Dll")]
        public static extern int GetClientRect(IntPtr hwnd, ref RECT rc);
        [DllImport("user32.dll")]
        public static extern int MoveWindow(IntPtr hwnd, int x, int y,
            int nWidth, int nHeight, int bRepaint);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
    }

}
