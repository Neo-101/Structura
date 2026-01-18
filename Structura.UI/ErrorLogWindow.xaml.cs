using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Structura.UI
{
    public partial class ErrorLogWindow : Window
    {
        public IEnumerable<ScanError> Errors { get; private set; }

        public ErrorLogWindow(IEnumerable<ScanError> errors)
        {
            InitializeComponent();
            Errors = errors;
            DataContext = this;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Type\tPath");
            foreach (var err in Errors)
            {
                sb.AppendLine($"{err.Message}\t{err.Path}");
            }
            Clipboard.SetText(sb.ToString());
            MessageBox.Show("Errors copied to clipboard.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
