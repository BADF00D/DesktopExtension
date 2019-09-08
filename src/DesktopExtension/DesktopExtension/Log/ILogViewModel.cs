using DesktopExtension.Utils;
using Reactive.Bindings;
using System;

namespace DesktopExtension.Log
{
    public interface ILogViewModel
    {
        ReactiveCollection<INotification> Notifications { get;  }
        ReactiveProperty<INotification> SelectedNotification { get; }
    }

    class LogViewModel : ILogViewModel, IDisposable
    {
        public ReactiveCollection<INotification> Notifications { get; }
        public ReactiveProperty<INotification> SelectedNotification { get; }

        public LogViewModel()
        {
            Notifications = new ReactiveCollection<INotification>();
            SelectedNotification = new ReactiveProperty<INotification>();
        }

        public void Dispose()
        {
            Notifications?.Dispose();
            SelectedNotification?.Dispose();
        }
    }

    class LogDesignViewModel : LogViewModel
    {
        public LogDesignViewModel()
        {
            Notifications.AddOnScheduler(new DummyNotification("Some info", NotificationType.Information));
            Notifications.AddOnScheduler(new DummyNotification("Some warning", NotificationType.Warning));
            Notifications.AddOnScheduler(new DummyNotification("Some error", NotificationType.Error));
        }

        private class DummyNotification : ANotification
        {
            public DummyNotification(string message, NotificationType type) : base(message, type)
            {
            }
        }
    }
}