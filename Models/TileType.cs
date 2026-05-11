using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmVisualizer.Models
{
    public enum TileType
    {
        Unvisited,  // white
        Wall,       // dark gray - blocks path
        Start,      // green
        End,        // red
        Visited,    // light blue - explored by BFS
        Path        // yellow - final shortest path
    }
}