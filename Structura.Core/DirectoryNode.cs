using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Structura.Core
{
    public class DirectoryNode : IDirectoryNode
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
        
        [JsonPropertyName("depth_level")]
        public int Depth { get; set; }
        
        public FileStatModel Stats { get; set; } = new FileStatModel();
        
        public List<FileEntry> Files { get; set; } = new List<FileEntry>();
        
        public List<DirectoryNode> ChildrenList { get; set; } = new List<DirectoryNode>();

        [JsonIgnore]
        public DirectoryNode Parent { get; set; }

        [JsonIgnore]
        public IEnumerable<IDirectoryNode> Children => ChildrenList;

        public DirectoryNode(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
        }

        // Required for deserialization if needed, though mostly for export here.
        public DirectoryNode() { } 
    }
}
