using System;
using System.Collections.Generic;
using DesktopExtension.Interop;

namespace DesktopExtension.SavePosition
{
    internal class PostionRestoreOperator
    {
        public void Restore(IReadOnlyCollection<PositionWindowsByProcess> windows)
        {
            foreach (var positionWindowsByProcess in windows)
            {
                foreach (var positionedWindow in positionWindowsByProcess.RootWindows)
                {
                    try
                    {
                        var (left, top, right, bottom) = positionedWindow.Reactangle;
                        Console.WriteLine($"Moving '{positionedWindow.Title}' from process '{positionWindowsByProcess.Process.ProcessName}' to ({left},{top},{right},{bottom})");
                        User32.MoveTo(positionedWindow.WindowHandle, positionedWindow.Reactangle);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Unable to restore position of {positionedWindow.Title}: {e.Message}");
                    }
                }
            }
        }
    }
}