using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CodeNameK.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected readonly IErrorRevealerFactory _errorRevealerFactory;

        public ViewModelBase(IErrorRevealerFactory errorRevealerFactory)
        {
            _errorRevealerFactory = errorRevealerFactory ?? throw new ArgumentNullException(nameof(errorRevealerFactory));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected T Dispatch<T>(Func<T> callBack)
        {
            return Application.Current.Dispatcher.Invoke<T>(callBack);
        }
    }
}