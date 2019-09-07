using System;
using DesktopExtension.Interop;

namespace DesktopExtension.SavePosition
{
    internal class PositionedWindow
    {
        public WindowRectangle Rectangle { get; }
        public IntPtr WindowHandle { get; }
        public string Title { get; }

        public PositionedWindow(WindowRectangle rectangle, IntPtr windowHandle, string title)
        {
            Rectangle = rectangle;
            WindowHandle = windowHandle;
            Title = title;
        }
    }
}