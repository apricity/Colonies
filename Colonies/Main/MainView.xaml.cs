namespace Wacton.Colonies.Main
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
            this.ToggleFlyout(0);
        }

        private void ShowDebug(object sender, RoutedEventArgs e)
        {
            this.ToggleFlyout(1);
        }

        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;

            if (flyout == null)
            {
                return;
            }

            if (flyout.IsOpen)
            {
                flyout.IsOpen = false;
            }
            else
            {
                foreach (var otherFlyout in this.Flyouts.Items.Cast<Flyout>().Where(otherFlyout => !otherFlyout.Equals(flyout)))
                {
                    otherFlyout.IsOpen = false;
                }

                flyout.IsOpen = true;
            }
        }
    }
}
