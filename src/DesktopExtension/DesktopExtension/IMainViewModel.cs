using System;
using DesktopExtension.Testing;

namespace DesktopExtension
{
    public interface IMainViewModel
    {
        ITestingViewModel Testing { get; }
    }

    class MainViewModel : IMainViewModel, IDisposable
    {

        public MainViewModel()
        {
            Testing = new TestingViewModel();
        }

        public void Dispose()
        {
            (Testing as IDisposable)?.Dispose();
        }

        public ITestingViewModel Testing { get; }
    }
}