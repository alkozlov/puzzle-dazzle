using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Tests.Models;

public class MazeShapeTests
{
	[Fact]
	public void Rectangle_AllCellsActive()
	{
		// Arrange & Act
		var shape = MazeShape.Rectangle(10, 10);

		// Assert
		Assert.Equal(10, shape.Rows);
		Assert.Equal(10, shape.Columns);
		Assert.Equal(ShapeType.Rectangle, shape.Type);
		
		for (int row = 0; row < 10; row++)
		{
			for (int col = 0; col < 10; col++)
			{
				Assert.True(shape.IsActive(row, col), $"Cell ({row}, {col}) should be active");
			}
		}
	}

	[Fact]
	public void Circle_CenterCellsActive()
	{
		// Arrange & Act
		var shape = MazeShape.Circle(20, 20);

		// Assert
		Assert.Equal(ShapeType.Circle, shape.Type);
		
		// Center cell should definitely be active
		Assert.True(shape.IsActive(10, 10), "Center cell should be active");
		
		// Corners should be inactive (outside circle)
		Assert.False(shape.IsActive(0, 0), "Top-left corner should be inactive");
		Assert.False(shape.IsActive(0, 19), "Top-right corner should be inactive");
		Assert.False(shape.IsActive(19, 0), "Bottom-left corner should be inactive");
		Assert.False(shape.IsActive(19, 19), "Bottom-right corner should be inactive");
	}

	[Fact]
	public void Diamond_CenterAndEdgeMidpointsActive()
	{
		// Arrange & Act
		var shape = MazeShape.Diamond(20, 20);

		// Assert
		Assert.Equal(ShapeType.Diamond, shape.Type);
		
		// Center should be active
		Assert.True(shape.IsActive(10, 10), "Center should be active");
		
		// Edge midpoints should be active (or very close to edge)
		Assert.True(shape.IsActive(0, 10), "Top midpoint should be active");
		Assert.True(shape.IsActive(19, 10), "Bottom midpoint should be active");
		Assert.True(shape.IsActive(10, 0), "Left midpoint should be active");
		Assert.True(shape.IsActive(10, 19), "Right midpoint should be active");
		
		// Corners should be inactive
		Assert.False(shape.IsActive(0, 0), "Top-left corner should be inactive");
		Assert.False(shape.IsActive(0, 19), "Top-right corner should be inactive");
	}

	[Fact]
	public void Heart_HasCorrectShape()
	{
		// Arrange & Act
		var shape = MazeShape.Heart(30, 30);

		// Assert
		Assert.Equal(ShapeType.Heart, shape.Type);
		
		// Should have some active cells
		int activeCells = 0;
		for (int row = 0; row < 30; row++)
		{
			for (int col = 0; col < 30; col++)
			{
				if (shape.IsActive(row, col))
					activeCells++;
			}
		}
		
		// Heart should have fewer cells than rectangle but more than 0
		Assert.True(activeCells > 0, "Heart should have active cells");
		Assert.True(activeCells < 900, "Heart should have fewer cells than full rectangle");
	}

	[Fact]
	public void IsActive_OutOfBounds_ReturnsFalse()
	{
		// Arrange
		var shape = MazeShape.Rectangle(10, 10);

		// Act & Assert
		Assert.False(shape.IsActive(-1, 5));
		Assert.False(shape.IsActive(5, -1));
		Assert.False(shape.IsActive(10, 5));
		Assert.False(shape.IsActive(5, 10));
	}

	[Fact]
	public void FromType_CreatesCorrectShape()
	{
		// Arrange & Act
		var rectangle = MazeShape.FromType(ShapeType.Rectangle, 10, 10);
		var circle = MazeShape.FromType(ShapeType.Circle, 10, 10);
		var diamond = MazeShape.FromType(ShapeType.Diamond, 10, 10);
		var heart = MazeShape.FromType(ShapeType.Heart, 10, 10);

		// Assert
		Assert.Equal(ShapeType.Rectangle, rectangle.Type);
		Assert.Equal(ShapeType.Circle, circle.Type);
		Assert.Equal(ShapeType.Diamond, diamond.Type);
		Assert.Equal(ShapeType.Heart, heart.Type);
	}

	[Fact]
	public void Circle_HasFewerCellsThanRectangle()
	{
		// Arrange & Act
		var circle = MazeShape.Circle(20, 20);
		var rectangle = MazeShape.Rectangle(20, 20);

		// Assert - Circle should have fewer active cells than rectangle
		int circleActiveCells = 0;
		int rectangleActiveCells = 0;
		
		for (int i = 0; i < 20; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				if (circle.IsActive(i, j)) circleActiveCells++;
				if (rectangle.IsActive(i, j)) rectangleActiveCells++;
			}
		}

		Assert.Equal(400, rectangleActiveCells); // Full grid
		Assert.True(circleActiveCells < rectangleActiveCells, "Circle should have fewer cells than rectangle");
		Assert.True(circleActiveCells > 200, "Circle should have more than half the cells");
	}
}
