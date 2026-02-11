using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Rendering;

namespace PuzzleDazzle.Controls;

/// <summary>
/// Custom view for displaying a maze using MAUI Graphics.
/// </summary>
public class MazeView : GraphicsView
{
	private Maze? _maze;
	private readonly ClassicMazeRenderer _renderer;

	public MazeView()
	{
		_renderer = new ClassicMazeRenderer();
		Drawable = new MazeDrawable(_renderer);
	}

	/// <summary>
	/// Sets the maze to display.
	/// </summary>
	public void SetMaze(Maze maze)
	{
		_maze = maze;
		((MazeDrawable)Drawable).Maze = maze;
		Invalidate();
	}

	/// <summary>
	/// Drawable class that renders the maze.
	/// </summary>
	private class MazeDrawable : IDrawable
	{
		private readonly IMazeRenderer _renderer;
		public Maze? Maze { get; set; }

		public MazeDrawable(IMazeRenderer renderer)
		{
			_renderer = renderer;
		}

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			if (Maze == null)
				return;

			_renderer.Render(canvas, Maze, dirtyRect.Width, dirtyRect.Height);
		}
	}
}
