using System;
using System.Threading.Tasks;
using System.Windows;

namespace WPFExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AsyncJob2().RunTaskWithCallbacks(
                ex =>
                {
                    MessageBox.Show($"There is an error in Async Job: {ex.Message}");
                },
                () =>
                {
                    MessageBox.Show("AsyncJob is called.");
                });
        }

        private async Task AsyncJob2()
        {
            await Task.Delay(100);
            // throw new InvalidOperationException("Hello, exception again!");
        }
    }
}
