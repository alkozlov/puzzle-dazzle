using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Rendering;

namespace PuzzleDazzle.Services;

/// <summary>
/// Service for exporting mazes as images.
/// </summary>
public class MazeExportService
{
	private readonly IMazeRenderer _renderer;
	private readonly IServiceProvider _serviceProvider;

	public MazeExportService(IMazeRenderer renderer, IServiceProvider serviceProvider)
	{
		_renderer = renderer;
		_serviceProvider = serviceProvider;
	}

	/// <summary>
	/// Captures a screenshot of a view and saves it.
	/// This is a simple approach that works reliably on Android.
	/// </summary>
	public async Task<byte[]> CaptureViewAsync(IView view)
	{
		var result = await view.CaptureAsync();
		if (result == null)
			throw new InvalidOperationException("Failed to capture view");

		using var stream = new MemoryStream();
		await result.CopyToAsync(stream, ScreenshotFormat.Png);
		return stream.ToArray();
	}

	/// <summary>
	/// Saves captured image bytes as a PNG file to the device.
	/// </summary>
	public async Task<string> SaveToFileAsync(byte[] imageBytes, string? fileName = null)
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

		// Save to file
		await File.WriteAllBytesAsync(filePath, imageBytes);

		return filePath;
	}

	/// <summary>
	/// Saves image bytes to temp file for sharing.
	/// </summary>
	public async Task<string> SaveToTempFileAsync(byte[] imageBytes)
	{
		var tempPath = Path.GetTempPath();
		var fileName = $"maze_share_{DateTime.Now:yyyyMMdd_HHmmss}.png";
		var filePath = Path.Combine(tempPath, fileName);

		await File.WriteAllBytesAsync(filePath, imageBytes);

		return filePath;
	}
}
