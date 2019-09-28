using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using DesktopExtension.SavePosition;
using DesktopExtension.Utils;

namespace DesktopExtension
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackupAndRestorePosition _backupAndRestorePosition;

        public MainWindow()
        {
            InitializeComponent();
            var bus = new NotificationBus();
            DataContext = new MainViewModel(bus);

            var systemEvents = BackupEventTypeCreator.CreateFromSystemEvents(bus);

            systemEvents = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMinutes(1))
                .Select(_ => BackupAndRestorePosition.EventType.Backup)
                .Merge(systemEvents);

            var collector = new PositionWindowsByProcessCollector();

            var excludedProcesses = LoadExcludedProcesses();


            _backupAndRestorePosition = new BackupAndRestorePosition(systemEvents, collector, new PostionRestoreOperator(bus),
                excludedProcesses, bus);
        }

        private static IReadOnlyCollection<string> LoadExcludedProcesses()
        {
            const string fileName = "excludedProcesses.txt";
            var path = Path.Combine(Environment.CurrentDirectory, "settings", fileName);
            return File.Exists(path)
                ? File.ReadAllLines(path)
                : Enumerable.Empty<string>().ToArray();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _backupAndRestorePosition.Dispose();
            _backupAndRestorePosition = null;
            (DataContext as IDisposable)?.Dispose();
            DataContext = null;
        }
    }
}