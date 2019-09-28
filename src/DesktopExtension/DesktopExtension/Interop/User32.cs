using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DesktopExtension.Interop
{
    internal class Powrprof
    {
        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        public static void SwitchToHibernate() => SetSuspendState(true, true, true);
        public static void SwitchToStandby() => SetSuspendState(false, true, true);
    }

    internal partial class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool LockWorkStation();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);



        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Callbacks.Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(Callbacks.Win32Callback callPtr, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public override string ToString()
            {
                return $"L{Left},T{Top},RL{Top},BL{Bottom}";
            }
        }


        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd)+1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);


        public struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>Coordinates are in workspace coordinates.</remarks>
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            /// <summary>
            /// The Flags that control the position of the minimized window and the method by which the window is restored. This member can be one or more of the following values.
            /// </summary>
            public WindowPlacementFlags Flags;
            /// <summary>
            /// The current show state of the window. This member can be one of the following values.
            /// </summary>
            public ShowState showCmd;
            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is minimized.
            /// </summary>
            public Point ptMinPosition;
            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is maximized.
            /// </summary>
            public Point ptMaxPosition;
            /// <summary>
            /// The window's coordinates when the window is in the restored position.
            /// </summary>
            public RECT rcNormalPosition;

            public override string ToString()
            {
                return $"Flags: {Flags} Position: {rcNormalPosition} ShowCmd: {showCmd} Flags: {Flags}";
            }
        }

        [Flags]
        public enum WindowPlacementFlags
        {
            None = 0,
            /// <summary>
            /// The coordinates of the minimized window may be specified. This flag must be specified if the coordinates are set in the ptMinPosition member.
            /// </summary>
            WPF_SETMINPOSITION = 0x1,
            /// <summary>
            /// The restored window will be maximized, regardless of whether it was maximized before it was minimized. This setting is only valid the next time the window is restored. It does not change the default restoration behavior. This flag is only valid when the SW_SHOWMINIMIZED value is specified for the showCmd member.
            /// </summary>
            WPF_RESTORETOMAXIMIZED = 0x2,
            /// <summary>
            /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            WPF_ASYNCWINDOWPLACEMENT = 0x4,
        }

        public enum ShowState
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            SW_HIDE = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
            /// </summary>
            SW_SHOWNORMAL = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            SW_SHOWMINIMIZED = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            SW_MAXIMIZE = 3,
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            SW_SHOWMAXIMIZED = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except the window is not activated.
            /// </summary>
            SW_SHOWNOACTIVATE = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            SW_SHOW = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level window in the z-order.
            /// </summary>
            SW_MINIMIZE = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
            /// </summary>
            SW_RESTORE = 9,
            /// <summary>
            /// Displays the window in its current size and position. This value is similar to SW_SHOW, except the window is not activated.
            /// </summary>
            SW_SHOWNA = 8,
        }

        public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);

            return placement;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Point pt);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        [Flags]
        public enum SetWindowPosFlags
        {
            /// <summary>
            /// Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            NOSIZE = 0x0001,
            /// <summary>
            ///  Retains the current position (ignores X and Y parameters).
            /// </summary>
            NOMOVE = 0x0002,
            /// <summary>
            /// Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            NOZORDER = 0x0004,
            /// <summary>
            /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            NOREDRAW = 0x0008,
            /// <summary>
            /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            NOACTIVATE = 0x0010,
            /// <summary>
            /// Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            DRAWFRAME = 0x0020,
            /// <summary>
            /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            FRAMECHANGED = 0x0020,
            /// <summary>
            /// Displays the window.
            /// </summary>
            SHOWWINDOW = 0x0040,
            /// <summary>
            /// Hides the window
            /// </summary>
            HIDEWINDOW = 0x0080,
            /// <summary>
            /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            NOCOPYBITS = 0x0100,
            /// <summary>
            /// Does not change the owner window's position in the Z order.
            /// </summary>
            NOOWNERZORDER = 0x0200,
            /// <summary>
            /// Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            NOREPOSITION = 0x0200,
            /// <summary>
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            NOSENDCHANGING = 0x0400,
            /// <summary>
            /// Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            DEFERERASE = 0x2000,
            /// <summary>
            /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            ASYNCWINDOWPOS = 0x4000
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public static void MoveTo(IntPtr hWnd, WindowRectangle rectangle)
        {
            var width = rectangle.Right - rectangle.Left;
            var height = rectangle.Bottom - rectangle.Top;

            SetWindowPos(hWnd, IntPtr.Zero, rectangle.Left, rectangle.Top, width, height,
                (int)(SetWindowPosFlags.NOZORDER));
        }

        public static WindowRectangle GetWindowRectFrom(IntPtr hwnd)
        {
            var rect = new RECT();
            if (GetWindowRect(hwnd, ref rect))
            {
                return new WindowRectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
            throw new Exception("unable to retrieve rect");
        }
    }
}