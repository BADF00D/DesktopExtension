using DesktopExtension.Utils;
using Reactive.Bindings;
using System;
using System.Linq;
using System.Reactive.Linq;
using DesktopExtension.SavePosition;

namespace DesktopExtension.Log
{
    public interface ILogViewModel
    {
        ReactiveCollection<INotification> Notifications { get;  }
        ReactiveProperty<INotification> SelectedNotification { get; }
        ReactiveProperty<object> Details { get; }
    }

    class LogViewModel : ILogViewModel, IDisposable
    {
        public ReactiveCollection<INotification> Notifications { get; }
        public ReactiveProperty<INotification> SelectedNotification { get; }
        public ReactiveProperty<object> Details { get; }

        public LogViewModel(INotificationBus bus)
        {
            Notifications = bus.Notifications.ToReactiveCollection();
            SelectedNotification = new ReactiveProperty<INotification>();
            Details = SelectedNotification?.Select(sn =>
                {
                    return sn is BackupNotification bn
                        ? new BackupLogDetailViewModel(bn.PositionWindowsByProcesses)
                        : default(object);
                }).ToReactiveProperty();
        }

        public void Dispose()
        {
            Notifications?.Dispose();
            SelectedNotification?.Dispose();
        }
    }

    class LogDesignViewModel : LogViewModel
    {
        public LogDesignViewModel(): base(new NotificationBus())
        {
            Notifications.AddOnScheduler(new DummyNotification("Some info", NotificationType.Information));
            Notifications.AddOnScheduler(new DummyNotification("Some warning", NotificationType.Warning));
            Notifications.AddOnScheduler(new DummyNotification("Some error", NotificationType.Error));
            var backupNotification = new BackupNotification(new PositionWindowsByProcess[0]);
            Notifications.AddOnScheduler(backupNotification);
            SelectedNotification.Value = backupNotification;
        }

        private class DummyNotification : ANotification
        {
            public DummyNotification(string message, NotificationType type) : base(message, type)
            {
            }
        }
    }
}