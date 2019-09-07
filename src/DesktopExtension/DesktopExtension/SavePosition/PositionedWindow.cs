using System;
using DesktopExtension.Interop;

namespace DesktopExtension.SavePosition
{
    internal class PositionedWindow
    {
        public User32.WindowRectangle Reactangle { get; }
        public IntPtr WindowHandle { get; }
        public string Title { get; }

        public PositionedWindow(User32.WindowRectangle reactangle, IntPtr windowHandle, string title)
        {
            Reactangle = reactangle;
            WindowHandle = windowHandle;
            Title = title;
        }
    }
}