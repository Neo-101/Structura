namespace Structura.UI
{
    public class BreadthStatItem
    {
        public string Range { get; set; } // "0", "1-5", "6-10", etc.
        public long FolderCount { get; set; }
        public double FolderPercent { get; set; }
        public int SortOrder { get; set; } // Helper for sorting
    }
}