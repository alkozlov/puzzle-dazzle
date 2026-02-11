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
	public Cell StartCell { get; private set; }

	/// <summary>
	/// The ending cell of the maze.
	/// </summary>
	public Cell EndCell { get; private set; }

	/// <summary>
	/// Difficulty level of the maze (affects generation algorithm).
	/// </summary>
	public MazeDifficulty Difficulty { get; private set; }

	public Maze(int rows, int columns, MazeDifficulty difficulty = MazeDifficulty.Medium)
	{
		if (rows < 5 || columns < 5)
			throw new ArgumentException("Maze must be at least 5x5 cells");

		Rows = rows;
		Columns = columns;
		Difficulty = difficulty;

		// Initialize the grid
		Grid = new Cell[rows, columns];
		for (int row = 0; row < rows; row++)
		{
			for (int col = 0; col < columns; col++)
			{
				Grid[row, col] = new Cell(row, col);
			}
		}

		// Set start and end cells
		StartCell = Grid[0, 0];
		StartCell.IsStart = true;

		EndCell = Grid[rows - 1, columns - 1];
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
	/// Gets all unvisited neighbors of a cell.
	/// </summary>
	public List<Cell> GetUnvisitedNeighbors(Cell cell)
	{
		var neighbors = new List<Cell>();

		// Check top neighbor
		var top = GetCell(cell.Row - 1, cell.Column);
		if (top != null && !top.Visited)
			neighbors.Add(top);

		// Check right neighbor
		var right = GetCell(cell.Row, cell.Column + 1);
		if (right != null && !right.Visited)
			neighbors.Add(right);

		// Check bottom neighbor
		var bottom = GetCell(cell.Row + 1, cell.Column);
		if (bottom != null && !bottom.Visited)
			neighbors.Add(bottom);

		// Check left neighbor
		var left = GetCell(cell.Row, cell.Column - 1);
		if (left != null && !left.Visited)
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
