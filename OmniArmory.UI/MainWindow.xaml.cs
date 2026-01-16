using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Added for MouseEventArgs
using OmniArmory.Core;
using OmniArmory.Shared;

namespace OmniArmory.UI
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cts;
        private readonly TreeScanner _scanner;
        private DirectoryNode _currentRootNode; // Store result for export

        // Simple property to toggle DropZone visibility (could be a full ViewModel, but keeping it simple for now)
        public bool HasData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _scanner = new TreeScanner();
            DataContext = this;
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    string path = files[0];
                    if (System.IO.Directory.Exists(path))
                    {
                        await StartScanAsync(path);
                    }
                    else
                    {
                        MessageBox.Show("Please drop a folder, not a file.", "Invalid Item", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _ = StartScanAsync(dialog.SelectedPath);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRootNode == null) return;

            using (var dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                dialog.FileName = $"OmniArmory_Scan_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        string json = JsonSerializer.Serialize(_currentRootNode, options);
                        File.WriteAllText(dialog.FileName, json);
                        MessageBox.Show($"Export successful!\nSaved to: {dialog.FileName}", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async Task StartScanAsync(string rootPath)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            _cts = new CancellationTokenSource();

            // UI Reset
            ResultTree.ItemsSource = null;
            ResultTree.Visibility = Visibility.Collapsed;
            DropZone.Visibility = Visibility.Visible; // Show drop zone while resetting
            ExportButton.Visibility = Visibility.Collapsed; // Hide export during scan
            _currentRootNode = null;
            
            StatusText.Text = "Scanning...";
            DetailText.Text = rootPath;

            var progress = new Progress<int>(count =>
            {
                // To avoid spamming the UI thread, we might want to throttle this, 
                // but for now let's see raw performance.
                // StatusText.Text = $"Scanning... {count} folders found"; 
                // Updating text on every folder is too heavy. Let's rely on the scanner completing.
                // Or update periodically if we had a more complex progress reporter.
            });

            try
            {
                // Config
                var config = new ScanConfig
                {
                    RootPath = rootPath,
                    MaxDepth = int.MaxValue, // Full scan
                    IncludeHidden = true,
                    SkipReparsePoints = true
                };

                // Run Scan
                var rootNode = await _scanner.ScanAsync(rootPath, config, progress, _cts.Token);
                _currentRootNode = rootNode; // Save for export

                // Bind Results
                var nodes = new List<DirectoryNode> { rootNode };
                ResultTree.ItemsSource = nodes;
                ResultTree.Visibility = Visibility.Visible;
                DropZone.Visibility = Visibility.Collapsed;
                ExportButton.Visibility = Visibility.Visible; // Show export button

                StatusText.Text = "Scan Complete";
                DetailText.Text = $"Found {rootNode.Stats.DeepFileCount:N0} files in {rootNode.Stats.DeepDirCount:N0} folders.";
            }
            catch (OperationCanceledException)
            {
                StatusText.Text = "Scan Canceled";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Error";
                MessageBox.Show($"Scan failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResultTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is DirectoryNode node)
            {
                DetailText.Text = node.FullPath;
            }
        }
    }
}
