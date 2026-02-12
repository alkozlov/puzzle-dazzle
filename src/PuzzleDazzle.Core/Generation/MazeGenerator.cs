using PuzzleDazzle.Core.Generation.Algorithms;
using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Generation;

/// <summary>
/// Generates mazes using Recursive Backtracking algorithm.
/// </summary>
public class MazeGenerator
{
	private readonly Random _random;
	private readonly RecursiveBacktrackingAlgorithm _algorithm;

	public MazeGenerator()
	{
		_random = new Random();
		_algorithm = new RecursiveBacktrackingAlgorithm();
	}

	public MazeGenerator(int seed)
	{
		_random = new Random(seed);
		_algorithm = new RecursiveBacktrackingAlgorithm();
	}

	/// <summary>
	/// Generates a maze asynchronously using Recursive Backtracking algorithm.
	/// </summary>
	/// <param name="rows">Number of rows in the maze.</param>
	/// <param name="columns">Number of columns in the maze.</param>
	/// <param name="difficulty">Difficulty level affecting maze complexity.</param>
	/// <param name="progress">Optional progress callback (0.0 to 1.0).</param>
	/// <returns>A generated Maze object.</returns>
	public async Task<Maze> GenerateAsync(
		int rows, 
		int columns, 
		MazeDifficulty difficulty = MazeDifficulty.Medium,
		IProgress<double>? progress = null)
	{
		return await Task.Run(() => Generate(rows, columns, difficulty, progress));
	}

	/// <summary>
	/// Generates a maze synchronously using Recursive Backtracking algorithm.
	/// </summary>
	/// <param name="rows">Number of rows in the maze.</param>
	/// <param name="columns">Number of columns in the maze.</param>
	/// <param name="difficulty">Difficulty level affecting maze complexity.</param>
	/// <param name="progress">Optional progress callback (0.0 to 1.0).</param>
	/// <returns>A generated Maze object.</returns>
	public Maze Generate(
		int rows, 
		int columns, 
		MazeDifficulty difficulty = MazeDifficulty.Medium,
		IProgress<double>? progress = null)
	{
		// Generate the maze using Recursive Backtracking algorithm
		return _algorithm.Generate(rows, columns, difficulty, progress, _random);
	}
}
