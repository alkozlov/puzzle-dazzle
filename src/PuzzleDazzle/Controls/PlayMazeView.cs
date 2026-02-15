using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Rendering;
using MazeCell = PuzzleDazzle.Core.Models.Cell;

namespace PuzzleDazzle.Controls;

/// <summary>
/// A non-interactive maze view used in play mode.
/// Renders the maze via <see cref="ClassicMazeRenderer"/>, then overlays:
///   • The active path as a light-blue polyline (dead-end trimming maintained by caller)
///   • The player position as a filled blue circle
/// Start (green) and end (red) markers are drawn by the base renderer.
/// </summary>
public class PlayMazeView : GraphicsView
{
	private readonly PlayMazeDrawable _drawable;

	public PlayMazeView()
	{
		_drawable = new PlayMazeDrawable(new ClassicMazeRenderer());
		Drawable = _drawable;
	}

	/// <summary>Assigns the maze and resets play state.</summary>
	public void SetMaze(Maze maze)
	{
		_drawable.Maze = maze;
		_drawable.PlayerCell = maze.StartCell;
		_drawable.Path = new List<MazeCell> { maze.StartCell };
		Invalidate();
	}

	/// <summary>Updates the player position and active path, then redraws.</summary>
	public void UpdatePlayerState(MazeCell playerCell, IReadOnlyList<MazeCell> path)
	{
		_drawable.PlayerCell = playerCell;
		_drawable.Path = path;
		Invalidate();
	}

	// ── Drawable ─────────────────────────────────────────────────────────────

	private class PlayMazeDrawable : IDrawable
	{
		private readonly ClassicMazeRenderer _renderer;

		public Maze? Maze { get; set; }
		public MazeCell? PlayerCell { get; set; }
		public IReadOnlyList<MazeCell>? Path { get; set; }

		// Path color: hue is light blue, alpha controls brightness independently.
		// Increase PathAlpha to make the path more visible (0.0 = invisible, 1.0 = fully opaque).
		private const float PathHueR = 0xAD / 255f; // #ADD8E6 — light blue
		private const float PathHueG = 0xD8 / 255f;
		private const float PathHueB = 0xE6 / 255f;
		private const float PathAlpha = 1.0f;        // doubled from 0.5 → fully opaque
		private static readonly Color PathColor = Color.FromRgba(PathHueR, PathHueG, PathHueB, PathAlpha);

		private static readonly Color PlayerColor = Color.FromArgb("#1565C0"); // dark blue

		public PlayMazeDrawable(ClassicMazeRenderer renderer)
		{
			_renderer = renderer;
		}

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			if (Maze == null) return;

			float width = dirtyRect.Width;
			float height = dirtyRect.Height;

			// 1. Draw the base maze (walls + start/end markers)
			_renderer.Render(canvas, Maze, width, height);

			// 2. Calculate layout metrics (must mirror ClassicMazeRenderer)
			float cellWidth = width / Maze.Columns;
			float cellHeight = height / Maze.Rows;
			float cellSize = Math.Min(cellWidth, cellHeight);
			float offsetX = (width - cellSize * Maze.Columns) / 2f;
			float offsetY = (height - cellSize * Maze.Rows) / 2f;

			// 3. Draw path polyline
			if (Path != null && Path.Count > 1)
			{
				canvas.StrokeColor = PathColor;
				canvas.StrokeSize = Math.Max(2f, cellSize * 0.18f);
				canvas.StrokeLineCap = LineCap.Round;
				canvas.StrokeLineJoin = LineJoin.Round;

				var pathF = new PathF();
				bool first = true;
				foreach (var cell in Path)
				{
					float cx = offsetX + cell.Column * cellSize + cellSize / 2f;
					float cy = offsetY + cell.Row * cellSize + cellSize / 2f;
					if (first) { pathF.MoveTo(cx, cy); first = false; }
					else pathF.LineTo(cx, cy);
				}
				canvas.DrawPath(pathF);
			}

			// 4. Draw player dot (on top of path)
			if (PlayerCell != null)
			{
				float px = offsetX + PlayerCell.Column * cellSize + cellSize / 2f;
				float py = offsetY + PlayerCell.Row * cellSize + cellSize / 2f;
				float playerRadius = cellSize * 0.28f;

				// White border for contrast
				canvas.FillColor = Colors.White;
				canvas.FillCircle(px, py, playerRadius + 2f);

				canvas.FillColor = PlayerColor;
				canvas.FillCircle(px, py, playerRadius);
			}
		}
	}
}
