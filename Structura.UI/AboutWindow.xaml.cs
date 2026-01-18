using System.Windows;

namespace Structura.UI
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            VersionText.Text = BuildInfo.Version;
            CommitText.Text = BuildInfo.CommitHash;
            DateText.Text = BuildInfo.BuildTime;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
