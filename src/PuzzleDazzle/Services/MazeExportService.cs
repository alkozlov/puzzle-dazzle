using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Rendering;

namespace PuzzleDazzle.Services;

/// <summary>
/// Service for exporting mazes as images.
/// </summary>
public class MazeExportService
{
	private readonly IMazeRenderer _renderer;

	public MazeExportService(IMazeRenderer renderer)
	{
		_renderer = renderer;
	}

	/// <summary>
	/// Exports a maze to a PNG image.
	/// </summary>
	/// <param name="maze">The maze to export.</param>
	/// <param name="width">Image width in pixels.</param>
	/// <param name="height">Image height in pixels.</param>
	/// <returns>PNG image as byte array.</returns>
	public async Task<byte[]> ExportToPngAsync(Maze maze, int width = 2048, int height = 2048)
	{
		return await Task.Run(() =>
		{
			// Create a bitmap canvas
			var bitmap = new Microsoft.Maui.Graphics.Platform.PlatformImage(width, height);
			var canvas = bitmap.Canvas;

			// Render the maze to the canvas
			_renderer.Render(canvas, maze, width, height);

			// Convert to PNG bytes
			using var stream = new MemoryStream();
			bitmap.Save(stream);
			return stream.ToArray();
		});
	}

	/// <summary>
	/// Saves a maze as a PNG file to the device.
	/// </summary>
	/// <param name="maze">The maze to save.</param>
	/// <param name="fileName">Optional custom file name (without extension).</param>
	/// <returns>The full path to the saved file.</returns>
	public async Task<string> SaveToFileAsync(Maze maze, string? fileName = null)
	{
		// Generate filename if not provided
		if (string.IsNullOrEmpty(fileName))
		{
			fileName = $"maze_{DateTime.Now:yyyyMMdd_HHmmss}";
		}

		// Get the Pictures directory
		var picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
		var puzzleDazzlePath = Path.Combine(picturesPath, "PuzzleDazzle");
		
		// Create directory if it doesn't exist
		Directory.CreateDirectory(puzzleDazzlePath);

		// Full file path
		var filePath = Path.Combine(puzzleDazzlePath, $"{fileName}.png");

		// Export to PNG
		var pngBytes = await ExportToPngAsync(maze);

		// Save to file
		await File.WriteAllBytesAsync(filePath, pngBytes);

		return filePath;
	}

	/// <summary>
	/// Gets a temporary file path for sharing.
	/// </summary>
	public async Task<string> SaveToTempFileAsync(Maze maze)
	{
		var tempPath = Path.GetTempPath();
		var fileName = $"maze_share_{DateTime.Now:yyyyMMdd_HHmmss}.png";
		var filePath = Path.Combine(tempPath, fileName);

		var pngBytes = await ExportToPngAsync(maze);
		await File.WriteAllBytesAsync(filePath, pngBytes);

		return filePath;
	}
}
