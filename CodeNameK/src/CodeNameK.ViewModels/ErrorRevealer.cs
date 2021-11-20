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
    }
}