using System.Collections.Generic;

namespace OmniArmory.Shared
{
    public class DirectoryNode : IDirectoryNode
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public FileStatModel Stats { get; set; } = new FileStatModel();
        public List<DirectoryNode> ChildrenList { get; set; } = new List<DirectoryNode>();

        public IEnumerable<IDirectoryNode> Children => ChildrenList;

        public DirectoryNode(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
        }
    }
}
