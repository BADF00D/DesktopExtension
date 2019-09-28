using Reactive.Bindings;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using DesktopExtension.Interop;
using Reactive.Bindings.Extensions;

namespace DesktopExtension.Testing
{
    public interface ITestingViewModel
    {
        ReactiveCommand SwitchUserCommand { get; }
        ReactiveCommand LockCommand { get; }
        ReactiveCommand SleepCommand { get; }
        ReactiveCommand HibernateCommand { get; }
    }

    internal class TestingViewModel : ITestingViewModel, IDisposable
    {
        public ReactiveCommand SwitchUserCommand { get; }
        public ReactiveCommand LockCommand { get; }
        public ReactiveCommand SleepCommand { get; }
        public ReactiveCommand HibernateCommand { get; }
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        public TestingViewModel()
        {
            SwitchUserCommand = new ReactiveCommand(Observable.Return(false));
            LockCommand = new ReactiveCommand();
            SleepCommand = new ReactiveCommand();
            HibernateCommand= new ReactiveCommand();

            SleepCommand.Do(_ => { Powrprof.SwitchToStandby(); }).Subscribe().AddTo(_subscriptions);
            HibernateCommand.Do(_ => { Powrprof.SwitchToHibernate(); }).Subscribe().AddTo(_subscriptions);
            LockCommand.Do(_ => { User32.LockWorkStation(); }).Subscribe().AddTo(_subscriptions);
        }

        public void Dispose()
        {
            SwitchUserCommand?.Dispose();
            LockCommand?.Dispose();
            SleepCommand?.Dispose();
            HibernateCommand?.Dispose();
            _subscriptions?.Dispose();
        }
    }
}