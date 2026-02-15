namespace PuzzleDazzle.Controls;

/// <summary>
/// Direction values emitted by JoystickView when the user taps a zone.
/// </summary>
public enum JoystickDirection
{
	Up,
	Down,
	Left,
	Right
}

/// <summary>
/// Circular joystick control divided into four directional tap zones.
/// Each tap raises <see cref="DirectionTapped"/> with the corresponding direction.
/// Taps outside the inscribed circle or within the 10% dead-center zone are ignored.
/// </summary>
public class JoystickView : GraphicsView
{
	/// <summary>
	/// Raised when the user taps one of the four directional zones.
	/// </summary>
	public event EventHandler<JoystickDirection>? DirectionTapped;

	private readonly JoystickDrawable _drawable;

	public JoystickView()
	{
		_drawable = new JoystickDrawable();
		Drawable = _drawable;

		var tap = new TapGestureRecognizer();
		tap.Tapped += OnTapped;
		GestureRecognizers.Add(tap);
	}

	private void OnTapped(object? sender, TappedEventArgs e)
	{
		var point = e.GetPosition(this);
		if (point == null) return;

		var direction = HitTest(point.Value);
		if (direction.HasValue)
			DirectionTapped?.Invoke(this, direction.Value);
	}

	/// <summary>
	/// Determines which directional zone the tap hit.
	/// The circle is divided into four 90° sectors by the diagonals:
	///   Up    = top sector    (|dy| > |dx|, dy &lt; 0)
	///   Down  = bottom sector (|dy| > |dx|, dy > 0)
	///   Left  = left sector   (|dx| > |dy|, dx &lt; 0)
	///   Right = right sector  (|dx| > |dy|, dx > 0)
	/// Taps outside the inscribed circle are ignored.
	/// </summary>
	private JoystickDirection? HitTest(Point point)
	{
		double cx = Width / 2.0;
		double cy = Height / 2.0;
		double radius = Math.Min(cx, cy);

		double dx = point.X - cx;
		double dy = point.Y - cy;

		// Ignore taps outside the circle
		if (dx * dx + dy * dy > radius * radius)
			return null;

		// Ignore dead-center taps (within 10% of radius)
		if (Math.Abs(dx) < radius * 0.1 && Math.Abs(dy) < radius * 0.1)
			return null;

		if (Math.Abs(dy) >= Math.Abs(dx))
			return dy < 0 ? JoystickDirection.Up : JoystickDirection.Down;
		else
			return dx < 0 ? JoystickDirection.Left : JoystickDirection.Right;
	}

	// ── Drawable ─────────────────────────────────────────────────────────────

	private class JoystickDrawable : IDrawable
	{
		public void Draw(ICanvas canvas, RectF rect)
		{
			float cx = rect.Width / 2f;
			float cy = rect.Height / 2f;
			float radius = Math.Min(cx, cy) - 4f;

			// Outer circle background
			canvas.FillColor = Color.FromArgb("#CC333333");
			canvas.FillCircle(cx, cy, radius);

			// Dividing lines (cross)
			canvas.StrokeColor = Color.FromArgb("#66FFFFFF");
			canvas.StrokeSize = 1.5f;
			canvas.DrawLine(cx, cy - radius, cx, cy + radius);
			canvas.DrawLine(cx - radius, cy, cx + radius, cy);

			// Arrow indicators
			canvas.StrokeColor = Colors.White;
			canvas.StrokeSize = 2.5f;
			canvas.StrokeLineCap = LineCap.Round;
			canvas.StrokeLineJoin = LineJoin.Round;

			float arrowOffset = radius * 0.55f;
			float arrowSize = radius * 0.18f;

			// Up arrow
			DrawArrow(canvas, cx, cy - arrowOffset, 0, -arrowSize);
			// Down arrow
			DrawArrow(canvas, cx, cy + arrowOffset, 0, arrowSize);
			// Left arrow
			DrawArrow(canvas, cx - arrowOffset, cy, -arrowSize, 0);
			// Right arrow
			DrawArrow(canvas, cx + arrowOffset, cy, arrowSize, 0);
		}

		/// <summary>
		/// Draws a simple chevron arrow at (tipX, tipY) pointing in direction (dx, dy).
		/// </summary>
		private static void DrawArrow(ICanvas canvas, float tipX, float tipY, float dx, float dy)
		{
			// Perpendicular direction
			float px = -dy;
			float py = dx;

			float tailX = tipX - dx;
			float tailY = tipY - dy;

			var path = new PathF();
			path.MoveTo(tailX + px, tailY + py);
			path.LineTo(tipX, tipY);
			path.LineTo(tailX - px, tailY - py);
			canvas.DrawPath(path);
		}
	}
}
