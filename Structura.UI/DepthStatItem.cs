using System;

namespace Structura.UI
{
    public class DepthStatItem
    {
        public int Depth { get; set; }
        public long FolderCount { get; set; }
        public long FileCount { get; set; }
        public long Size { get; set; }

        public double FolderPercent { get; set; }
        public double FilePercent { get; set; }
        public double SizePercent { get; set; }
        
        public long EstimatedTokens { get; set; }

        public string SizeDisplay => FormatSize(Size);
        public string TokenDisplay => $"{EstimatedTokens:N0}";

        private string FormatSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }
    }
}
