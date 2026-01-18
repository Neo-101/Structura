using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Structura.Core;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

using System.Linq;
using System.Collections.Generic;

namespace Structura.UI
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DirectoryNode> _rootNodes;
        public ObservableCollection<DirectoryNode> RootNodes
        {
            get => _rootNodes;
            set { _rootNodes = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ScanError> _scanErrors = new ObservableCollection<ScanError>();
        public ObservableCollection<ScanError> ScanErrors
        {
            get => _scanErrors;
            set { _scanErrors = value; OnPropertyChanged(); }
        }

        private bool _hasErrors;
        public bool HasErrors
        {
            get => _hasErrors;
            set { _hasErrors = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _recentPaths;
        public ObservableCollection<string> RecentPaths
        {
            get => _recentPaths;
            set { _recentPaths = value; OnPropertyChanged(); }
        }

        private string _selectedPath;
        public string SelectedPath
        {
            get => _selectedPath;
            set { _selectedPath = value; OnPropertyChanged(); }
        }

        private string _exportDirectory;
        public string ExportDirectory
        {
            get => _exportDirectory;
            set 
            { 
                _exportDirectory = value; 
                OnPropertyChanged(); 
                SaveSettings(); // Auto-save when changed
            }
        }

        private bool _includeHidden = false;
        public bool IncludeHidden
        {
            get => _includeHidden;
            set { _includeHidden = value; OnPropertyChanged(); }
        }

        private bool _isUnlimitedDepth = true;
        public bool IsUnlimitedDepth
        {
            get => _isUnlimitedDepth;
            set 
            { 
                _isUnlimitedDepth = value; 
                OnPropertyChanged(); 
                // When toggled off, if MaxDepth was huge, reset to something reasonable like 2
                if (!value && _maxDepth > 100) 
                {
                    MaxDepth = 2;
                }
            }
        }

        private int _maxDepth = 2;
        public int MaxDepth
        {
            get => _maxDepth;
            set { _maxDepth = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            RootNodes = new ObservableCollection<DirectoryNode>();
            
            var settings = SettingsManager.Load();
            RecentPaths = new ObservableCollection<string>(settings.RecentPaths);
            if (RecentPaths.Count > 0)
            {
                SelectedPath = RecentPaths[0];
            }
            
            if (!string.IsNullOrEmpty(settings.ExportDirectory) && Directory.Exists(settings.ExportDirectory))
            {
                ExportDirectory = settings.ExportDirectory;
            }
            else
            {
                // Default to Downloads
                ExportDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
            }
        }
        
        private void SaveSettings()
        {
             var settings = new AppSettings 
             { 
                 RecentPaths = new System.Collections.Generic.List<string>(RecentPaths),
                 ExportDirectory = ExportDirectory
             };
             SettingsManager.Save(settings);
        }

        public void AddToHistory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            // Remove if exists to move to top
            if (RecentPaths.Contains(path))
            {
                RecentPaths.Remove(path);
            }

            RecentPaths.Insert(0, path);

            // Limit history size
            while (RecentPaths.Count > 10)
            {
                RecentPaths.RemoveAt(RecentPaths.Count - 1);
            }

            SelectedPath = path;

            // Save settings
            SaveSettings();
        }

        public async Task ScanAsync(string path)
        {
            ScanErrors.Clear();
            HasErrors = false;

            var scanner = new TreeScanner();
            var config = new ScanConfig 
            { 
                RootPath = path,
                MaxDepth = IsUnlimitedDepth ? -1 : MaxDepth,
                IncludeHidden = IncludeHidden 
            };

            var rootNode = await scanner.ScanAsync(path, config);
            
            RootNodes.Clear();
            RootNodes.Add(rootNode);

            CollectErrors(rootNode);
            if (ScanErrors.Count > 0)
            {
                HasErrors = true;
            }
        }

        private void CollectErrors(DirectoryNode node)
        {
            if (node.Stats.IsAccessDenied)
            {
                ScanErrors.Add(new ScanError(node.FullPath, "Access Denied"));
                return;
            }
            
            if (node.Stats.IsPartial)
            {
                bool childCaused = false;
                foreach (var child in node.ChildrenList)
                {
                    if (child.Stats.IsPartial)
                    {
                        childCaused = true;
                        CollectErrors(child);
                    }
                }
                
                if (!childCaused)
                {
                     ScanErrors.Add(new ScanError(node.FullPath, "Incomplete Scan (Path too long or IO Error)"));
                }
            }
        }

        public long ExportJson(string filePath)
        {
            if (RootNodes.Count == 0) return 0;

            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
            };
            
            var rootNode = RootNodes[0];
            
            // Map to Slim Model for LLM Optimization
            var slimTree = MapToSlim(rootNode, IsUnlimitedDepth ? 9999 : MaxDepth);
            var exportModel = new SlimExportModel(rootNode.FullPath, slimTree);

            // Inject Stats
            var depthStats = AnalyzeDepthDistribution();
            var breadthStats = AnalyzeBreadthDistribution();

            exportModel.Stats = new StructureStats
            {
                MaxDepth = depthStats.Count > 0 ? depthStats.Max(d => d.Depth) : 0,
                TotalFolders = depthStats.Sum(d => d.FolderCount),
                TotalFiles = depthStats.Sum(d => d.FileCount),
                DepthDistribution = depthStats.ToDictionary(d => d.Depth, d => (int)(d.FolderCount + d.FileCount)),
                BreadthDistribution = breadthStats.ToDictionary(b => b.Range, b => (int)b.FolderCount)
            };

            // Export the wrapper model
            var jsonString = JsonSerializer.Serialize(exportModel, options);
            File.WriteAllText(filePath, jsonString);

            // Estimate tokens (approx 4 chars per token)
            return (long)(jsonString.Length / 4.0);
        }

        private SlimDirectoryNode MapToSlim(DirectoryNode node)
        {
            var slimNode = new SlimDirectoryNode(node.Name);

            if (node.Files.Count > 0)
            {
                slimNode.Files = new System.Collections.Generic.List<SlimFileEntry>();
                foreach (var file in node.Files)
                {
                    slimNode.Files.Add(new SlimFileEntry(file.Name, file.Size, file.LastModified));
                }
            }

            if (node.ChildrenList.Count > 0)
            {
                slimNode.Dirs = new System.Collections.Generic.List<SlimDirectoryNode>();
                foreach (var child in node.ChildrenList)
                {
                    slimNode.Dirs.Add(MapToSlim(child));
                }
            }

            return slimNode;
        }

        public List<DepthStatItem> AnalyzeDepthDistribution()
        {
            if (RootNodes == null || RootNodes.Count == 0) return new List<DepthStatItem>();

            var stats = new Dictionary<int, DepthStatItem>();
            long totalFiles = 0;
            long totalFolders = 0;
            long totalSize = 0;

            // We need to calculate cumulative tokens for "Scan Depth = N"
            // This means for Depth N, we need to generate JSON for tree cut at Depth N.
            // Doing this by actual serialization is slow. We can estimate.
            // Or, we can do it smartly:
            // 1. Traverse and tag every node with its depth.
            // 2. "Tokens at Depth N" = Tokens of the entire tree IF we pruned everything > N.
            
            // Let's do a fast estimation pass.
            // Calculate base tokens for each node (name + overhead).
            // Accumulate these up to depth N.

            // BFS Traversal for Stats
            var queue = new Queue<DirectoryNode>();
            queue.Enqueue(RootNodes[0]);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                
                if (!stats.ContainsKey(node.Depth))
                {
                    stats[node.Depth] = new DepthStatItem { Depth = node.Depth };
                }

                stats[node.Depth].FolderCount++;
                totalFolders++;

                if (node.Files != null)
                {
                    stats[node.Depth].FileCount += node.Files.Count;
                    long nodeSize = node.Files.Sum(f => f.Size);
                    stats[node.Depth].Size += nodeSize;

                    totalFiles += node.Files.Count;
                    totalSize += nodeSize;
                }

                foreach (var child in node.ChildrenList)
                {
                    queue.Enqueue(child);
                }
            }

            var result = stats.Values.OrderBy(x => x.Depth).ToList();

            // Calculate Percentages and Cumulative Tokens
            // For tokens: We need to simulate the export at each depth.
            // Since we have the full tree in memory, we can just MapToSlim with a maxDepth constraint and measure length.
            // This might be slightly expensive if the tree is huge, but usually fine for < 100k files.
            
            // Optimization: Only calculate for existing depths
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
            };

            var rootNode = RootNodes[0];

            foreach (var item in result)
            {
                if (totalFolders > 0) item.FolderPercent = (double)item.FolderCount / totalFolders * 100.0;
                if (totalFiles > 0) item.FilePercent = (double)item.FileCount / totalFiles * 100.0;
                if (totalSize > 0) item.SizePercent = (double)item.Size / totalSize * 100.0;

                // Calculate Tokens for this specific depth setting
                // We reuse MapToSlim but we need a version that accepts maxDepth
                var slimTree = MapToSlim(rootNode, item.Depth);
                var exportModel = new SlimExportModel(rootNode.FullPath, slimTree);
                
                // We don't need to write to disk, just serialize to string
                string json = JsonSerializer.Serialize(exportModel, options);
                item.EstimatedTokens = (long)(json.Length / 4.0);
            }

            return result;
        }
        
        public List<BreadthStatItem> AnalyzeBreadthDistribution()
        {
            if (RootNodes == null || RootNodes.Count == 0) return new List<BreadthStatItem>();

            var buckets = new Dictionary<string, BreadthStatItem>();
            long totalFolders = 0;

            // BFS Traversal
            var queue = new Queue<DirectoryNode>();
            queue.Enqueue(RootNodes[0]);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                totalFolders++;

                // Width = Direct Files + Direct Subdirectories
                int width = node.Files.Count + node.ChildrenList.Count;

                string bucketKey;
                int sortOrder;

                if (width == 0) { bucketKey = "0 (Empty)"; sortOrder = 0; }
                else if (width <= 5) { bucketKey = "1-5"; sortOrder = 1; }
                else if (width <= 10) { bucketKey = "6-10"; sortOrder = 2; }
                else if (width <= 20) { bucketKey = "11-20"; sortOrder = 3; }
                else if (width <= 50) { bucketKey = "21-50"; sortOrder = 4; }
                else if (width <= 100) { bucketKey = "51-100"; sortOrder = 5; }
                else { bucketKey = "100+"; sortOrder = 6; }

                if (!buckets.ContainsKey(bucketKey))
                {
                    buckets[bucketKey] = new BreadthStatItem { Range = bucketKey, SortOrder = sortOrder };
                }
                buckets[bucketKey].FolderCount++;

                foreach (var child in node.ChildrenList)
                {
                    queue.Enqueue(child);
                }
            }

            var result = buckets.Values.OrderBy(x => x.SortOrder).ToList();

            foreach (var item in result)
            {
                if (totalFolders > 0) item.FolderPercent = (double)item.FolderCount / totalFolders * 100.0;
            }

            return result;
        }

        // Overload for Depth-Limited mapping
        private SlimDirectoryNode MapToSlim(DirectoryNode node, int maxDepth)
        {
            // If we are deeper than maxDepth, we don't return this node at all?
            // Wait, logic: If user selects Depth 1, they see Root (0) -> Children (1).
            // So if node.Depth > maxDepth, we prune it.
            // However, MapToSlim is recursive.
            // We should check: if node.Depth > maxDepth, return null? 
            // But we need to handle the root call.
            
            // Actually, better logic: 
            // In the recursion, if child.Depth > maxDepth, don't add it.
            
            var slimNode = new SlimDirectoryNode(node.Name);

            // Files are always included if the folder is included (implied by requirement "scan to target folder... and directly contained files")
            if (node.Files.Count > 0)
            {
                slimNode.Files = new System.Collections.Generic.List<SlimFileEntry>();
                foreach (var file in node.Files)
                {
                    slimNode.Files.Add(new SlimFileEntry(file.Name, file.Size, file.LastModified));
                }
            }

            if (node.ChildrenList.Count > 0)
            {
                // Only add children if they are within the depth limit
                var validChildren = new System.Collections.Generic.List<SlimDirectoryNode>();
                foreach (var child in node.ChildrenList)
                {
                    if (child.Depth <= maxDepth)
                    {
                        var childSlim = MapToSlim(child, maxDepth);
                        if (childSlim != null)
                        {
                            validChildren.Add(childSlim);
                        }
                    }
                }
                
                if (validChildren.Count > 0)
                {
                    slimNode.Dirs = validChildren;
                }
            }

            return slimNode;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
