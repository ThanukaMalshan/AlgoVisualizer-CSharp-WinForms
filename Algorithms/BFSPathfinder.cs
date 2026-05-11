using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AlgorithmVisualizer.Models;

namespace AlgorithmVisualizer.Algorithms
{
    public class BFSPathfinder
    {
        public List<PathStep> GenerateSteps(
            TileType[,] grid, Point start, Point end, int rows, int cols)
        {
            var steps = new List<PathStep>();
            var queue = new Queue<Point>();
            var visited = new HashSet<Point>();
            var parent = new Dictionary<Point, Point>();

            // Row and column directions: up, down, left, right
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            queue.Enqueue(start);
            visited.Add(start);
            bool found = false;

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();

                // Record as visited (skip start and end - they keep their own colour)
                if (!current.Equals(start) && !current.Equals(end))
                {
                    steps.Add(new PathStep { Node = current, IsPath = false });
                }

                // Found the end!
                if (current.Equals(end))
                {
                    found = true;
                    break;
                }

                // Check all 4 neighbours
                for (int d = 0; d < 4; d++)
                {
                    int nr = current.X + dr[d];
                    int nc = current.Y + dc[d];
                    Point next = new Point(nr, nc);

                    bool inBounds = nr >= 0 && nr < rows && nc >= 0 && nc < cols;
                    bool notWall = inBounds && grid[nr, nc] != TileType.Wall;
                    bool notVisited = !visited.Contains(next);

                    if (inBounds && notWall && notVisited)
                    {
                        queue.Enqueue(next);
                        visited.Add(next);
                        parent[next] = current; // remember how we got here
                    }
                }
            }

            // If we found the end, trace back the shortest path
            if (found)
            {
                var path = new List<Point>();
                Point p = end;

                while (parent.ContainsKey(p))
                {
                    path.Add(p);
                    p = parent[p];
                }

                path.Reverse(); // reverse so path goes start → end

                foreach (Point node in path)
                {
                    // Don't colour the end tile - it keeps its red colour
                    if (!node.Equals(end))
                    {
                        steps.Add(new PathStep { Node = node, IsPath = true });
                    }
                }
            }

            return steps;
        }
    }
}