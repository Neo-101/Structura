using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Structura.Core
{
    public class TreeScanner
    {
        public async Task<DirectoryNode> ScanAsync(string rootPath, ScanConfig config, IProgress<int> progress = null, CancellationToken ct = default)
        {
            return await Task.Run(() => ScanRecursive(rootPath, config, 0, progress, ct), ct);
        }

        private DirectoryNode ScanRecursive(string path, ScanConfig config, int currentDepth, IProgress<int> progress, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            // Handle Long Paths (> 248 chars for Directory, 260 for File)
            // While .NET 4.6.2+ supports long paths without \\?\, Win7 often needs it.
            // And if manifest isn't respected (Win7), we need \\?\.
            // Safest way for scanning is to ensure path is valid for DirectoryInfo.
            // But modifying 'path' to have \\?\ prefix might propagate weird paths to UI.
            // We'll trust the Manifest for Win10, and for Win7 users might face limits unless we use \\?\.
            // Let's rely on standard DirectoryInfo for now as per "Silent Recovery".
            // If PathTooLongException occurs, we catch it.
            
            DirectoryInfo dirInfo;
            try 
            {
                dirInfo = new DirectoryInfo(path);
            }
            catch (PathTooLongException)
            {
                // Fallback attempt with \\?\ prefix? 
                // Note: \\?\ only works with absolute paths.
                // But DirectoryInfo might still throw if runtime checks are strict.
                // We'll just mark as Partial/Error for now to avoid crashing.
                var errNode = new DirectoryNode(path, System.IO.Path.GetFileName(path));
                errNode.Depth = currentDepth;
                errNode.Stats.IsPartial = true;
                return errNode;
            }

            var node = new DirectoryNode(dirInfo.FullName, dirInfo.Name);
            node.Depth = currentDepth;

            // Calculate Relative Path for the current node
            string nodeRelativePath = ""; // Default to empty for root, or handle specifically
            if (dirInfo.FullName.Length > config.RootPath.Length)
            {
                nodeRelativePath = dirInfo.FullName.Substring(config.RootPath.Length);
                if (nodeRelativePath.StartsWith(Path.DirectorySeparatorChar.ToString()) || nodeRelativePath.StartsWith(Path.AltDirectorySeparatorChar.ToString()))
                {
                     nodeRelativePath = nodeRelativePath.Substring(1);
                }
            }
            // For the root node itself, typically we might want "." or "" depending on convention.
            // If it's empty, it means it's the root. 
            // The user example shows path without leading slash/dot.
            node.RelativePath = nodeRelativePath;

            // Report progress (1 folder scanned)
            progress?.Report(1);

            try
            {
                // Optimization: EnumerateFileSystemInfos is faster than GetFiles + GetDirectories
                // because it iterates the MFT only once.
                foreach (var info in dirInfo.EnumerateFileSystemInfos())
                {
                    ct.ThrowIfCancellationRequested();

                    bool isHidden = (info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                    bool isDotHidden = info.Name.StartsWith("."); // Also treat dot-files/folders as hidden (e.g. .obsidian, .git)
                    
                    if (!config.IncludeHidden && (isHidden || isDotHidden))
                    {
                        continue;
                    }

                    if (info is FileInfo fileInfo)
                    {
                        node.Stats.DirectFileCount++;
                        node.Stats.DeepFileCount++;
                        
                        string ext = fileInfo.Extension.ToLowerInvariant();
                        if (string.IsNullOrEmpty(ext)) ext = "(no-extension)";

                        if (node.Stats.ExtensionDistribution == null) node.Stats.ExtensionDistribution = new Dictionary<string, long>();

                        if (node.Stats.ExtensionDistribution.ContainsKey(ext))
                        {
                            node.Stats.ExtensionDistribution[ext]++;
                        }
                        else
                        {
                            node.Stats.ExtensionDistribution[ext] = 1;
                        }

                        var fileEntry = new FileEntry(fileInfo.FullName, fileInfo.Name, fileInfo.Length, fileInfo.LastWriteTime);
                        
                        // Calculate File Relative Path
                        string fileRelativePath = fileInfo.FullName.Substring(config.RootPath.Length);
                        if (fileRelativePath.StartsWith(Path.DirectorySeparatorChar.ToString()) || fileRelativePath.StartsWith(Path.AltDirectorySeparatorChar.ToString()))
                        {
                             fileRelativePath = fileRelativePath.Substring(1);
                        }
                        fileEntry.RelativePath = fileRelativePath;

                        fileEntry.Parent = node;
                        node.Files.Add(fileEntry);
                    }
                    else if (info is DirectoryInfo subDir)
                    {
                        node.Stats.DirectDirCount++;
                        node.Stats.DeepDirCount++; // Count the immediate child itself

                        // Determine if we should recurse
                        bool shouldRecurse = true;
                        bool includeInTree = true;

                        // Check MaxDepth
                        if (config.MaxDepth > 0 && currentDepth >= config.MaxDepth)
                        {
                            shouldRecurse = false;
                            includeInTree = false; // Don't show items beyond max depth
                        }

                        // Check Reparse Points
                        if (shouldRecurse)
                        {
                            bool isReparse = (subDir.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
                            if (config.SkipReparsePoints && isReparse)
                            {
                                // Exception: OneDrive/Cloud folders often have Offline attribute.
                                // We want to count their metadata if possible, but traversing them might be slow or trigger download?
                                // User said: "Exception: If it's a OneDrive folder... count its metadata (Direct/Deep counts)"
                                // This implies we SHOULD recurse if it's a cloud folder, even if SkipReparsePoints is true.
                                // Typically, OneDrive folders have FileAttributes.Offline or just ReparsePoint (Data on Demand).
                                // A safe heuristic for "System Junction/Symlink" vs "Cloud Placeholder":
                                // .NET 4.8 doesn't expose LinkTarget or ReparseTag easily.
                                // However, most Junctions do NOT have the Offline attribute.
                                // Cloud placeholders often DO (or at least used to).
                                // Let's try: Recurse if it is Offline, even if Reparse.
                                
                                bool isOffline = (subDir.Attributes & FileAttributes.Offline) == FileAttributes.Offline;
                                if (!isOffline)
                                {
                                    shouldRecurse = false;
                                    // But we still want to see the reparse point in the tree (as a leaf)
                                    includeInTree = true;
                                }
                            }
                        }

                        if (shouldRecurse)
                        {
                            var childNode = ScanRecursive(subDir.FullName, config, currentDepth + 1, progress, ct);
                            childNode.Parent = node;
                            node.ChildrenList.Add(childNode);
                            
                            // Post-Order Accumulation
                            node.Stats.Add(childNode.Stats);
                            
                            if (childNode.Stats.IsPartial) node.Stats.IsPartial = true;
                        }
                        else if (includeInTree)
                        {
                            // Even if we don't recurse (ReparsePoint), we want the folder to appear in the tree
                            var childNode = new DirectoryNode(subDir.FullName, subDir.Name);
                            childNode.Depth = currentDepth + 1;
                            
                            // Calculate Relative Path for non-recursive node
                            string childRelativePath = ".";
                            if (subDir.FullName.Length > config.RootPath.Length)
                            {
                                childRelativePath = subDir.FullName.Substring(config.RootPath.Length);
                                if (childRelativePath.StartsWith(Path.DirectorySeparatorChar.ToString()) || childRelativePath.StartsWith(Path.AltDirectorySeparatorChar.ToString()))
                                {
                                     childRelativePath = childRelativePath.Substring(1);
                                }
                            }
                            childNode.RelativePath = childRelativePath;

                            childNode.Parent = node;
                            node.ChildrenList.Add(childNode);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                node.Stats.IsAccessDenied = true;
                node.Stats.IsPartial = true;
            }
            catch (PathTooLongException)
            {
                 node.Stats.IsPartial = true;
            }
            catch (Exception)
            {
                // General error handling
                node.Stats.IsPartial = true;
                // Log error if we had a logger, for now silent recovery as per "Silent Recovery" pattern
            }

            // Post-processing for leaf folders
            // If this node has no children folders (ChildrenList is empty), it is a leaf folder.
            // We can remove RelativePath from its files to save space/redundancy.
            if (node.ChildrenList.Count == 0 && node.Files.Count > 0)
            {
                foreach (var file in node.Files)
                {
                    file.RelativePath = null;
                }
            }

            return node;
        }
    }
}
