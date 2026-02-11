using PuzzleDazzle.Core.Generation;
using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Tests.Generation;

public class MazeGeneratorTests
{
	[Fact]
	public void Generate_ValidDimensions_CreatesMazeWithCorrectSize()
	{
		// Arrange
		var generator = new MazeGenerator();

		// Act
		var maze = generator.Generate(10, 15, MazeDifficulty.Medium);

		// Assert
		Assert.NotNull(maze);
		Assert.Equal(10, maze.Rows);
		Assert.Equal(15, maze.Columns);
	}

	[Fact]
	public void Generate_MarksAllCellsAsVisited()
	{
		// Arrange
		var generator = new MazeGenerator();

		// Act
		var maze = generator.Generate(10, 10, MazeDifficulty.Medium);

		// Assert - After generation, all cells should be visited (then reset)
		// The maze should have removed walls creating paths
		int cellsWithRemovedWalls = 0;
		for (int row = 0; row < 10; row++)
		{
			for (int col = 0; col < 10; col++)
			{
				var cell = maze.Grid[row, col];
				// At least one wall should be removed for most cells
				if (!cell.TopWall || !cell.RightWall || !cell.BottomWall || !cell.LeftWall)
				{
					cellsWithRemovedWalls++;
				}
			}
		}
		
		// Almost all cells should have at least one wall removed (except possibly some corners)
		Assert.True(cellsWithRemovedWalls >= 95); // At least 95 out of 100 cells
	}

	[Fact]
	public void Generate_CreatesConnectedMaze()
	{
		// Arrange
		var generator = new MazeGenerator(seed: 42); // Use seed for reproducibility

		// Act
		var maze = generator.Generate(10, 10, MazeDifficulty.Medium);

		// Assert - Verify maze is connected by checking that we can reach all cells from start
		var reachable = new HashSet<(int, int)>();
		var queue = new Queue<Cell>();
		queue.Enqueue(maze.StartCell);
		reachable.Add((maze.StartCell.Row, maze.StartCell.Column));

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			
			// Check all four neighbors
			if (!current.TopWall)
			{
				var neighbor = maze.GetCell(current.Row - 1, current.Column);
				if (neighbor != null && reachable.Add((neighbor.Row, neighbor.Column)))
					queue.Enqueue(neighbor);
			}
			if (!current.RightWall)
			{
				var neighbor = maze.GetCell(current.Row, current.Column + 1);
				if (neighbor != null && reachable.Add((neighbor.Row, neighbor.Column)))
					queue.Enqueue(neighbor);
			}
			if (!current.BottomWall)
			{
				var neighbor = maze.GetCell(current.Row + 1, current.Column);
				if (neighbor != null && reachable.Add((neighbor.Row, neighbor.Column)))
					queue.Enqueue(neighbor);
			}
			if (!current.LeftWall)
			{
				var neighbor = maze.GetCell(current.Row, current.Column - 1);
				if (neighbor != null && reachable.Add((neighbor.Row, neighbor.Column)))
					queue.Enqueue(neighbor);
			}
		}

		// All 100 cells should be reachable
		Assert.Equal(100, reachable.Count);
	}

	[Fact]
	public void Generate_WithSeed_ProducesSameMaze()
	{
		// Arrange
		var generator1 = new MazeGenerator(seed: 123);
		var generator2 = new MazeGenerator(seed: 123);

		// Act
		var maze1 = generator1.Generate(10, 10, MazeDifficulty.Medium);
		var maze2 = generator2.Generate(10, 10, MazeDifficulty.Medium);

		// Assert - Both mazes should be identical
		for (int row = 0; row < 10; row++)
		{
			for (int col = 0; col < 10; col++)
			{
				var cell1 = maze1.Grid[row, col];
				var cell2 = maze2.Grid[row, col];

				Assert.Equal(cell1.TopWall, cell2.TopWall);
				Assert.Equal(cell1.RightWall, cell2.RightWall);
				Assert.Equal(cell1.BottomWall, cell2.BottomWall);
				Assert.Equal(cell1.LeftWall, cell2.LeftWall);
			}
		}
	}

	[Fact]
	public void Generate_EasyDifficulty_HasFewerWallsThanMedium()
	{
		// Arrange
		var generator = new MazeGenerator(seed: 42);

		// Act
		var easyMaze = generator.Generate(20, 20, MazeDifficulty.Easy);
		var generator2 = new MazeGenerator(seed: 42);
		var mediumMaze = generator2.Generate(20, 20, MazeDifficulty.Medium);

		// Assert - Easy maze should have more removed walls (more shortcuts)
		int easyWallCount = CountWalls(easyMaze);
		int mediumWallCount = CountWalls(mediumMaze);

		Assert.True(easyWallCount < mediumWallCount, 
			$"Easy maze should have fewer walls ({easyWallCount}) than medium maze ({mediumWallCount})");
	}

	[Fact]
	public async Task GenerateAsync_CreatesValidMaze()
	{
		// Arrange
		var generator = new MazeGenerator();

		// Act
		var maze = await generator.GenerateAsync(10, 10, MazeDifficulty.Medium);

		// Assert
		Assert.NotNull(maze);
		Assert.Equal(10, maze.Rows);
		Assert.Equal(10, maze.Columns);
	}

	[Fact]
	public async Task GenerateAsync_ReportsProgress()
	{
		// Arrange
		var generator = new MazeGenerator();
		var progressValues = new List<double>();
		var progress = new Progress<double>(value => progressValues.Add(value));

		// Act
		await generator.GenerateAsync(20, 20, MazeDifficulty.Medium, progress);

		// Assert
		Assert.NotEmpty(progressValues);
		Assert.Contains(progressValues, p => p > 0 && p < 1); // Should have intermediate values
		Assert.Equal(1.0, progressValues.Last()); // Should end at 100%
	}

	[Fact]
	public void Generate_ResetsVisitedFlags()
	{
		// Arrange
		var generator = new MazeGenerator();

		// Act
		var maze = generator.Generate(10, 10, MazeDifficulty.Medium);

		// Assert - All cells should have Visited = false after generation
		for (int row = 0; row < 10; row++)
		{
			for (int col = 0; col < 10; col++)
			{
				Assert.False(maze.Grid[row, col].Visited);
			}
		}
	}

	// Helper method to count total walls in a maze
	private int CountWalls(Maze maze)
	{
		int wallCount = 0;
		for (int row = 0; row < maze.Rows; row++)
		{
			for (int col = 0; col < maze.Columns; col++)
			{
				var cell = maze.Grid[row, col];
				if (cell.TopWall) wallCount++;
				if (cell.RightWall) wallCount++;
				if (cell.BottomWall) wallCount++;
				if (cell.LeftWall) wallCount++;
			}
		}
		return wallCount;
	}
}
