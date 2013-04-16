namespace Colonies.Views
{
    using System.Windows;

    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            this.InitializeComponent();
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            this.Flyouts[0].IsOpen = !this.Flyouts[0].IsOpen;
        }
    }
}
