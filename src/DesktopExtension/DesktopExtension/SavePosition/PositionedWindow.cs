using System;
using DesktopExtension.Interop;

namespace DesktopExtension.SavePosition
{
    internal class PositionedWindow
    {
        public WindowRectangle Rectangle { get; }
        public IntPtr WindowHandle { get; }
        public string Title { get; }
        public User32.WINDOWPLACEMENT Placement { get; }
        public bool IsVisible { get; }

        public PositionedWindow(WindowRectangle rectangle, IntPtr windowHandle, string title,
            User32.WINDOWPLACEMENT placement, bool isVisible)
        {
            Rectangle = rectangle;
            WindowHandle = windowHandle;
            Title = title;
            Placement = placement;
            IsVisible = isVisible;
        }
    }
}