namespace PuzzleDazzle.Core.Models;

/// <summary>
/// Specifies the algorithm used for maze generation.
/// </summary>
public enum MazeAlgorithm
{
    /// <summary>
    /// Recursive Backtracking (Depth-First Search).
    /// Creates perfect mazes with long, winding corridors.
    /// Fast and memory efficient.
    /// </summary>
    RecursiveBacktracking = 0,

    /// <summary>
    /// Prim's Algorithm (Randomized).
    /// Creates perfect mazes with more branching and shorter passages.
    /// Medium speed.
    /// </summary>
    Prims = 1,

    /// <summary>
    /// Kruskal's Algorithm (Randomized Minimum Spanning Tree).
    /// Creates perfect mazes by treating it as a graph problem.
    /// Good balance of characteristics.
    /// </summary>
    Kruskals = 2,

    /// <summary>
    /// Wilson's Algorithm (Loop-Erased Random Walk).
    /// Creates perfectly unbiased mazes with uniform distribution.
    /// Slower but produces the most "random" looking mazes.
    /// </summary>
    Wilsons = 3
}
