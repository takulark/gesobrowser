using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web.UI.WebControls;
using static gesobrowser.WinApi;

namespace gesobrowser
{
    class WinApi
    {
        [DllImport("kernel32.dll")]
        public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpstring, string lpFileName);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public SW showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
            public RECT rcDevice;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        public enum SW
        {
            HIDE = 0,
            SHOWNORMAL = 1,
            SHOWMINIMIZED = 2,
            SHOWMAXIMIZED = 3,
            SHOWNOACTIVATE = 4,
            SHOW = 5,
            MINIMIZE = 6,
            SHOWMINNOACTIVE = 7,
            SHOWNA = 8,
            RESTORE = 9,
            SHOWDEFAULT = 10,
        }
        //[DllImport("user32.dll")]
        //public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public Int32 cbData;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);
        //public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        public const int WM_COPYDATA = 0x4A;
        public const int WM_SETICON = 0x80;
        public const int WM_APP = 0x8001;
        public const int WM_APP1= WM_APP+1;

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, SW nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UpdateWindow(IntPtr hWnd);

        public const int MONITOR_DEFAULTTONEAREST = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        [DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
                                       int x, int y, int cx, int cy, int flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, ref MONITORINFO info);
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr MonitorFromRect(ref RECT rect, int flags);

        /// <summary>
            /// The MONITORINFOEX structure contains information about a display monitor.
            /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
            /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
            /// for the display monitor.
            /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        //public void SetWindowPlacement(IntPtr hWnd, [In] ref RECT rcWnd)
        //{
        //    // 対象モニタの情報を取得
        //    IntPtr hMonitor = MonitorFromRect(
        //        ref rcWnd, MONITOR_DEFAULTTONEAREST);
        //    MONITORINFO mi = new MONITORINFO();
        //    mi.cbSize = Marshal.SizeOf(mi);
        //    GetMonitorInfo(hMonitor, ref mi);

        //    // 位置補正
        //    if (rcWnd.right > mi.rcMonitor.right)
        //    {
        //        rcWnd.left -= rcWnd.right - mi.rcMonitor.right;
        //        rcWnd.right = mi.rcMonitor.right;
        //    }
        //    if (rcWnd.left < mi.rcMonitor.left)
        //    {
        //        rcWnd.right += mi.rcMonitor.left - rcWnd.left;
        //        rcWnd.left = mi.rcMonitor.left;
        //    }
        //    if (rcWnd.bottom > mi.rcMonitor.bottom)
        //    {
        //        rcWnd.top -= rcWnd.bottom - mi.rcMonitor.bottom;
        //        rcWnd.bottom = mi.rcMonitor.bottom;
        //    }
        //    if (rcWnd.top < mi.rcMonitor.top)
        //    {
        //        rcWnd.bottom += mi.rcMonitor.top - rcWnd.top;
        //        rcWnd.top = mi.rcMonitor.top;
        //    }

        //    // ウィンドウ位置復元
        //    SetWindowPos(
        //        hWnd, IntPtr.Zero, rcWnd.left, rcWnd.top,
        //        rcWnd.right - rcWnd.left, rcWnd.bottom - rcWnd.top,
        //        SWP_NOZORDER);

        //}
    }
}
