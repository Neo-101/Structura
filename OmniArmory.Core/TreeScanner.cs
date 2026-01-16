using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OmniArmory.Shared;

namespace OmniArmory.Core
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

            var dirInfo = new DirectoryInfo(path);
            var node = new DirectoryNode(dirInfo.FullName, dirInfo.Name);

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
                    if (!config.IncludeHidden && isHidden)
                    {
                        continue;
                    }

                    if (info is FileInfo)
                    {
                        node.Stats.DirectFileCount++;
                        node.Stats.DeepFileCount++;
                    }
                    else if (info is DirectoryInfo subDir)
                    {
                        node.Stats.DirectDirCount++;
                        node.Stats.DeepDirCount++; // Count the immediate child itself

                        // Determine if we should recurse
                        bool shouldRecurse = true;

                        // Check MaxDepth
                        if (config.MaxDepth > 0 && currentDepth >= config.MaxDepth)
                        {
                            shouldRecurse = false;
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
                                }
                            }
                        }

                        if (shouldRecurse)
                        {
                            var childNode = ScanRecursive(subDir.FullName, config, currentDepth + 1, progress, ct);
                            node.ChildrenList.Add(childNode);
                            
                            // Post-Order Accumulation
                            node.Stats.DeepFileCount += childNode.Stats.DeepFileCount;
                            node.Stats.DeepDirCount += childNode.Stats.DeepDirCount;
                            
                            if (childNode.Stats.IsPartial) node.Stats.IsPartial = true;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                node.Stats.IsAccessDenied = true;
                node.Stats.IsPartial = true;
            }
            catch (Exception ex)
            {
                // General error handling
                node.Stats.IsPartial = true;
                // Log error if we had a logger, for now silent recovery as per "Silent Recovery" pattern
            }

            return node;
        }
    }
}
