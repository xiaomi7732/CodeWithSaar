using AndroidX.Lifecycle;
using System;

namespace CodeNameK.Droid.ViewModels
{
    internal static class ViewModelExtensions
    {
        public static T Get<T>(this ViewModelProvider viewModelProvider)
            where T : ViewModel
        {
            if (viewModelProvider is null)
            {
                throw new ArgumentNullException(nameof(viewModelProvider));
            }

            T viewModel = viewModelProvider.Get(Java.Lang.Class.FromType(typeof(T))) as T;
            return viewModel;
        }

        public static T Get<T>(this ViewModelProvider viewModelProvider, string name)
            where T : ViewModel
        {
            if (viewModelProvider is null)
            {
                throw new ArgumentNullException(nameof(viewModelProvider));
            }

            if (string.IsNullOrEmpty(name))
            {
                return Get<T>(viewModelProvider);
            }

            return viewModelProvider.Get(name, Java.Lang.Class.FromType(typeof(T))) as T;
        }
    }
}