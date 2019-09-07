using System;
using System.Diagnostics;

namespace DesktopExtension.SavePosition
{
    internal class PositionWindowsByProcessCollectorOptions {
        public Func<PositionedWindow, bool> ExcludeWindow { get; }
        public Func<Process, bool> ExcludeProcess { get; }

        public PositionWindowsByProcessCollectorOptions(
            Func<PositionedWindow, bool> includeWindow = null, 
            Func<Process, bool> includeProcess = null)
        {
            ExcludeWindow = includeWindow ?? (pw => false);
            ExcludeProcess = includeProcess ?? (p => false);
        }

        public static PositionWindowsByProcessCollectorOptions DefaultOptions { get; } = new PositionWindowsByProcessCollectorOptions();
    }
}