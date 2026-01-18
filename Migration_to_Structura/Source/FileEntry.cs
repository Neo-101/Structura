namespace OmniArmory.Shared
{
    public class FileEntry
    {
        public string FullPath { get; set; }
        public long Size { get; set; }
        public string Hash { get; set; } // Placeholder for future hash calculation
        public bool IsScanError { get; set; } = false;

        [System.Text.Json.Serialization.JsonIgnore]
        public DirectoryNode Parent { get; set; }

        public FileEntry(string fullPath, long size)
        {
            FullPath = fullPath;
            Size = size;
        }

        public FileEntry() { } // For serialization
    }
}
