using System;
using System.Windows;

namespace CodeNameK.ViewModels
{
    internal class ErrorRevealer : IErrorRevealer
    {
        private string _defaultTitle;
        public ErrorRevealer(string defaultTitle)
        {
            _defaultTitle = string.IsNullOrEmpty(defaultTitle) ? "Unknown error" : defaultTitle;
        }

        public void Reveal(string message, string title)
        {
            Application.Current.Dispatcher.Invoke<int>(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                return int.MinValue;
            });
        }

        public void Reveal(Exception ex, string title)
        {
            string message = ex.Message;
#if DEBUG
            message = ex.ToString();
#endif
            Reveal(message, title);
        }

        public void Reveal(Exception ex) => Reveal(ex, _defaultTitle);
    }
}