using System;
using System.Reactive.Linq;
using Microsoft.Win32;

namespace DesktopExtension.SavePosition
{
    internal static class BackupEventTypeCreator
    {
        public static IObservable<BackupAndRestorePosition.EventType> CreateFromSystemEvents()
        {
            return CreateFromSessionSwitch()
                .Merge(CreateFromPowerMode());
        }

        private static IObservable<BackupAndRestorePosition.EventType> CreateFromSessionSwitch()
        {
            return Observable.FromEventPattern<SessionSwitchEventHandler, SessionSwitchEventArgs>(
                    ev => SystemEvents.SessionSwitch += ev,
                    ev => SystemEvents.SessionSwitch -= ev)
                .Select(eventPattern => eventPattern.EventArgs.Reason)
                .Where(reason => reason == SessionSwitchReason.SessionLock || reason == SessionSwitchReason.SessionUnlock)
                .Select(reason => reason == SessionSwitchReason.SessionLock
                    ? BackupAndRestorePosition.EventType.Backup
                    : BackupAndRestorePosition.EventType.Restore);
        }

        private static IObservable<BackupAndRestorePosition.EventType> CreateFromPowerMode()
        {
            return Observable.FromEventPattern<PowerModeChangedEventHandler, PowerModeChangedEventArgs>(
                    ev => SystemEvents.PowerModeChanged += ev,
                    ev => SystemEvents.PowerModeChanged -= ev
                )
                .Select(eventPattern => eventPattern.EventArgs.Mode)
                .Where(mode => mode != PowerModes.StatusChange)
                .Select(mode => mode == PowerModes.Suspend
                    ? BackupAndRestorePosition.EventType.Backup
                    : BackupAndRestorePosition.EventType.Restore);
        }
    }
}