using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Rendering;
using SkiaSharp;

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
	/// Adds left and right margins to an image.
	/// </summary>
	private byte[] AddMarginsToImage(byte[] imageBytes, int marginPixels = 60)
	{
		using var originalBitmap = SKBitmap.Decode(imageBytes);
		if (originalBitmap == null)
			return imageBytes; // Return original if decoding fails

		// Create new bitmap with margins
		var newWidth = originalBitmap.Width + (marginPixels * 2);
		var newHeight = originalBitmap.Height;

		using var newBitmap = new SKBitmap(newWidth, newHeight);
		using var canvas = new SKCanvas(newBitmap);

		// Fill with white background
		canvas.Clear(SKColors.White);

		// Draw original image centered with margins
		canvas.DrawBitmap(originalBitmap, marginPixels, 0);

		// Encode to PNG
		using var image = SKImage.FromBitmap(newBitmap);
		using var data = image.Encode(SKEncodedImageFormat.Png, 100);
		return data.ToArray();
	}

	/// <summary>
	/// Saves captured image bytes as a PNG file to the device.
	/// </summary>
	public async Task<string> SaveToFileAsync(byte[] imageBytes, string? fileName = null)
	{
		// Add margins to the image
		var imageBytesWithMargins = AddMarginsToImage(imageBytes);

		// Generate filename if not provided
		if (string.IsNullOrEmpty(fileName))
		{
			fileName = $"maze_{DateTime.Now:yyyyMMdd_HHmmss}";
		}

		// Use Android's public Pictures directory via DCIM
		// This makes files visible in Gallery and accessible to users
		var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
			Android.OS.Environment.DirectoryPictures)?.AbsolutePath 
			?? "/storage/emulated/0/Pictures";
		
		var puzzleDazzlePath = Path.Combine(picturesPath, "PuzzleDazzle");
		
		// Create directory if it doesn't exist
		Directory.CreateDirectory(puzzleDazzlePath);

		// Full file path
		var filePath = Path.Combine(puzzleDazzlePath, $"{fileName}.png");

		// Save to file
		await File.WriteAllBytesAsync(filePath, imageBytesWithMargins);

		// Notify Android media scanner so the image appears in Gallery
		try
		{
			var context = Android.App.Application.Context;
			Android.Media.MediaScannerConnection.ScanFile(
				context, 
				new[] { filePath }, 
				new[] { "image/png" }, 
				null);
		}
		catch
		{
			// Ignore media scanner errors
		}

		return filePath;
	}

	/// <summary>
	/// Saves image bytes to temp file for sharing.
	/// </summary>
	public async Task<string> SaveToTempFileAsync(byte[] imageBytes)
	{
		// Add margins to the image
		var imageBytesWithMargins = AddMarginsToImage(imageBytes);

		var tempPath = Path.GetTempPath();
		var fileName = $"maze_share_{DateTime.Now:yyyyMMdd_HHmmss}.png";
		var filePath = Path.Combine(tempPath, fileName);

		await File.WriteAllBytesAsync(filePath, imageBytesWithMargins);

		return filePath;
	}
}
