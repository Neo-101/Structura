using System;
using System.Collections.Generic;

namespace Structura.UI
{
    public class AppSettings
    {
        public List<string> RecentPaths { get; set; } = new List<string>();
        public string ExportDirectory { get; set; }
    }
}
