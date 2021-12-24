using System;
using System.Windows;

namespace CodeNameK.ViewModels
{
    public class ErrorRevealer
    {
        private string? _defaultTitle;

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
            message += Environment.NewLine + ex.StackTrace;
#endif
            Reveal(message, title);
        }

        public void Reveal(Exception ex)
            => Reveal(ex, _defaultTitle ?? "Unexpected error");

        public static ErrorRevealer WithTitle(string title)
        {
            ErrorRevealer newInstance = new ErrorRevealer() {
                _defaultTitle = title,
            };
            return newInstance;
        }
    }
}