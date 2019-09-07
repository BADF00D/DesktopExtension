using System.Collections.Generic;
using System.Diagnostics;

namespace DesktopExtension.SavePosition
{
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
}