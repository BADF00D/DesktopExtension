using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using DesktopExtension.Interop;
using DesktopExtension.Utils;

namespace DesktopExtension.SavePosition
{
    internal class BackupAndRestorePosition : IDisposable
    {
        private readonly PositionWindowsByProcessCollector _collector;
        private IReadOnlyCollection<PositionWindowsByProcess> _positionWindowsByProcesses;


        public BackupAndRestorePosition(IObservable<EventType> events, PositionWindowsByProcessCollector collector, PostionRestoreOperator restoreOperator, IReadOnlyCollection<string> excludedProcesses, INotificationBus bus)
        {
            _collector = collector;

            var options = new PositionWindowsByProcessCollectorOptions(
                window => string.IsNullOrWhiteSpace(window.Title)
                          //|| window.Rectangle.IsEmpty
                          //|| window.Rectangle.IsOffScreen() since there is a monitor left to main window, this does not work
                          || window.Placement.showCmd == User32.ShowState.SW_HIDE
                          || !window.IsVisible
                ,
                process => excludedProcesses.Any(n => process.ProcessName.Contains(n))
            );
            _subscription = events
                .Do(e =>
                {
                    if (e == EventType.Backup)
                    {
                        _positionWindowsByProcesses = _collector.Collect(options);
                        bus.Emit(new BackupNotification(_positionWindowsByProcesses));
                        Console.WriteLine($"{DateTime.Now} - BACKUP");
                    }
                    else
                    {
                        restoreOperator.Restore(_positionWindowsByProcesses);
                        Console.WriteLine($"{DateTime.Now} - RESTORE");
                    }
                })
                .Subscribe();
        }

        public enum EventType
        {
            Backup,
            Restore
        }

        private readonly IDisposable _subscription;

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }

    internal class BackupNotification : ANotification
    {
        public readonly IReadOnlyCollection<PositionWindowsByProcess> PositionWindowsByProcesses;

        public BackupNotification(IReadOnlyCollection<PositionWindowsByProcess> positionWindowsByProcesses)
            : base("Backup performed", NotificationType.Information)
        {
            PositionWindowsByProcesses = positionWindowsByProcesses;
        }
    }
}