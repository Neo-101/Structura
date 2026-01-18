using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Structura.UI
{
    public static class SettingsManager
    {
        private static string _settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Structura");
        private static string _settingsFile = Path.Combine(_settingsFolder, "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(_settingsFile))
                {
                    string json = File.ReadAllText(_settingsFile);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    return settings ?? new AppSettings();
                }
            }
            catch
            {
                // Ignore errors, return default
            }
            return new AppSettings();
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                if (!Directory.Exists(_settingsFolder))
                {
                    Directory.CreateDirectory(_settingsFolder);
                }
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFile, json);
            }
            catch
            {
                // Ignore errors
            }
        }
    }
}
