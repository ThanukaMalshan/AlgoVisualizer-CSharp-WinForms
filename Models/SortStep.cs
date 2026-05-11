using System.Collections.Generic;

namespace AlgorithmVisualizer.Models
{
    public class SortStep
    {
        public int[] ArrayState { get; set; }   // full array at this moment
        public int CompareA { get; set; } = -1; // index being compared (red)
        public int CompareB { get; set; } = -1; // index being compared (red)
        public int SwapA { get; set; } = -1;    // index being swapped (orange)
        public int SwapB { get; set; } = -1;    // index being swapped (orange)
        public bool IsComparison { get; set; }  // true = count this as a comparison
        public HashSet<int> SortedIndices { get; set; } = new HashSet<int>(); // green bars
    }
}