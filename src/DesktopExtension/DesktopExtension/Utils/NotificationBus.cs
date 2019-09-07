using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DesktopExtension.Utils
{
    public class NotificationBus : IDisposable, INotificationBus
    {
        private readonly Subject<INotification> _subject = new Subject<INotification>();
        public IObservable<INotification> Notifications { get; }

        public NotificationBus()
        {
            Notifications = _subject.AsObservable();
        }

        public void Emit(INotification notification)
        {
            _subject.OnNext(notification);
        }

        public void Dispose()
        {
            _subject?.Dispose();
        }
    }
}