using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using DesktopExtension.Interop;
using DesktopExtension.SavePosition;
using Reactive.Bindings;

namespace DesktopExtension.Log
{
    public interface ILogDetailViewModel
    {

    }

    class BackupLogDetailViewModel : ILogDetailViewModel
    {
        public ProcessDetailViewModel[] Processes { get; }
        public ReactiveProperty<ProcessDetailViewModel> SelectedProcess { get; }

        public BackupLogDetailViewModel(IEnumerable<PositionWindowsByProcess> processes)
        {
            var processDetailViewModels = processes
                .Select(p => new ProcessDetailViewModel(p))
                .OrderBy(p => p.Name)
                .ToArray();
            Processes = processDetailViewModels;
            SelectedProcess = new ReactiveProperty<ProcessDetailViewModel>(processDetailViewModels.FirstOrDefault());
        }
    }

    class ProcessDetailViewModel
    {
        public string Name { get; }
        public string MainWindowTitle { get; }
        public PositionedWindowViewModel[] RootWindows { get; }

        public ProcessDetailViewModel(PositionWindowsByProcess positions)
        {
            var process = positions.Process;
            Name = process.ProcessName;
            MainWindowTitle = process.MainWindowTitle;
            RootWindows = positions.RootWindows
                .Select(rw => new PositionedWindowViewModel(rw))
                .OrderBy(rw => rw.Title)
                .ToArray();
        }
    }

    class PositionedWindowViewModel
    {
        public string Title { get; }
        public string WindowHandle { get; }
        public string Position { get; }
        public string ShowState { get; }
        public string Flags { get; }
        public ReactiveCommand MoveUnderMouseCommand {get; }

        public PositionedWindowViewModel(PositionedWindow rw)
        {
            Title = rw.Title;
            WindowHandle = rw.WindowHandle.ToString();
            Position = rw.Rectangle.ToString();
            ShowState = rw.Placement.showCmd.ToString();
            Flags = rw.Placement.Flags.ToString();
            MoveUnderMouseCommand = new ReactiveCommand();

            MoveUnderMouseCommand
                .Do(_ =>
                {
                    var p = new User32.Point();
                    User32.GetCursorPos(ref p);
                    User32.SetWindowPos(rw.WindowHandle,
                        IntPtr.Zero,
                        p.X,
                        p.X,
                        0,
                        0,
                        (int) (User32.SetWindowPosFlags.SHOWWINDOW | User32.SetWindowPosFlags.NOSIZE));
                    //User32.MoveTo(rw.WindowHandle, new WindowRectangle(p.X, p.Y, 200, 200));
                }).Subscribe();
        }
    }
}