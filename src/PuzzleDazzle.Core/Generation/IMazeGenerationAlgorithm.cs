using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Generation;

/// <summary>
/// Interface for maze generation algorithms.
/// Defines the contract for implementing different maze generation strategies.
/// </summary>
public interface IMazeGenerationAlgorithm
{
    /// <summary>
    /// Gets the name of the algorithm.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a brief description of the algorithm and its characteristics.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Generates a maze using this algorithm.
    /// </summary>
    /// <param name="rows">Number of rows in the maze.</param>
    /// <param name="columns">Number of columns in the maze.</param>
    /// <param name="difficulty">Difficulty level affecting maze complexity.</param>
    /// <param name="progress">Optional progress reporter for long-running operations.</param>
    /// <param name="random">Random number generator for reproducible results.</param>
    /// <returns>A generated maze.</returns>
    Maze Generate(int rows, int columns, MazeDifficulty difficulty, IProgress<double>? progress, Random random);
}
