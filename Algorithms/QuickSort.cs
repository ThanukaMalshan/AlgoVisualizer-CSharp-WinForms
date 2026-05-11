using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmVisualizer.Models;

namespace AlgorithmVisualizer.Algorithms
{
    public class QuickSort
    {
        private List<SortStep> steps;
        private HashSet<int> sorted;
        private int[] arr;

        public List<SortStep> GenerateSteps(int[] inputArray)
        {
            steps = new List<SortStep>();
            sorted = new HashSet<int>();
            arr = (int[])inputArray.Clone();

            QuickSortHelper(0, arr.Length - 1);

            return steps;
        }

        private void QuickSortHelper(int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(low, high);
                sorted.Add(pivotIndex);
                QuickSortHelper(low, pivotIndex - 1);
                QuickSortHelper(pivotIndex + 1, high);
            }
            else if (low == high)
            {
                sorted.Add(low);
            }
        }

        private int Partition(int low, int high)
        {
            int pivot = arr[high]; // pivot is always the last element
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                // Record: comparing current element with pivot
                steps.Add(new SortStep
                {
                    ArrayState = (int[])arr.Clone(),
                    CompareA = j,
                    CompareB = high, // high = pivot position
                    SwapA = -1,
                    SwapB = -1,
                    IsComparison = true,
                    SortedIndices = new HashSet<int>(sorted)
                });

                if (arr[j] <= pivot)
                {
                    i++;

                    // Swap arr[i] and arr[j]
                    int temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;

                    // Record: swap happened
                    steps.Add(new SortStep
                    {
                        ArrayState = (int[])arr.Clone(),
                        CompareA = -1,
                        CompareB = -1,
                        SwapA = i,
                        SwapB = j,
                        IsComparison = false,
                        SortedIndices = new HashSet<int>(sorted)
                    });
                }
            }

            // Place pivot in its final position
            int tmp = arr[i + 1];
            arr[i + 1] = arr[high];
            arr[high] = tmp;

            steps.Add(new SortStep
            {
                ArrayState = (int[])arr.Clone(),
                CompareA = -1,
                CompareB = -1,
                SwapA = i + 1,
                SwapB = high,
                IsComparison = false,
                SortedIndices = new HashSet<int>(sorted)
            });

            return i + 1;
        }
    }
}