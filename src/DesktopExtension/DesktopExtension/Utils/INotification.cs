using System;

namespace DesktopExtension.Utils
{
    public interface INotification
    {
        Guid Id { get; }
        DateTimeOffset Timestamp { get; }
        string Message { get; }
        NotificationType Type { get; }
    }
}