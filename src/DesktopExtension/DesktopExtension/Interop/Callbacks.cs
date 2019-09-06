using System;

namespace DesktopExtension.Interop
{
    internal class Callbacks
    {
        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);
    }
}