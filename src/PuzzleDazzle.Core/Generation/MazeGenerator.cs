using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Generation;

/// <summary>
/// Generates mazes using the recursive backtracking algorithm.
/// </summary>
public class MazeGenerator
{
	private readonly Random _random;

	public MazeGenerator()
	{
		_random = new Random();
	}

	public MazeGenerator(int seed)
	{
		_random = new Random(seed);
	}

	/// <summary>
	/// Generates a maze asynchronously using recursive backtracking algorithm.
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
	/// Generates a maze synchronously using recursive backtracking algorithm.
	/// </summary>
	public Maze Generate(
		int rows, 
		int columns, 
		MazeDifficulty difficulty = MazeDifficulty.Medium,
		IProgress<double>? progress = null)
	{
		var maze = new Maze(rows, columns, difficulty);
		
		// Total cells to visit for progress tracking
		int totalCells = rows * columns;
		int visitedCells = 0;

		// Start from the start cell
		var stack = new Stack<Cell>();
		var current = maze.StartCell;
		current.Visited = true;
		visitedCells++;
		
		progress?.Report((double)visitedCells / totalCells);

		while (true)
		{
			var unvisitedNeighbors = maze.GetUnvisitedNeighbors(current);

			if (unvisitedNeighbors.Count > 0)
			{
				// Choose a random unvisited neighbor
				var next = unvisitedNeighbors[_random.Next(unvisitedNeighbors.Count)];
				
				// Push current cell to stack
				stack.Push(current);
				
				// Remove wall between current and chosen neighbor
				current.RemoveWallBetween(next);
				
				// Move to chosen neighbor
				current = next;
				current.Visited = true;
				visitedCells++;
				
				// Report progress
				if (visitedCells % 10 == 0) // Update every 10 cells to avoid too frequent updates
				{
					progress?.Report((double)visitedCells / totalCells);
				}
			}
			else if (stack.Count > 0)
			{
				// Backtrack
				current = stack.Pop();
			}
			else
			{
				// All cells visited
				break;
			}
		}

		// Apply difficulty-based modifications
		ApplyDifficultyModifications(maze, difficulty);

		// Reset visited flags for potential pathfinding later
		maze.ResetVisited();

		// Final progress report
		progress?.Report(1.0);

		return maze;
	}

	/// <summary>
	/// Applies modifications based on difficulty level.
	/// </summary>
	private void ApplyDifficultyModifications(Maze maze, MazeDifficulty difficulty)
	{
		switch (difficulty)
		{
			case MazeDifficulty.Easy:
				// For easy difficulty, remove some additional walls to create shortcuts
				RemoveRandomWalls(maze, wallsToRemove: (int)(maze.Rows * maze.Columns * 0.15));
				break;

			case MazeDifficulty.Medium:
				// Medium keeps the generated maze as-is
				break;

			case MazeDifficulty.Hard:
				// For hard difficulty, the generated maze is already complex enough
				// Could add additional dead-ends or complexity here in future
				break;
		}
	}

	/// <summary>
	/// Removes random walls to create shortcuts and make the maze easier.
	/// </summary>
	private void RemoveRandomWalls(Maze maze, int wallsToRemove)
	{
		int removed = 0;
		int maxAttempts = wallsToRemove * 3; // Prevent infinite loop
		int attempts = 0;

		while (removed < wallsToRemove && attempts < maxAttempts)
		{
			attempts++;

			// Pick a random cell
			int row = _random.Next(maze.Rows);
			int col = _random.Next(maze.Columns);
			var cell = maze.Grid[row, col];

			// Skip start and end cells
			if (cell.IsStart || cell.IsEnd)
				continue;

			// Try to remove a random wall
			int wallDirection = _random.Next(4); // 0=top, 1=right, 2=bottom, 3=left

			switch (wallDirection)
			{
				case 0: // Top
					if (cell.TopWall && row > 0)
					{
						var neighbor = maze.Grid[row - 1, col];
						cell.RemoveWallBetween(neighbor);
						removed++;
					}
					break;

				case 1: // Right
					if (cell.RightWall && col < maze.Columns - 1)
					{
						var neighbor = maze.Grid[row, col + 1];
						cell.RemoveWallBetween(neighbor);
						removed++;
					}
					break;

				case 2: // Bottom
					if (cell.BottomWall && row < maze.Rows - 1)
					{
						var neighbor = maze.Grid[row + 1, col];
						cell.RemoveWallBetween(neighbor);
						removed++;
					}
					break;

				case 3: // Left
					if (cell.LeftWall && col > 0)
					{
						var neighbor = maze.Grid[row, col - 1];
						cell.RemoveWallBetween(neighbor);
						removed++;
					}
					break;
			}
		}
	}
}
