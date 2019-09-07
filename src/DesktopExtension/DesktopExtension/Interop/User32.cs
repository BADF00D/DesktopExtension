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

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd);
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        [Flags]
        private enum SetWindowPosFlags
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