#if WINDOWS

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Kontecg.Runtime
{
    internal class Win32Api
    {
        [SecuritySafeCritical]
        public static bool SetForegroundWindow(IntPtr hwnd)
        {
            return Import.SetForegroundWindow(hwnd);
        }

        [SecuritySafeCritical]
        public static bool RestoreWindowAsync(IntPtr hwnd)
        {
            return Import.ShowWindowAsync(hwnd,
                IsMaxmimized(hwnd) ? (int)Import.ShowWindowCommands.ShowMaximized : (int)Import.ShowWindowCommands.Restore);
        }

        [SecuritySafeCritical]
        public static bool IsMaxmimized(IntPtr hwnd)
        {
            Import.WINDOWPLACEMENT placement = new();
            placement.length = Marshal.SizeOf(placement);
            if (!Import.GetWindowPlacement(hwnd, ref placement)) return false;
            return placement.showCmd == Import.ShowWindowCommands.ShowMaximized;
        }

        private static class Import
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWPLACEMENT
            {
                public int length;
                public int flags;
                public ShowWindowCommands showCmd;
                public System.Drawing.Point ptMinPosition;
                public System.Drawing.Point ptMaxPosition;
                public System.Drawing.Rectangle rcNormalPosition;
            }

            public enum ShowWindowCommands : int
            {
                Hide = 0,
                Normal = 1,
                ShowMinimized = 2,
                ShowMaximized = 3,
                ShowNoActivate = 4,
                Show = 5,
                Minimize = 6,
                ShowMinNoActive = 7,
                ShowNA = 8,
                Restore = 9,
                ShowDefault = 10,
                ForceMinimize = 11
            }
        }
    }
}
#endif
