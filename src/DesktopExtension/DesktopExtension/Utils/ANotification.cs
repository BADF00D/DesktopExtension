using System;

namespace DesktopExtension.Utils
{
    internal class ANotification : INotification
    {
        public ANotification(string message, NotificationType type)
        {
            Id = Guid.NewGuid();
            Timestamp = DateTimeOffset.UtcNow;
            Message = message;
            Type = type;
        }

        public Guid Id { get; }
        public DateTimeOffset Timestamp { get; }
        public string Message { get; }
        public NotificationType Type { get; }
    }
}