using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Rendering;

namespace PuzzleDazzle.Controls;

/// <summary>
/// Custom view for displaying a maze using MAUI Graphics.
/// Supports pinch-to-zoom and pan gestures via canvas transforms.
/// </summary>
public class MazeView : GraphicsView
{
	// Zoom bounds
	private const float MinScale = 1.0f;
	private const float MaxScale = 4.0f;

	// Current transform state
	private float _scale = 1.0f;
	private float _translateX = 0f;
	private float _translateY = 0f;

	// Pan tracking
	private float _panStartTx = 0f;
	private float _panStartTy = 0f;

	private readonly MazeDrawable _drawable;

	public MazeView()
	{
		_drawable = new MazeDrawable(new ClassicMazeRenderer());
		Drawable = _drawable;

		// Pinch-to-zoom
		var pinch = new PinchGestureRecognizer();
		pinch.PinchUpdated += OnPinchUpdated;
		GestureRecognizers.Add(pinch);

		// Pan (drag to scroll when zoomed in)
		var pan = new PanGestureRecognizer();
		pan.PanUpdated += OnPanUpdated;
		GestureRecognizers.Add(pan);
	}

	/// <summary>
	/// Sets the maze to display and resets zoom/pan.
	/// </summary>
	public void SetMaze(Maze maze)
	{
		_drawable.Maze = maze;
		ResetTransform();
	}

	private void ResetTransform()
	{
		_scale = 1.0f;
		_translateX = 0f;
		_translateY = 0f;
		ApplyTransform();
	}

	private void ApplyTransform()
	{
		_drawable.Scale = _scale;
		_drawable.TranslateX = _translateX;
		_drawable.TranslateY = _translateY;
		Invalidate();
	}

	/// <summary>
	/// Clamps translation so the scaled content doesn't scroll fully off-screen.
	/// </summary>
	private void ClampTranslation()
	{
		float maxTx = (float)(Width * (_scale - 1)) / 2f;
		float maxTy = (float)(Height * (_scale - 1)) / 2f;
		_translateX = Math.Clamp(_translateX, -maxTx, maxTx);
		_translateY = Math.Clamp(_translateY, -maxTy, maxTy);
	}

	private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
	{
		// e.Scale is a per-frame delta ratio (e.g. 1.02 = "2% larger than last frame").
		// Multiply against the current scale each frame to accumulate smoothly.
		// Do NOT multiply against a "start scale" — that resets to start on every event
		// and causes the image to jerk wildly.
		if (e.Status != GestureStatus.Running)
			return;

		float newScale = Math.Clamp(_scale * (float)e.Scale, MinScale, MaxScale);

		if (newScale <= MinScale)
		{
			_scale = MinScale;
			_translateX = 0f;
			_translateY = 0f;
		}
		else
		{
			// Zoom towards the pinch centre so the content under the fingers stays fixed.
			// ScaleOrigin is in [0,1] relative to the view size.
			float originX = (float)(e.ScaleOrigin.X * Width);
			float originY = (float)(e.ScaleOrigin.Y * Height);

			float ratio = newScale / _scale;
			_translateX = originX + (_translateX - originX) * ratio;
			_translateY = originY + (_translateY - originY) * ratio;

			_scale = newScale;
			ClampTranslation();
		}

		ApplyTransform();
	}

	private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
	{
		// Only allow panning when zoomed in
		if (_scale <= MinScale)
			return;

		switch (e.StatusType)
		{
			case GestureStatus.Started:
				_panStartTx = _translateX;
				_panStartTy = _translateY;
				break;

			case GestureStatus.Running:
				_translateX = _panStartTx + (float)e.TotalX;
				_translateY = _panStartTy + (float)e.TotalY;
				ClampTranslation();
				ApplyTransform();
				break;
		}
	}

	/// <summary>
	/// Drawable that renders the maze with a zoom/pan canvas transform.
	/// </summary>
	private class MazeDrawable : IDrawable
	{
		private readonly IMazeRenderer _renderer;

		public Maze? Maze { get; set; }
		public float Scale { get; set; } = 1.0f;
		public float TranslateX { get; set; } = 0f;
		public float TranslateY { get; set; } = 0f;

		public MazeDrawable(IMazeRenderer renderer)
		{
			_renderer = renderer;
		}

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			if (Maze == null)
				return;

			canvas.SaveState();

			// Apply zoom centred on the canvas, then offset by pan translation
			float cx = dirtyRect.Width / 2f;
			float cy = dirtyRect.Height / 2f;
			canvas.Translate(cx + TranslateX, cy + TranslateY);
			canvas.Scale(Scale, Scale);
			canvas.Translate(-cx, -cy);

			_renderer.Render(canvas, Maze, dirtyRect.Width, dirtyRect.Height);

			canvas.RestoreState();
		}
	}
}
