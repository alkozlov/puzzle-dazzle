namespace PuzzleDazzle.Core.Models;

/// <summary>
/// Defines the shape of a maze using a boolean mask.
/// True = active cell (part of the maze), False = inactive cell (not part of the maze).
/// </summary>
public class MazeShape
{
	/// <summary>
	/// The type of shape.
	/// </summary>
	public ShapeType Type { get; private set; }

	/// <summary>
	/// The mask defining which cells are active (true) or inactive (false).
	/// Dimensions: [rows, columns]
	/// </summary>
	public bool[,] Mask { get; private set; }

	/// <summary>
	/// Number of rows in the shape.
	/// </summary>
	public int Rows { get; private set; }

	/// <summary>
	/// Number of columns in the shape.
	/// </summary>
	public int Columns { get; private set; }

	private MazeShape(ShapeType type, int rows, int columns, bool[,] mask)
	{
		Type = type;
		Rows = rows;
		Columns = columns;
		Mask = mask;
	}

	/// <summary>
	/// Checks if a cell at the given position is active (part of the maze).
	/// </summary>
	public bool IsActive(int row, int column)
	{
		if (row < 0 || row >= Rows || column < 0 || column >= Columns)
			return false;

		return Mask[row, column];
	}

	/// <summary>
	/// Creates a standard rectangular shape (all cells active).
	/// </summary>
	public static MazeShape Rectangle(int rows, int columns)
	{
		var mask = new bool[rows, columns];
		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < columns; col++)
			{
				mask[row, col] = true;
			}
		}

		return new MazeShape(ShapeType.Rectangle, rows, columns, mask);
	}

	/// <summary>
	/// Creates a circular shape.
	/// </summary>
	public static MazeShape Circle(int rows, int columns)
	{
		var mask = new bool[rows, columns];
		
		// Calculate center and radius
		double centerRow = rows / 2.0;
		double centerCol = columns / 2.0;
		double radiusRow = rows / 2.0;
		double radiusCol = columns / 2.0;

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < columns; col++)
			{
				// Normalize to unit circle (ellipse formula)
				double normalizedRow = (row - centerRow) / radiusRow;
				double normalizedCol = (col - centerCol) / radiusCol;
				
				// Check if point is inside the circle/ellipse
				double distance = normalizedRow * normalizedRow + normalizedCol * normalizedCol;
				mask[row, col] = distance <= 1.0;
			}
		}

		return new MazeShape(ShapeType.Circle, rows, columns, mask);
	}

	/// <summary>
	/// Creates a diamond shape.
	/// </summary>
	public static MazeShape Diamond(int rows, int columns)
	{
		var mask = new bool[rows, columns];
		
		// Calculate center
		double centerRow = rows / 2.0;
		double centerCol = columns / 2.0;

		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < columns; col++)
			{
				// Manhattan distance from center, normalized
				double normalizedDistance = 
					Math.Abs(row - centerRow) / (rows / 2.0) + 
					Math.Abs(col - centerCol) / (columns / 2.0);
				
				// Inside diamond if normalized Manhattan distance <= 1
				mask[row, col] = normalizedDistance <= 1.0;
			}
		}

		return new MazeShape(ShapeType.Diamond, rows, columns, mask);
	}

	/// <summary>
	/// Creates a heart shape (for fun!).
	/// </summary>
	public static MazeShape Heart(int rows, int columns)
	{
		var mask = new bool[rows, columns];
		
		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < columns; col++)
			{
				// Normalize coordinates to [-1, 1] range
				double x = (col - columns / 2.0) / (columns / 2.0);
				double y = -(row - rows / 2.0) / (rows / 2.0); // Flip Y for standard orientation
				
				// Heart equation: (x^2 + y^2 - 1)^3 - x^2*y^3 <= 0
				// Scaled and adjusted for better shape
				double heart = Math.Pow(x * x + y * y - 0.8, 3) - x * x * y * y * y * 0.5;
				
				mask[row, col] = heart <= 0;
			}
		}

		return new MazeShape(ShapeType.Heart, rows, columns, mask);
	}

	/// <summary>
	/// Gets a shape by its type.
	/// </summary>
	public static MazeShape FromType(ShapeType type, int rows, int columns)
	{
		return type switch
		{
			ShapeType.Rectangle => Rectangle(rows, columns),
			ShapeType.Circle => Circle(rows, columns),
			ShapeType.Diamond => Diamond(rows, columns),
			ShapeType.Heart => Heart(rows, columns),
			_ => Rectangle(rows, columns) // Default to rectangle
		};
	}
}

/// <summary>
/// Types of predefined maze shapes.
/// </summary>
public enum ShapeType
{
	/// <summary>
	/// Standard rectangular maze (all cells active).
	/// </summary>
	Rectangle = 0,

	/// <summary>
	/// Circular/oval maze.
	/// </summary>
	Circle = 1,

	/// <summary>
	/// Diamond-shaped maze.
	/// </summary>
	Diamond = 2,

	/// <summary>
	/// Heart-shaped maze.
	/// </summary>
	Heart = 3
}
