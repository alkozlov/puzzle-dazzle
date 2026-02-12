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
    RecursiveBacktracking = 0
}
