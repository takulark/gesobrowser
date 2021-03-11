using System;
using System.Text;
using System.Runtime.InteropServices;

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
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

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
    }
}
