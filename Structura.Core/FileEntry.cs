using System.Text.Json.Serialization;

namespace Structura.Core
{
    public class FileEntry
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string FullPath { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RelativePath { get; set; }
        public long Size { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Hash { get; set; } // Placeholder for future hash calculation
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsScanError { get; set; } = false;

        public System.DateTime LastModified { get; set; }

        [JsonIgnore]
        public DirectoryNode Parent { get; set; }

        public FileEntry(string fullPath, string name, long size, System.DateTime lastModified)
        {
            FullPath = fullPath;
            Name = name;
            Size = size;
            LastModified = lastModified;
        }

        public FileEntry() { } // For serialization
    }
}
