using CodeNameK.Core.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeNameK.ViewModels
{
    internal class AsyncRelayCommand : ICommand
    {
        Func<object?, Task> _executeAsync;
        Func<object?, bool>? _canExecute;
        Action<Exception>? _exceptionCallback;

        public AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null, Action<Exception>? exceptionCallback = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute; ;
            _exceptionCallback = exceptionCallback;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _executeAsync.Invoke(parameter).FireWithExceptionHandler(_exceptionCallback);
        }
    }
}
