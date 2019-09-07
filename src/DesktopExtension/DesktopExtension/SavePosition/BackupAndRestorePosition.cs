using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace DesktopExtension.SavePosition
{
    internal class BackupAndRestorePosition : IDisposable
    {
        private readonly PositionWindowsByProcessCollector _collector;
        private IReadOnlyCollection<PositionWindowsByProcess> _positionWindowsByProcesses;


        public BackupAndRestorePosition(IObservable<EventType> events, PositionWindowsByProcessCollector collector, PostionRestoreOperator restoreOperator, IReadOnlyCollection<string> excludedProcesses)
        {
            _collector = collector;

            var options = new PositionWindowsByProcessCollectorOptions(
                window => string.IsNullOrWhiteSpace(window.Title) || window.Reactangle.IsEmpty || window.Reactangle.IsOffScreen(),
                process => excludedProcesses.Any(n => process.ProcessName.Contains(n))
            );
            _subscription = events
                .Do(e =>
                {
                    if (e == EventType.Backup)
                    {
                        _positionWindowsByProcesses = _collector.Collect(options);
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
}