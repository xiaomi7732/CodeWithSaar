using System;
using System.Windows;

namespace CodeNameK.ViewModels
{
    public class ErrorRevealer
    {
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
    }
}