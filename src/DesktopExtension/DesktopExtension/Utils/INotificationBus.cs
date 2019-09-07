using System;

namespace DesktopExtension.Utils
{
    public interface INotificationBus
    {
        IObservable<INotification> Notifications { get; }
        void Emit(INotification notification);
    }
}