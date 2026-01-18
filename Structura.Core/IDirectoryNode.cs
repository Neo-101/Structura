using System.Collections.Generic;

namespace Structura.Core
{
    public interface IDirectoryNode
    {
        string Name { get; }
        string FullPath { get; }
        FileStatModel Stats { get; }
        IEnumerable<IDirectoryNode> Children { get; }
    }
}
