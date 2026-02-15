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
/// Circular joystick control with four angle-based directional sectors and a central dead zone.
///
/// Sector boundaries (clockwise from top, 0° = 12 o'clock):
///   Up    : 315° – 360° and 0° – 45°
///   Right :  45° – 135°
///   Down  : 135° – 225°
///   Left  : 225° – 315°
///
/// A small filled circle is drawn at the center to indicate the dead zone.
/// Taps inside the dead zone or outside the outer circle are ignored.
/// </summary>
public class JoystickView : GraphicsView
{
	/// <summary>
	/// Raised when the user taps one of the four directional zones.
	/// </summary>
	public event EventHandler<JoystickDirection>? DirectionTapped;

	// Dead zone radius as a fraction of the joystick radius
	private const double DeadZoneFraction = 0.22;

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
	/// Maps a tap position to a direction using angle-based sector detection.
	/// Angle is measured clockwise from the top (12 o'clock = 0°).
	/// </summary>
	private JoystickDirection? HitTest(Point point)
	{
		double cx = Width / 2.0;
		double cy = Height / 2.0;
		double radius = Math.Min(cx, cy);
		double deadRadius = radius * DeadZoneFraction;

		double dx = point.X - cx;
		double dy = point.Y - cy;
		double distSq = dx * dx + dy * dy;

		// Ignore taps outside the outer circle
		if (distSq > radius * radius)
			return null;

		// Ignore taps inside the dead zone
		if (distSq <= deadRadius * deadRadius)
			return null;

		// atan2 gives angle from positive-X axis, counter-clockwise, in radians.
		// Convert to clockwise-from-top degrees:
		//   clockwiseDeg = (90 - atan2Deg + 360) % 360
		double atan2Deg = Math.Atan2(dy, dx) * (180.0 / Math.PI);
		double angle = (90.0 - atan2Deg + 360.0) % 360.0;

		// Map angle to direction
		if (angle < 45.0 || angle >= 315.0)
			return JoystickDirection.Up;
		if (angle < 135.0)
			return JoystickDirection.Right;
		if (angle < 225.0)
			return JoystickDirection.Down;
		return JoystickDirection.Left;
	}

	// ── Drawable ─────────────────────────────────────────────────────────────

	private class JoystickDrawable : IDrawable
	{
		public void Draw(ICanvas canvas, RectF rect)
		{
			float cx = rect.Width / 2f;
			float cy = rect.Height / 2f;
			float radius = Math.Min(cx, cy) - 4f;
			float deadRadius = radius * (float)DeadZoneFraction;

			// Outer circle background
			canvas.FillColor = Color.FromArgb("#CC333333");
			canvas.FillCircle(cx, cy, radius);

			// Sector dividing lines at 45°, 135°, 225°, 315° (clockwise from top)
			// In screen coords (y-down): these are at diagonal angles
			canvas.StrokeColor = Color.FromArgb("#55FFFFFF");
			canvas.StrokeSize = 1.5f;

			// 45° CW from top  → screen angle 45° from positive-X (upper-right diagonal)
			// 135° CW from top → screen angle -45° (lower-right diagonal)
			// etc.  Use unit vectors rotated from top:
			//   screen_dx =  sin(angleDeg * π/180)
			//   screen_dy = -cos(angleDeg * π/180)
			DrawSectorLine(canvas, cx, cy, radius, 45f);
			DrawSectorLine(canvas, cx, cy, radius, 135f);
			DrawSectorLine(canvas, cx, cy, radius, 225f);
			DrawSectorLine(canvas, cx, cy, radius, 315f);

			// Direction arrow labels
			canvas.StrokeColor = Colors.White;
			canvas.StrokeSize = 2.5f;
			canvas.StrokeLineCap = LineCap.Round;
			canvas.StrokeLineJoin = LineJoin.Round;

			float arrowOffset = radius * 0.62f;
			float arrowSize = radius * 0.18f;

			DrawArrow(canvas, cx, cy - arrowOffset, 0, -arrowSize); // Up
			DrawArrow(canvas, cx, cy + arrowOffset, 0,  arrowSize); // Down
			DrawArrow(canvas, cx - arrowOffset, cy, -arrowSize, 0); // Left
			DrawArrow(canvas, cx + arrowOffset, cy,  arrowSize, 0); // Right

			// Dead zone circle (filled, slightly lighter to stand out)
			canvas.FillColor = Color.FromArgb("#66FFFFFF");
			canvas.FillCircle(cx, cy, deadRadius);
		}

		/// <summary>
		/// Draws a full-diameter line through the center at the given clockwise-from-top angle.
		/// </summary>
		private static void DrawSectorLine(ICanvas canvas, float cx, float cy, float radius, float angleDeg)
		{
			float rad = angleDeg * MathF.PI / 180f;
			float sdx = MathF.Sin(rad);
			float sdy = -MathF.Cos(rad);

			canvas.DrawLine(
				cx - sdx * radius, cy - sdy * radius,
				cx + sdx * radius, cy + sdy * radius);
		}

		/// <summary>
		/// Draws a chevron arrow with its tip at (tipX, tipY) pointing in direction (dx, dy).
		/// </summary>
		private static void DrawArrow(ICanvas canvas, float tipX, float tipY, float dx, float dy)
		{
			float px = -dy;
			float py =  dx;

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
