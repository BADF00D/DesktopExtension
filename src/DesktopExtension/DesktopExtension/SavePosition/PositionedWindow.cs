using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using DesktopExtension.Interop;

namespace DesktopExtension.SavePosition
{
    internal class PositionedWindow
    {
        public User32.WindowRectangle Position { get; }
        public IntPtr WindowHandle { get; }
        public string Title { get; }

        public PositionedWindow(User32.WindowRectangle position, IntPtr windowHandle, string title)
        {
            Position = position;
            WindowHandle = windowHandle;
            Title = title;
        }
    }

    internal class PositionWindowsByProcess
    {
        public Process Process { get; }

        public IReadOnlyCollection<PositionedWindow> RootWindows { get; }

        public PositionWindowsByProcess(Process process, IReadOnlyCollection<PositionedWindow> rootWindows)
        {
            Process = process;
            RootWindows = rootWindows;
        }
    }

    internal class PositionWindowsByProcessCollectorOptions {
        public Func<PositionedWindow, bool> FilterWindow { get; }
        public Func<Process, bool> FilterProcess { get; }

        public PositionWindowsByProcessCollectorOptions(
            Func<PositionedWindow, bool> filterWindow = null, 
            Func<Process, bool> filterProcess = null)
        {
            FilterWindow = filterWindow ?? (pw => true);
            FilterProcess = filterProcess ?? (p => true);
        }


    }

    internal class PositionWindowsByProcessCollector
    {
        public IReadOnlyCollection<PositionWindowsByProcess> Collect(PositionWindowsByProcessCollectorOptions options)
        {
            return Process.GetProcesses()
                .Where(options.FilterProcess)
                .Select(p =>
                {
                    var handles = GetRootWindowsOfProcess(p.Id);
                    var windows = handles
                        .Select(hWnd =>
                        {
                            var rect = User32.GetWindowRectFrom(hWnd);
                            var title = User32.GetWindowTitle(hWnd);

                            return new PositionedWindow(rect, hWnd, title);
                        })
                        .Where(options.FilterWindow)
                        .ToArray();
                    return new PositionWindowsByProcess(p, windows);
                })
                .ToArray();
        }

        private List<IntPtr> GetRootWindowsOfProcess(int pid)
        {
            var rootWindows = GetChildWindows(IntPtr.Zero);
            var dsProcRootWindows = new List<IntPtr>();
            foreach (var hWnd in rootWindows)
            {
                User32.GetWindowThreadProcessId(hWnd, out var lpdwProcessId);
                if (lpdwProcessId == pid)
                    dsProcRootWindows.Add(hWnd);
            }

            return dsProcRootWindows;
        }

        private static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            var result = new List<IntPtr>();
            var listHandle = GCHandle.Alloc(result);
            try
            {
                Callbacks.Win32Callback childProc = EnumWindow;
                User32.EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }

            return result;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            var gch = GCHandle.FromIntPtr(pointer);
            var list = gch.Target as List<IntPtr>;
            if (list == null) throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }
    }
}