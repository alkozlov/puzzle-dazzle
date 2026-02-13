using PuzzleDazzle.Core.Models;
using PuzzleDazzle.Rendering;

namespace PuzzleDazzle.Controls;

/// <summary>
/// Custom view for displaying a maze using MAUI Graphics.
/// Supports pinch-to-zoom and pan gestures via canvas transforms.
///
/// Transform model
/// ---------------
/// We maintain a single (offsetX, offsetY, scale) triple that maps content
/// coordinates to screen coordinates:
///
///   screenPoint = contentPoint * scale + (offsetX, offsetY)
///
/// The canvas transform in Draw() is therefore:
///   canvas.Translate(offsetX, offsetY)
///   canvas.Scale(scale, scale)
///
/// Zooming around a pivot P (screen coords):
///   new_offset = P - (P - offset) * (newScale / oldScale)
///
/// This keeps the pixel at P fixed on screen while scale changes.
/// </summary>
public class MazeView : GraphicsView
{
	private const float MinScale = 1.0f;
	private const float MaxScale = 4.0f;

	// Transform: content → screen
	private float _scale = 1.0f;
	private float _offsetX = 0f;   // screen translation (canvas origin offset)
	private float _offsetY = 0f;

	// Pan tracking
	private float _panStartOffsetX = 0f;
	private float _panStartOffsetY = 0f;

	private readonly MazeDrawable _drawable;

	public MazeView()
	{
		_drawable = new MazeDrawable(new ClassicMazeRenderer());
		Drawable = _drawable;

		var pinch = new PinchGestureRecognizer();
		pinch.PinchUpdated += OnPinchUpdated;
		GestureRecognizers.Add(pinch);

		var pan = new PanGestureRecognizer();
		pan.PanUpdated += OnPanUpdated;
		GestureRecognizers.Add(pan);
	}

	public void SetMaze(Maze maze)
	{
		_drawable.Maze = maze;
		ResetTransform();
	}

	private void ResetTransform()
	{
		_scale = 1.0f;
		_offsetX = 0f;
		_offsetY = 0f;
		PushTransform();
	}

	private void PushTransform()
	{
		_drawable.Scale = _scale;
		_drawable.OffsetX = _offsetX;
		_drawable.OffsetY = _offsetY;
		Invalidate();
	}

	/// <summary>
	/// Clamps the offset so content cannot be panned fully off-screen.
	/// At scale S the content is S times wider/taller than the view.
	/// The offset may range from -(S-1)*W to 0 on each axis.
	/// </summary>
	private void ClampOffset()
	{
		float w = (float)Width;
		float h = (float)Height;

		float minOx = -((_scale - 1f) * w);
		float minOy = -((_scale - 1f) * h);

		_offsetX = Math.Clamp(_offsetX, minOx, 0f);
		_offsetY = Math.Clamp(_offsetY, minOy, 0f);
	}

	private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
	{
		if (e.Status != GestureStatus.Running)
			return;

		float delta = (float)e.Scale;
		if (delta <= 0f)
			return;

		float newScale = Math.Clamp(_scale * delta, MinScale, MaxScale);

		// Pivot point in screen (view) coordinates
		float pivotX = (float)(e.ScaleOrigin.X * Width);
		float pivotY = (float)(e.ScaleOrigin.Y * Height);

		// Keep the pivot pixel fixed:
		//   newOffset = pivot - (pivot - oldOffset) * (newScale / oldScale)
		float ratio = newScale / _scale;
		_offsetX = pivotX - (pivotX - _offsetX) * ratio;
		_offsetY = pivotY - (pivotY - _offsetY) * ratio;

		_scale = newScale;

		if (_scale <= MinScale)
		{
			_scale = MinScale;
			_offsetX = 0f;
			_offsetY = 0f;
		}
		else
		{
			ClampOffset();
		}

		PushTransform();
	}

	private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
	{
		if (_scale <= MinScale)
			return;

		switch (e.StatusType)
		{
			case GestureStatus.Started:
				_panStartOffsetX = _offsetX;
				_panStartOffsetY = _offsetY;
				break;

			case GestureStatus.Running:
				_offsetX = _panStartOffsetX + (float)e.TotalX;
				_offsetY = _panStartOffsetY + (float)e.TotalY;
				ClampOffset();
				PushTransform();
				break;
		}
	}

	private class MazeDrawable : IDrawable
	{
		private readonly IMazeRenderer _renderer;

		public Maze? Maze { get; set; }
		public float Scale { get; set; } = 1.0f;
		public float OffsetX { get; set; } = 0f;
		public float OffsetY { get; set; } = 0f;

		public MazeDrawable(IMazeRenderer renderer)
		{
			_renderer = renderer;
		}

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			if (Maze == null)
				return;

			canvas.SaveState();

			// Simple affine: translate then scale.
			// Content point P maps to screen point P*Scale + (OffsetX, OffsetY).
			canvas.Translate(OffsetX, OffsetY);
			canvas.Scale(Scale, Scale);

			_renderer.Render(canvas, Maze, dirtyRect.Width, dirtyRect.Height);

			canvas.RestoreState();
		}
	}
}
