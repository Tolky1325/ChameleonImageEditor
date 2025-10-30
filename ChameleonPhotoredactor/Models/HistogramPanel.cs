using System.Collections.Generic;

namespace MyProject.Models
{
    public class HistogramPanel
    {
        public Dictionary<int, int> LightLevels { get; set; } = new Dictionary<int, int>();

        public void AddPixel(int level)
        {
            if (LightLevels.ContainsKey(level))
                LightLevels[level]++;
            else
                LightLevels[level] = 1;
        }
    }
}
