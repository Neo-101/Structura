namespace Structura.Core
{
    public class ScanConfig
    {
        public string RootPath { get; set; }
        public int MaxDepth { get; set; } = 2;
        public bool IncludeHidden { get; set; } = false;
        public bool SkipReparsePoints { get; set; } = true;
    }
}
