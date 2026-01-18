using System.Windows;
using System.Windows.Forms; // For FolderBrowserDialog

namespace Structura.UI
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        private string _lastExportedFilePath;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            
            // Initial Prompt Display
            PromptPreviewBox.Text = PromptTemplates.GetPrompt();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(_viewModel.SelectedPath))
                {
                    dialog.SelectedPath = _viewModel.SelectedPath;
                }

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _viewModel.SelectedPath = dialog.SelectedPath;
                }
            }
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            var path = _viewModel.SelectedPath;
            if (string.IsNullOrWhiteSpace(path) || !System.IO.Directory.Exists(path))
            {
                 System.Windows.MessageBox.Show("Please select a valid directory.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                 return;
            }

            StatusText.Text = "Scanning...";
            await _viewModel.ScanAsync(path);
            
            _viewModel.AddToHistory(path);

            if (_viewModel.HasErrors)
            {
                 StatusText.Text = $"Scan Complete with {_viewModel.ScanErrors.Count} warnings.";
            }
            else
            {
                 StatusText.Text = "Scan Complete.";
            }
        }

        private void ViewErrors_Click(object sender, RoutedEventArgs e)
        {
            var errorWindow = new ErrorLogWindow(_viewModel.ScanErrors);
            errorWindow.Owner = this;
            errorWindow.ShowDialog();
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_lastExportedFilePath) && System.IO.File.Exists(_lastExportedFilePath))
            {
                string argument = $"/select, \"{_lastExportedFilePath}\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            else
            {
                 System.Windows.MessageBox.Show("File not found or moved.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                 OpenFolderBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void CopyPromptButton_Click(object sender, RoutedEventArgs e)
        {
            var prompt = PromptPreviewBox.Text;
            System.Windows.Clipboard.SetText(prompt);
            System.Windows.MessageBox.Show("Prompt copied to clipboard!\n\nYou can now paste it into ChatGPT/Claude along with your exported JSON file.", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetExportFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder to save exported JSON files";
                if (System.IO.Directory.Exists(_viewModel.ExportDirectory))
                {
                    dialog.SelectedPath = _viewModel.ExportDirectory;
                }

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _viewModel.ExportDirectory = dialog.SelectedPath;
                }
            }
        }

        private void ViewDistribution_Click(object sender, RoutedEventArgs e)
        {
            var depthStats = _viewModel.AnalyzeDepthDistribution();
            if (depthStats.Count == 0)
            {
                System.Windows.MessageBox.Show("Please scan a directory first.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            var breadthStats = _viewModel.AnalyzeBreadthDistribution();

            var window = new DepthDistributionWindow(depthStats, breadthStats);
            window.Owner = this;
            window.ShowDialog();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var path = _viewModel.SelectedPath;
            if (string.IsNullOrWhiteSpace(path) || !System.IO.Directory.Exists(path))
            {
                 System.Windows.MessageBox.Show("Please select a valid directory to scan first.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                 return;
            }

            // Ensure Export Directory exists
            if (string.IsNullOrWhiteSpace(_viewModel.ExportDirectory) || !System.IO.Directory.Exists(_viewModel.ExportDirectory))
            {
                // Fallback to Downloads if invalid
                 _viewModel.ExportDirectory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
            }

            string targetFile = System.IO.Path.Combine(_viewModel.ExportDirectory, "Structura_Export.json");

            try
            {
                StatusText.Text = "Scanning & Exporting...";
                
                // Force a fresh scan for export to ensure accuracy
                await _viewModel.ScanAsync(path);
                _viewModel.AddToHistory(path);

                long tokenCount = _viewModel.ExportJson(targetFile);
                _lastExportedFilePath = targetFile;
                
                // Copy file to clipboard
                var fileDropList = new System.Collections.Specialized.StringCollection();
                fileDropList.Add(targetFile);
                System.Windows.Clipboard.SetFileDropList(fileDropList);

                StatusText.Text = "Exported & Copied to Clipboard.";
                OpenFolderBtn.Visibility = Visibility.Visible;
                
                string msg = $"Successfully exported to:\n{targetFile}\n\nEstimated Tokens: {tokenCount:N0}\n(Calculation based on 1 token ≈ 4 chars)\n\nThe file has been copied to your clipboard. You can paste it directly into LLM chat.";
                if (_viewModel.HasErrors)
                {
                    msg += $"\n\nNote: The scan encountered {_viewModel.ScanErrors.Count} warnings. Click 'View Errors' for details.";
                }
                
                System.Windows.MessageBox.Show(msg, "Export & Copy Success", MessageBoxButton.OK, _viewModel.HasErrors ? MessageBoxImage.Warning : MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                 StatusText.Text = "Export Failed.";
                 System.Windows.MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
