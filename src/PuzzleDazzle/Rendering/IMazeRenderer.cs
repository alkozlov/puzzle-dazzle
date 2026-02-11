using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Rendering;

/// <summary>
/// Interface for maze renderers.
/// </summary>
public interface IMazeRenderer
{
	/// <summary>
	/// Renders a maze to a drawable canvas.
	/// </summary>
	/// <param name="canvas">The canvas to draw on.</param>
	/// <param name="maze">The maze to render.</param>
	/// <param name="width">Available width in pixels.</param>
	/// <param name="height">Available height in pixels.</param>
	void Render(ICanvas canvas, Maze maze, float width, float height);

	/// <summary>
	/// Gets the recommended minimum size for the given maze.
	/// </summary>
	SizeF GetMinimumSize(Maze maze);
}
