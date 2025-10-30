using System.Collections.Generic;

namespace MyProject.Models
{
    public class ExifPanel
    {
        public Dictionary<string, string> ExifData { get; set; } = new Dictionary<string, string>();

        public void AddExifData(string key, string value)
        {
            if (!ExifData.ContainsKey(key))
                ExifData.Add(key, value);
        }
    }
}
