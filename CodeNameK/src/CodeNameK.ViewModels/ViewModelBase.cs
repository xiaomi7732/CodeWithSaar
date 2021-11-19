using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CodeNameK.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
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