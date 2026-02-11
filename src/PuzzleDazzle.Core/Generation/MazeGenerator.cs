using PuzzleDazzle.Core.Generation.Algorithms;
using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Generation;

/// <summary>
/// Generates mazes using various algorithms.
/// Supports Recursive Backtracking, Prim's, Kruskal's, and Wilson's algorithms.
/// </summary>
public class MazeGenerator
{
	private readonly Random _random;
	private readonly Dictionary<MazeAlgorithm, IMazeGenerationAlgorithm> _algorithms;

	public MazeGenerator()
	{
		_random = new Random();
		_algorithms = InitializeAlgorithms();
	}

	public MazeGenerator(int seed)
	{
		_random = new Random(seed);
		_algorithms = InitializeAlgorithms();
	}

	/// <summary>
	/// Initializes all available maze generation algorithms.
	/// </summary>
	private Dictionary<MazeAlgorithm, IMazeGenerationAlgorithm> InitializeAlgorithms()
	{
		return new Dictionary<MazeAlgorithm, IMazeGenerationAlgorithm>
		{
			{ MazeAlgorithm.RecursiveBacktracking, new RecursiveBacktrackingAlgorithm() },
			{ MazeAlgorithm.Prims, new PrimsAlgorithm() },
			{ MazeAlgorithm.Kruskals, new KruskalsAlgorithm() },
			{ MazeAlgorithm.Wilsons, new WilsonsAlgorithm() }
		};
	}

	/// <summary>
	/// Generates a maze asynchronously using the specified algorithm.
	/// </summary>
	/// <param name="rows">Number of rows in the maze.</param>
	/// <param name="columns">Number of columns in the maze.</param>
	/// <param name="difficulty">Difficulty level affecting maze complexity.</param>
	/// <param name="algorithm">Algorithm to use for generation.</param>
	/// <param name="progress">Optional progress callback (0.0 to 1.0).</param>
	/// <returns>A generated Maze object.</returns>
	public async Task<Maze> GenerateAsync(
		int rows, 
		int columns, 
		MazeDifficulty difficulty = MazeDifficulty.Medium,
		MazeAlgorithm algorithm = MazeAlgorithm.RecursiveBacktracking,
		IProgress<double>? progress = null)
	{
		return await Task.Run(() => Generate(rows, columns, difficulty, algorithm, progress));
	}

	/// <summary>
	/// Generates a maze synchronously using the specified algorithm.
	/// </summary>
	/// <param name="rows">Number of rows in the maze.</param>
	/// <param name="columns">Number of columns in the maze.</param>
	/// <param name="difficulty">Difficulty level affecting maze complexity.</param>
	/// <param name="algorithm">Algorithm to use for generation.</param>
	/// <param name="progress">Optional progress callback (0.0 to 1.0).</param>
	/// <returns>A generated Maze object.</returns>
	public Maze Generate(
		int rows, 
		int columns, 
		MazeDifficulty difficulty = MazeDifficulty.Medium,
		MazeAlgorithm algorithm = MazeAlgorithm.RecursiveBacktracking,
		IProgress<double>? progress = null)
	{
		// Get the selected algorithm
		if (!_algorithms.TryGetValue(algorithm, out var algorithmImpl))
		{
			// Fallback to Recursive Backtracking if algorithm not found
			algorithmImpl = _algorithms[MazeAlgorithm.RecursiveBacktracking];
		}

		// Generate the maze using the selected algorithm
		return algorithmImpl.Generate(rows, columns, difficulty, progress, _random);
	}

	/// <summary>
	/// Gets the algorithm implementation for a given algorithm type.
	/// </summary>
	public IMazeGenerationAlgorithm GetAlgorithm(MazeAlgorithm algorithm)
	{
		return _algorithms.TryGetValue(algorithm, out var impl) 
			? impl 
			: _algorithms[MazeAlgorithm.RecursiveBacktracking];
	}
}
