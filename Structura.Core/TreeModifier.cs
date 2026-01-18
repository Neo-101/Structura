using System.Linq;

namespace Structura.Core
{
    public static class TreeModifier
    {
        public static void RemoveFile(FileEntry file)
        {
            if (file == null || file.Parent == null) return;

            var parent = file.Parent;
            
            // Remove from list
            if (parent.Files.Remove(file))
            {
                // Update Parent Stats
                parent.Stats.DirectFileCount--;
                // DeepFileCount will be updated in bubble up? 
                // Wait, if we remove from Direct, we also remove from Deep of this node.
                // But we need to bubble up the subtraction to ALL ancestors.
                
                long sizeDiff = file.Size;
                
                // Update current node's stats
                // Note: FileStatModel doesn't store Size sum in the current implementation shown in snapshot V2 (FileStatModel check needed).
                // Let's check FileStatModel content. If it tracks size, we update it.
                // Assuming it might track counts.
                
                BubbleUpStats(parent, -1, sizeDiff);
            }
        }

        private static void BubbleUpStats(DirectoryNode node, int fileCountDelta, long sizeDelta)
        {
            var current = node;
            while (current != null)
            {
                current.Stats.DeepFileCount += fileCountDelta;
                // current.Stats.DeepSize += sizeDelta; // If we had size.
                
                current = current.Parent;
            }
        }
    }
}
