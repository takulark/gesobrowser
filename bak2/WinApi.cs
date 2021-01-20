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
        [DllImport("kernel32.dll")]
        public static extern uint GetPrivateProfileString(string lpAppName,string lpKeyName, string lpDefault,StringBuilder lpReturnedString, uint nSize,string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern uint GetPrivateProfileInt(string lpAppName,string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName,string lpstring,string lpFileName);

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
        public static extern bool SetWindowPlacement(IntPtr hWnd,[In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd,out WINDOWPLACEMENT lpwndpl);
        /*
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName,string lpTargetFileName,SymbolicLink dwFlags);
        public enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle,uint DesiredAccess,out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport("advapi32.dll", SetLastError = true,   CharSet = CharSet.Auto)]
        public static extern bool LookupPrivilegeValue(string lpSystemName,string lpName,out long lpLuid);

        [System.Runtime.InteropServices.StructLayout(
           System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        public struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public long Luid;
            public int Attributes;
        }
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,bool DisableAllPrivileges,ref TOKEN_PRIVILEGES NewState,int BufferLength,IntPtr PreviousState,IntPtr ReturnLength);
        */
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
        //public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref SendDATASTRUCT lParam);
        public const int WM_COPYDATA = 0x4A;
        public const int WM_SETICON = 0x80;
        public const int WM_APP = 0x8001;


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
        /*
        private static IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            switch (uMsg)
            {
                case WM_DESTROY:
                    PostQuitMessage(0);
                    break;
                default:
                    return DefWindowProc(hWnd, uMsg, wParam, lParam);
            }
            return IntPtr.Zero;
        }
        public static IntPtr NewWindow(string WindowName, IntPtr Instance, WNDPROC lWndProc)
        {
            WNDCLASSEX wndclassex = new WNDCLASSEX();
            wndclassex.cbSize = (uint)Marshal.SizeOf(wndclassex);
            wndclassex.style = CS.CS_HREDRAW | CS.CS_VREDRAW | CS.CS_GLOBALCLASS;
            wndclassex.lpfnWndProc = lWndProc;
            wndclassex.cbClsExtra = 0;
            wndclassex.cbWndExtra = 0;
            wndclassex.hInstance = Instance; //Process.GetCurrentProcess().Handle;
//           wndclassex.hIcon = ExtractIcon(wndclassex.hInstance, "up0669.ico", 0);
//            wndclassex.hCursor = LoadCursor(IntPtr.Zero, IDC.IDC_ARROW);
            wndclassex.hbrBackground = IntPtr.Zero;
            wndclassex.lpszMenuName = null;
            wndclassex.lpszClassName = WindowName + "ClassName";
            wndclassex.hIconSm = IntPtr.Zero;

            RegisterClassEx(ref wndclassex);

            IntPtr hWnd = CreateWindowEx(0, wndclassex.lpszClassName, WindowName, WS.WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, CW_USEDEFAULT, 800, 600, IntPtr.Zero, IntPtr.Zero, wndclassex.hInstance, IntPtr.Zero);

        //    ShowWindow(hWnd, SW.SHOW);
        //    UpdateWindow(hWnd);

            //           MSG msg = new MSG();
            //            while (GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
            //            {
            //                TranslateMessage(ref msg);
            //                DispatchMessage(ref msg);
            //            }
            return hWnd;
        }
        public delegate IntPtr WNDPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public CS style;
            public WNDPROC lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }
        [Flags]
        public enum IDC : uint
        {
            IDC_APPSTARTING = 0x7f8a,
            IDC_ARROW = 0x7f00,
            IDC_CROSS = 0x7f03,
            IDC_HAND = 0x7f89,
            IDC_HELP = 0x7f8b,
            IDC_IBEAM = 0x7f01,
            IDC_ICON = 0x7f81,
            IDC_NO = 0x7f88,
            IDC_SIZE = 0x7f80,
            IDC_SIZEALL = 0x7f86,
            IDC_SIZENESW = 0x7f83,
            IDC_SIZENS = 0x7f85,
            IDC_SIZENWSE = 0x7f82,
            IDC_SIZEWE = 0x7f84,
            IDC_UPARROW = 0x7f04,
            IDC_WAIT = 0x7f02
        }

        [Flags]
        public enum CS : uint
        {
            CS_VREDRAW = 0x0001,
            CS_HREDRAW = 0x0002,
            CS_DBLCLKS = 0x0008,
            CS_OWNDC = 0x0020,
            CS_CLASSDC = 0x0040,
            CS_PARENTDC = 0x0080,
            CS_NOCLOSE = 0x0200,
            CS_SAVEBITS = 0x0800,
            CS_BYTEALIGNCLIENT = 0x1000,
            CS_BYTEALIGNWINDOW = 0x2000,
            CS_GLOBALCLASS = 0x4000,
            CS_IME = 0x00010000,
            CS_DROPSHADOW = 0x00020000
        }
        [Flags]
        public enum WS : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_CAPTION = 0x00C00000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD
        }
        public const int CW_USEDEFAULT = unchecked((int)0x80000000);
        public const uint WM_DESTROY = 0x0002;
        public const uint WM_CREATE = 0x0001;
        [DllImport("user32.dll")]
        public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern int TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, IDC cursor);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, uint nIconIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        */
    }
}
