namespace OmniArmory.Shared
{
    public class FileStatModel
    {
        public long DirectFileCount { get; set; }
        public long DirectDirCount { get; set; }
        public long DeepFileCount { get; set; }
        public long DeepDirCount { get; set; }
        
        public bool IsPartial { get; set; }
        public bool IsAccessDenied { get; set; }

        public void Add(FileStatModel other)
        {
            if (other == null) return;
            DeepFileCount += other.DeepFileCount;
            DeepDirCount += other.DeepDirCount;
            if (other.IsPartial) IsPartial = true;
        }
    }
}
