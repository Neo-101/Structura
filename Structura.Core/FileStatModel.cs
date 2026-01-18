using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Structura.Core
{
    public class FileStatModel
    {
        public long DirectFileCount { get; set; }
        public long DirectDirCount { get; set; }
        public long DeepFileCount { get; set; }
        public long DeepDirCount { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsPartial { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsAccessDenied { get; set; }

        [JsonPropertyName("ext_stats")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, long> ExtensionDistribution { get; set; }

        public void Add(FileStatModel other)
        {
            if (other == null) return;
            DeepFileCount += other.DeepFileCount;
            DeepDirCount += other.DeepDirCount;
            if (other.IsPartial) IsPartial = true;

            if (other.ExtensionDistribution != null && other.ExtensionDistribution.Count > 0)
            {
                if (ExtensionDistribution == null) ExtensionDistribution = new Dictionary<string, long>();

                foreach (var kvp in other.ExtensionDistribution)
                {
                    if (ExtensionDistribution.ContainsKey(kvp.Key))
                    {
                        ExtensionDistribution[kvp.Key] += kvp.Value;
                    }
                    else
                    {
                        ExtensionDistribution[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
    }
}
