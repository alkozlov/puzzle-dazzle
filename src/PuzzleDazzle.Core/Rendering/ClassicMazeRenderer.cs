using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Rendering;

/// <summary>
/// Classic black and white maze renderer.
/// Renders mazes with thin black walls on white background.
/// </summary>
public class ClassicMazeRenderer : IMazeRenderer
{
	private const float WallThickness = 2.0f;
	private const float MinCellSize = 10.0f;

	/// <summary>
	/// Renders the maze using classic black and white style.
	/// </summary>
	public void Render(ICanvas canvas, Maze maze, float width, float height)
	{
		if (canvas == null || maze == null)
			return;

		// Calculate cell size based on available space
		float cellWidth = width / maze.Columns;
		float cellHeight = height / maze.Rows;
		float cellSize = Math.Min(cellWidth, cellHeight);

		// Center the maze if there's extra space
		float offsetX = (width - (cellSize * maze.Columns)) / 2;
		float offsetY = (height - (cellSize * maze.Rows)) / 2;

		// Clear background to white
		canvas.FillColor = Colors.White;
		canvas.FillRectangle(0, 0, width, height);

		// Draw maze walls in black
		canvas.StrokeColor = Colors.Black;
		canvas.StrokeSize = WallThickness;

		// Draw cells
		for (int row = 0; row < maze.Rows; row++)
		{
			for (int col = 0; col < maze.Columns; col++)
			{
				var cell = maze.Grid[row, col];
				float x = offsetX + (col * cellSize);
				float y = offsetY + (row * cellSize);

				// Draw walls for this cell
				if (cell.TopWall)
				{
					canvas.DrawLine(x, y, x + cellSize, y);
				}

				if (cell.RightWall)
				{
					canvas.DrawLine(x + cellSize, y, x + cellSize, y + cellSize);
				}

				if (cell.BottomWall)
				{
					canvas.DrawLine(x, y + cellSize, x + cellSize, y + cellSize);
				}

				if (cell.LeftWall)
				{
					canvas.DrawLine(x, y, x, y + cellSize);
				}
			}
		}

		// Highlight start and end points
		DrawStartEndMarkers(canvas, maze, cellSize, offsetX, offsetY);
	}

	/// <summary>
	/// Draws markers for start and end points.
	/// </summary>
	private void DrawStartEndMarkers(ICanvas canvas, Maze maze, float cellSize, float offsetX, float offsetY)
	{
		// Draw start marker (green circle)
		var startX = offsetX + (maze.StartCell.Column * cellSize) + (cellSize / 2);
		var startY = offsetY + (maze.StartCell.Row * cellSize) + (cellSize / 2);
		float markerRadius = cellSize * 0.3f;

		canvas.FillColor = Color.FromArgb("#4CAF50"); // Green
		canvas.FillCircle(startX, startY, markerRadius);

		// Draw end marker (red circle)
		var endX = offsetX + (maze.EndCell.Column * cellSize) + (cellSize / 2);
		var endY = offsetY + (maze.EndCell.Row * cellSize) + (cellSize / 2);

		canvas.FillColor = Color.FromArgb("#F44336"); // Red
		canvas.FillCircle(endX, endY, markerRadius);
	}

	/// <summary>
	/// Gets the minimum recommended size for rendering the maze.
	/// </summary>
	public SizeF GetMinimumSize(Maze maze)
	{
		float minWidth = maze.Columns * MinCellSize;
		float minHeight = maze.Rows * MinCellSize;
		return new SizeF(minWidth, minHeight);
	}
}
