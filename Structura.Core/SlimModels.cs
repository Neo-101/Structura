using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Structura.Core
{
    // Optimized for LLM Context Window
    public class SlimDirectoryNode
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("dirs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SlimDirectoryNode> Dirs { get; set; }

        [JsonPropertyName("files")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SlimFileEntry> Files { get; set; }

        public SlimDirectoryNode(string name)
        {
            Name = name;
        }
    }

    public class SlimFileEntry
    {
        [JsonPropertyName("n")] // Very short for frequent items if we wanted, but 'name' is clearer
        public string Name { get; set; }

        [JsonPropertyName("s")] // Size
        public long Size { get; set; }

        [JsonPropertyName("m")] // Modified
        public string LastModified { get; set; }

        public SlimFileEntry(string name, long size, DateTime modified)
        {
            Name = name;
            Size = size;
            LastModified = modified.ToString("yyyy-MM-dd"); // ISO date only is usually enough for knowledge base context
        }
    }

    public class StructureStats
    {
        [JsonPropertyName("max_depth")]
        public int MaxDepth { get; set; }

        [JsonPropertyName("total_files")]
        public long TotalFiles { get; set; }

        [JsonPropertyName("total_folders")]
        public long TotalFolders { get; set; }

        [JsonPropertyName("depth_distribution")]
        public Dictionary<int, int> DepthDistribution { get; set; } // Depth -> Node Count (Files+Dirs)

        [JsonPropertyName("breadth_distribution")]
        public Dictionary<string, int> BreadthDistribution { get; set; } // Bucket ("0", "1-5") -> Folder Count
    }

    public class SlimExportModel
    {
        [JsonPropertyName("meta")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>
        {
            { "n", "Name" },
            { "s", "Size (Bytes)" },
            { "m", "Last Modified (yyyy-MM-dd)" }
        };

        [JsonPropertyName("stats")]
        public StructureStats Stats { get; set; }

        public string Root { get; set; }
        public SlimDirectoryNode Tree { get; set; }
        public string Generated { get; set; }

        public SlimExportModel(string root, SlimDirectoryNode tree)
        {
            Root = root;
            Tree = tree;
            Generated = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
