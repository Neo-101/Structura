using System.Collections.Generic;

namespace OmniArmory.Shared
{
    public interface IDirectoryNode
    {
        string Name { get; }
        string FullPath { get; }
        FileStatModel Stats { get; }
        IEnumerable<IDirectoryNode> Children { get; }
    }
}
