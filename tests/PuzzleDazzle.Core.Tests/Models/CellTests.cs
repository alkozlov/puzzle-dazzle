using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Tests.Models;

public class CellTests
{
	[Fact]
	public void Constructor_InitializesCorrectly()
	{
		// Act
		var cell = new Cell(3, 5);

		// Assert
		Assert.Equal(3, cell.Row);
		Assert.Equal(5, cell.Column);
		Assert.True(cell.TopWall);
		Assert.True(cell.RightWall);
		Assert.True(cell.BottomWall);
		Assert.True(cell.LeftWall);
		Assert.False(cell.Visited);
		Assert.False(cell.IsStart);
		Assert.False(cell.IsEnd);
	}

	[Fact]
	public void RemoveWallBetween_TopNeighbor_RemovesCorrectWalls()
	{
		// Arrange
		var current = new Cell(5, 5);
		var top = new Cell(4, 5);

		// Act
		current.RemoveWallBetween(top);

		// Assert
		Assert.False(current.TopWall);
		Assert.False(top.BottomWall);
		Assert.True(current.BottomWall);
		Assert.True(current.LeftWall);
		Assert.True(current.RightWall);
	}

	[Fact]
	public void RemoveWallBetween_BottomNeighbor_RemovesCorrectWalls()
	{
		// Arrange
		var current = new Cell(5, 5);
		var bottom = new Cell(6, 5);

		// Act
		current.RemoveWallBetween(bottom);

		// Assert
		Assert.False(current.BottomWall);
		Assert.False(bottom.TopWall);
		Assert.True(current.TopWall);
		Assert.True(current.LeftWall);
		Assert.True(current.RightWall);
	}

	[Fact]
	public void RemoveWallBetween_LeftNeighbor_RemovesCorrectWalls()
	{
		// Arrange
		var current = new Cell(5, 5);
		var left = new Cell(5, 4);

		// Act
		current.RemoveWallBetween(left);

		// Assert
		Assert.False(current.LeftWall);
		Assert.False(left.RightWall);
		Assert.True(current.TopWall);
		Assert.True(current.BottomWall);
		Assert.True(current.RightWall);
	}

	[Fact]
	public void RemoveWallBetween_RightNeighbor_RemovesCorrectWalls()
	{
		// Arrange
		var current = new Cell(5, 5);
		var right = new Cell(5, 6);

		// Act
		current.RemoveWallBetween(right);

		// Assert
		Assert.False(current.RightWall);
		Assert.False(right.LeftWall);
		Assert.True(current.TopWall);
		Assert.True(current.BottomWall);
		Assert.True(current.LeftWall);
	}
}
