namespace PuzzleDazzle.Core.Models;

/// <summary>
/// Represents a single cell in the maze grid.
/// </summary>
public class Cell
{
	/// <summary>
	/// Row position in the maze grid (0-based).
	/// </summary>
	public int Row { get; set; }

	/// <summary>
	/// Column position in the maze grid (0-based).
	/// </summary>
	public int Column { get; set; }

	/// <summary>
	/// Indicates if the top wall is present.
	/// </summary>
	public bool TopWall { get; set; }

	/// <summary>
	/// Indicates if the right wall is present.
	/// </summary>
	public bool RightWall { get; set; }

	/// <summary>
	/// Indicates if the bottom wall is present.
	/// </summary>
	public bool BottomWall { get; set; }

	/// <summary>
	/// Indicates if the left wall is present.
	/// </summary>
	public bool LeftWall { get; set; }

	/// <summary>
	/// Indicates if this cell has been visited during maze generation.
	/// </summary>
	public bool Visited { get; set; }

	/// <summary>
	/// Indicates if this cell is the start point of the maze.
	/// </summary>
	public bool IsStart { get; set; }

	/// <summary>
	/// Indicates if this cell is the end point of the maze.
	/// </summary>
	public bool IsEnd { get; set; }

	public Cell(int row, int column)
	{
		Row = row;
		Column = column;
		
		// Initially all walls are present
		TopWall = true;
		RightWall = true;
		BottomWall = true;
		LeftWall = true;
		
		Visited = false;
		IsStart = false;
		IsEnd = false;
	}

	/// <summary>
	/// Removes the wall between this cell and the specified neighbor cell.
	/// </summary>
	public void RemoveWallBetween(Cell neighbor)
	{
		// Determine which wall to remove based on neighbor position
		int rowDiff = neighbor.Row - Row;
		int colDiff = neighbor.Column - Column;

		if (rowDiff == -1) // Neighbor is above
		{
			TopWall = false;
			neighbor.BottomWall = false;
		}
		else if (rowDiff == 1) // Neighbor is below
		{
			BottomWall = false;
			neighbor.TopWall = false;
		}
		else if (colDiff == -1) // Neighbor is to the left
		{
			LeftWall = false;
			neighbor.RightWall = false;
		}
		else if (colDiff == 1) // Neighbor is to the right
		{
			RightWall = false;
			neighbor.LeftWall = false;
		}
	}
}
