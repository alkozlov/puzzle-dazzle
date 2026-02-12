namespace PuzzleDazzle.Core.Models;

/// <summary>
/// Represents a maze with a grid of cells.
/// </summary>
public class Maze
{
	/// <summary>
	/// Number of rows in the maze.
	/// </summary>
	public int Rows { get; private set; }

	/// <summary>
	/// Number of columns in the maze.
	/// </summary>
	public int Columns { get; private set; }

	/// <summary>
	/// 2D array of cells that make up the maze grid.
	/// </summary>
	public Cell[,] Grid { get; private set; }

	/// <summary>
	/// The starting cell of the maze.
	/// </summary>
	public Cell StartCell { get; private set; } = null!;

	/// <summary>
	/// The ending cell of the maze.
	/// </summary>
	public Cell EndCell { get; private set; } = null!;

	/// <summary>
	/// Difficulty level of the maze (affects generation algorithm).
	/// </summary>
	public MazeDifficulty Difficulty { get; private set; }

	/// <summary>
	/// The shape defining which cells are active in the maze.
	/// </summary>
	public MazeShape Shape { get; private set; }

	public Maze(int rows, int columns, MazeDifficulty difficulty = MazeDifficulty.Medium, MazeShape? shape = null)
	{
		if (rows < 5 || columns < 5)
			throw new ArgumentException("Maze must be at least 5x5 cells");

		Rows = rows;
		Columns = columns;
		Difficulty = difficulty;
		
		// Use provided shape or default to rectangle
		Shape = shape ?? MazeShape.Rectangle(rows, columns);

		// Initialize the grid
		Grid = new Cell[rows, columns];
		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < columns; col++)
			{
				Grid[row, col] = new Cell(row, col);
				
				// Mark inactive cells based on shape mask
				if (!Shape.IsActive(row, col))
				{
					Grid[row, col].IsActive = false;
				}
			}
		}

		// Set start and end cells to first and last active cells
		SetStartAndEndCells();
	}
	
	/// <summary>
	/// Sets the start and end cells to the first and last active cells in the maze.
	/// </summary>
	private void SetStartAndEndCells()
	{
		Cell? firstActive = null;
		Cell? lastActive = null;
		
		// Find first active cell (top-left to bottom-right)
		for (int row = 0; row < Rows && firstActive == null; row++)
		{
			for (int col = 0; col < Columns && firstActive == null; col++)
			{
				if (Grid[row, col].IsActive)
				{
					firstActive = Grid[row, col];
				}
			}
		}
		
		// Find last active cell (bottom-right to top-left)
		for (int row = Rows - 1; row >= 0 && lastActive == null; row--)
		{
			for (int col = Columns - 1; col >= 0 && lastActive == null; col--)
			{
				if (Grid[row, col].IsActive)
				{
					lastActive = Grid[row, col];
				}
			}
		}
		
		if (firstActive == null || lastActive == null)
			throw new InvalidOperationException("Maze shape must have at least 2 active cells");
		
		StartCell = firstActive;
		StartCell.IsStart = true;
		
		EndCell = lastActive;
		EndCell.IsEnd = true;
	}

	/// <summary>
	/// Gets a cell at the specified position.
	/// </summary>
	public Cell? GetCell(int row, int column)
	{
		if (row < 0 || row >= Rows || column < 0 || column >= Columns)
			return null;

		return Grid[row, column];
	}

	/// <summary>
	/// Gets all unvisited neighbors of a cell (only active cells).
	/// </summary>
	public List<Cell> GetUnvisitedNeighbors(Cell cell)
	{
		var neighbors = new List<Cell>();

		// Check top neighbor
		var top = GetCell(cell.Row - 1, cell.Column);
		if (top != null && !top.Visited && top.IsActive)
			neighbors.Add(top);

		// Check right neighbor
		var right = GetCell(cell.Row, cell.Column + 1);
		if (right != null && !right.Visited && right.IsActive)
			neighbors.Add(right);

		// Check bottom neighbor
		var bottom = GetCell(cell.Row + 1, cell.Column);
		if (bottom != null && !bottom.Visited && bottom.IsActive)
			neighbors.Add(bottom);

		// Check left neighbor
		var left = GetCell(cell.Row, cell.Column - 1);
		if (left != null && !left.Visited && left.IsActive)
			neighbors.Add(left);

		return neighbors;
	}

	/// <summary>
	/// Resets the visited state of all cells (used during generation).
	/// </summary>
	public void ResetVisited()
	{
		for (int row = 0; row < Rows; row++)
		{
			for (int col = 0; col < Columns; col++)
			{
				Grid[row, col].Visited = false;
			}
		}
	}
}

/// <summary>
/// Difficulty levels for maze generation.
/// </summary>
public enum MazeDifficulty
{
	/// <summary>
	/// Easy - More straightforward paths, fewer dead ends.
	/// </summary>
	Easy,

	/// <summary>
	/// Medium - Balanced complexity.
	/// </summary>
	Medium,

	/// <summary>
	/// Hard - More complex paths, more dead ends.
	/// </summary>
	Hard
}
