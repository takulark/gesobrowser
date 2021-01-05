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
    }

}
