using System;
using System.Reactive.Linq;
using DesktopExtension.Utils;
using Microsoft.Win32;

namespace DesktopExtension.SavePosition
{
    internal static class BackupEventTypeCreator
    {
        public static IObservable<BackupAndRestorePosition.EventType> CreateFromSystemEvents(INotificationBus bus)
        {
            return CreateFromSessionSwitch(bus)
                .Merge(CreateFromPowerMode(bus));
        }

        private static IObservable<BackupAndRestorePosition.EventType> CreateFromSessionSwitch(INotificationBus bus)
        {
            return Observable.FromEventPattern<SessionSwitchEventHandler, SessionSwitchEventArgs>(
                    ev => SystemEvents.SessionSwitch += ev,
                    ev => SystemEvents.SessionSwitch -= ev)
                .Select(eventPattern => eventPattern.EventArgs.Reason)
                .Where(reason => reason == SessionSwitchReason.SessionLock || reason == SessionSwitchReason.SessionUnlock)
                .Select(reason =>
                {
                    if (reason == SessionSwitchReason.SessionLock)
                    {
                        bus.Emit(new BackupOrRestore("Backup triggered by SessionLock event"));
                        return BackupAndRestorePosition.EventType.Backup;
                    }
                    else
                    {
                        bus.Emit(new BackupOrRestore("Restore triggered by SessionUnlock event"));
                        return BackupAndRestorePosition.EventType.Restore;
                    }
                });
        }

        private static IObservable<BackupAndRestorePosition.EventType> CreateFromPowerMode(INotificationBus bus)
        {
            return Observable.FromEventPattern<PowerModeChangedEventHandler, PowerModeChangedEventArgs>(
                    ev => SystemEvents.PowerModeChanged += ev,
                    ev => SystemEvents.PowerModeChanged -= ev
                )
                .Select(eventPattern => eventPattern.EventArgs.Mode)
                .Where(mode => mode != PowerModes.StatusChange)
                .Select(mode =>
                {
                    if (mode == PowerModes.Suspend)
                    {
                        bus.Emit(new BackupOrRestore("Backup triggered by Suspend event"));
                        return BackupAndRestorePosition.EventType.Backup;
                    }
                    else
                    {
                        bus.Emit(new BackupOrRestore("Restore triggered by Power Event event"));
                        return BackupAndRestorePosition.EventType.Restore;
                    }
                });
        }

        private class BackupOrRestore : ANotification
        {
            public BackupOrRestore(string message) : base(message, NotificationType.Information)
            {
            }
        }
    }
}