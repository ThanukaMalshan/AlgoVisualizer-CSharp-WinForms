using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmVisualizer.Models;

namespace AlgorithmVisualizer.Algorithms
{
    public class InsertionSort
    {
        public List<SortStep> GenerateSteps(int[] inputArray)
        {
            var steps = new List<SortStep>();
            int[] arr = (int[])inputArray.Clone();
            var sorted = new HashSet<int>();
            int n = arr.Length;

            sorted.Add(0); // first element is trivially sorted

            for (int i = 1; i < n; i++)
            {
                int key = arr[i];
                int j = i - 1;

                while (j >= 0 && arr[j] > key)
                {
                    // Record: comparing arr[j] and arr[j+1]
                    steps.Add(new SortStep
                    {
                        ArrayState = (int[])arr.Clone(),
                        CompareA = j,
                        CompareB = j + 1,
                        SwapA = -1,
                        SwapB = -1,
                        IsComparison = true,
                        SortedIndices = new HashSet<int>(sorted)
                    });

                    // Shift element right
                    arr[j + 1] = arr[j];
                    j--;

                    // Record: the shift happened
                    steps.Add(new SortStep
                    {
                        ArrayState = (int[])arr.Clone(),
                        CompareA = -1,
                        CompareB = -1,
                        SwapA = j + 1,
                        SwapB = j + 2,
                        IsComparison = false,
                        SortedIndices = new HashSet<int>(sorted)
                    });
                }

                arr[j + 1] = key;
                sorted.Add(i);

                // Record: element placed in sorted position
                steps.Add(new SortStep
                {
                    ArrayState = (int[])arr.Clone(),
                    CompareA = -1,
                    CompareB = -1,
                    SwapA = -1,
                    SwapB = -1,
                    IsComparison = false,
                    SortedIndices = new HashSet<int>(sorted)
                });
            }

            return steps;
        }
    }
}
