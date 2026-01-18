using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OmniArmory.Shared
{
    public class DirectoryNode : IDirectoryNode
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
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
