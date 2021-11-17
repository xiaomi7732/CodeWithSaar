using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using CodeNameK.ViewModels;

namespace CodeNameK.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            DataContext = mainViewModel;
            InitializeComponent();
            DateTimeAxisLabel label;
        }
    }
}
