namespace Wacton.Colonies.Views
{
    using System.Linq;
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
            this.ToggleFlyout(this.Flyouts[0]);
        }

        private void ShowDebug(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(this.Flyouts[1]);
        }

        private void ToggleFlyout(Flyout flyout)
        {
            if (flyout.IsOpen)
            {
                flyout.IsOpen = false;
            }
            else
            {
                foreach (var otherFlyout in this.Flyouts.Except(new[] { flyout }))
                {
                    otherFlyout.IsOpen = false;
                }

                flyout.IsOpen = true;
            }
        }
    }
}
