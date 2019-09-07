using System;
using System.Collections.Generic;
using DesktopExtension.Interop;
using DesktopExtension.Utils;

namespace DesktopExtension.SavePosition
{
    internal class PostionRestoreOperator
    {
        private readonly INotificationBus _notificationBus;

        public PostionRestoreOperator(INotificationBus notificationBus)
        {
            _notificationBus = notificationBus;
        }

        public void Restore(IReadOnlyCollection<PositionWindowsByProcess> windows)
        {
            foreach (var positionWindowsByProcess in windows)
            {
                foreach (var positionedWindow in positionWindowsByProcess.RootWindows)
                {
                    var (left, top, right, bottom) = positionedWindow.Rectangle;
                    try
                    {
                        var message = $"Moving '{positionedWindow.Title}' from process '{positionWindowsByProcess.Process.ProcessName}' to ({left},{top},{right},{bottom})";
                        _notificationBus.Emit(new RestoreNotifaction(message, positionedWindow.Title, positionWindowsByProcess.Process.ProcessName, positionedWindow.Rectangle));
                        User32.MoveTo(positionedWindow.WindowHandle, positionedWindow.Rectangle);
                    }
                    catch (Exception e)
                    {
                        var message = $"Moving '{positionedWindow.Title}' from process '{positionWindowsByProcess.Process.ProcessName}' to ({left},{top},{right},{bottom})";
                        _notificationBus.Emit(new RestoreFailedNotification(message, e));
                    }
                }
            }
        }

        private class RestoreNotifaction  : ANotification{
            public string WindowTitle { get; }
            public string ProcessName { get; }
            public WindowRectangle Rectangle { get; }

            public RestoreNotifaction(string message, string windowTitle, string processName, WindowRectangle rectangle) : base(message, NotificationType.Information)
            {
                WindowTitle = windowTitle;
                ProcessName = processName;
                Rectangle = rectangle;
            }
        }
        private class RestoreFailedNotification : ANotification {
            public Exception Exception { get; }
            public RestoreFailedNotification(string message, Exception exception) : base(message, NotificationType.Error)
            {
                Exception = exception;
            }
        }
    }
}