using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Tests.Models;

public class MazeTests
{
	[Fact]
	public void Constructor_ValidDimensions_CreatesCorrectSizedMaze()
	{
		// Arrange & Act
		var maze = new Maze(10, 20, MazeDifficulty.Medium);

		// Assert
		Assert.Equal(10, maze.Rows);
		Assert.Equal(20, maze.Columns);
		Assert.Equal(MazeDifficulty.Medium, maze.Difficulty);
		Assert.NotNull(maze.Grid);
	}

	[Theory]
	[InlineData(3, 10)]
	[InlineData(10, 3)]
	[InlineData(4, 4)]
	public void Constructor_DimensionsTooSmall_ThrowsArgumentException(int rows, int cols)
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Maze(rows, cols));
	}

	[Fact]
	public void Constructor_InitializesAllCells()
	{
		// Arrange & Act
		var maze = new Maze(5, 5);

		// Assert
		for (int row = 0; row < 5; row++)
		{
			for (int col = 0; col < 5; col++)
			{
				var cell = maze.Grid[row, col];
				Assert.NotNull(cell);
				Assert.Equal(row, cell.Row);
				Assert.Equal(col, cell.Column);
			}
		}
	}

	[Fact]
	public void Constructor_SetsStartAndEndCells()
	{
		// Arrange & Act
		var maze = new Maze(10, 10);

		// Assert
		Assert.NotNull(maze.StartCell);
		Assert.True(maze.StartCell.IsStart);
		Assert.Equal(0, maze.StartCell.Row);
		Assert.Equal(0, maze.StartCell.Column);

		Assert.NotNull(maze.EndCell);
		Assert.True(maze.EndCell.IsEnd);
		Assert.Equal(9, maze.EndCell.Row);
		Assert.Equal(9, maze.EndCell.Column);
	}

	[Theory]
	[InlineData(0, 0, true)]
	[InlineData(5, 5, true)]
	[InlineData(9, 9, true)]
	[InlineData(-1, 0, false)]
	[InlineData(0, -1, false)]
	[InlineData(10, 5, false)]
	[InlineData(5, 10, false)]
	public void GetCell_VariousPositions_ReturnsCorrectResult(int row, int col, bool shouldExist)
	{
		// Arrange
		var maze = new Maze(10, 10);

		// Act
		var cell = maze.GetCell(row, col);

		// Assert
		if (shouldExist)
		{
			Assert.NotNull(cell);
			Assert.Equal(row, cell.Row);
			Assert.Equal(col, cell.Column);
		}
		else
		{
			Assert.Null(cell);
		}
	}

	[Fact]
	public void GetUnvisitedNeighbors_CenterCell_ReturnsFourNeighbors()
	{
		// Arrange
		var maze = new Maze(10, 10);
		var centerCell = maze.Grid[5, 5];

		// Act
		var neighbors = maze.GetUnvisitedNeighbors(centerCell);

		// Assert
		Assert.Equal(4, neighbors.Count);
		Assert.Contains(neighbors, n => n.Row == 4 && n.Column == 5); // Top
		Assert.Contains(neighbors, n => n.Row == 6 && n.Column == 5); // Bottom
		Assert.Contains(neighbors, n => n.Row == 5 && n.Column == 4); // Left
		Assert.Contains(neighbors, n => n.Row == 5 && n.Column == 6); // Right
	}

	[Fact]
	public void GetUnvisitedNeighbors_CornerCell_ReturnsTwoNeighbors()
	{
		// Arrange
		var maze = new Maze(10, 10);
		var cornerCell = maze.Grid[0, 0];

		// Act
		var neighbors = maze.GetUnvisitedNeighbors(cornerCell);

		// Assert
		Assert.Equal(2, neighbors.Count);
		Assert.Contains(neighbors, n => n.Row == 1 && n.Column == 0); // Bottom
		Assert.Contains(neighbors, n => n.Row == 0 && n.Column == 1); // Right
	}

	[Fact]
	public void GetUnvisitedNeighbors_AllNeighborsVisited_ReturnsEmpty()
	{
		// Arrange
		var maze = new Maze(10, 10);
		var centerCell = maze.Grid[5, 5];
		
		// Mark all neighbors as visited
		maze.Grid[4, 5].Visited = true;
		maze.Grid[6, 5].Visited = true;
		maze.Grid[5, 4].Visited = true;
		maze.Grid[5, 6].Visited = true;

		// Act
		var neighbors = maze.GetUnvisitedNeighbors(centerCell);

		// Assert
		Assert.Empty(neighbors);
	}

	[Fact]
	public void ResetVisited_AllCellsMarkedVisited_ResetsAllToFalse()
	{
		// Arrange
		var maze = new Maze(10, 10);
		
		// Mark all cells as visited
		for (int row = 0; row < 10; row++)
		{
			for (int col = 0; col < 10; col++)
			{
				maze.Grid[row, col].Visited = true;
			}
		}

		// Act
		maze.ResetVisited();

		// Assert
		for (int row = 0; row < 10; row++)
		{
			for (int col = 0; col < 10; col++)
			{
				Assert.False(maze.Grid[row, col].Visited);
			}
		}
	}
}
