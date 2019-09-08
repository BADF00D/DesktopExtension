using System;
using DesktopExtension.Log;
using DesktopExtension.Testing;
using DesktopExtension.Utils;

namespace DesktopExtension
{
    public interface IMainViewModel
    {
        ITestingViewModel Testing { get; }
        ILogViewModel Log { get; }
    }

    class MainViewModel : IMainViewModel, IDisposable
    {

        public MainViewModel(INotificationBus bus)
        {
            Testing = new TestingViewModel();
            Log = new LogViewModel(bus);
        }

        public void Dispose()
        {
            (Testing as IDisposable)?.Dispose();
            (Log as IDisposable)?.Dispose();
        }

        public ITestingViewModel Testing { get; }
        public ILogViewModel Log { get; }
    }
}