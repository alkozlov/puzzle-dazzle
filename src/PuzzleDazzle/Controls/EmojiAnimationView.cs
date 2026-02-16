namespace PuzzleDazzle.Controls;

/// <summary>
/// Animates a large emoji with a looping bounce (scale pulse) effect.
/// Uses a dispatcher timer to drive frame invalidation at ~60 fps.
/// The emoji scales between <see cref="MinScale"/> and <see cref="MaxScale"/>
/// following a sine wave, giving a smooth heartbeat-like bounce.
/// </summary>
public class EmojiAnimationView : GraphicsView
{
	private const string Emoji = "🐸";
	private const double MinScale = 0.75;
	private const double MaxScale = 1.15;
	private const double CycleDurationSeconds = 0.9; // one full bounce period

	private readonly EmojiDrawable _drawable;
	private IDispatcherTimer? _timer;
	private DateTime _startTime;

	public EmojiAnimationView()
	{
		_drawable = new EmojiDrawable();
		Drawable = _drawable;
	}

	/// <summary>Starts the animation loop.</summary>
	public void StartAnimation()
	{
		_startTime = DateTime.UtcNow;
		_timer = Dispatcher.CreateTimer();
		_timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 fps
		_timer.Tick += OnTick;
		_timer.Start();
	}

	/// <summary>Stops the animation loop.</summary>
	public void StopAnimation()
	{
		_timer?.Stop();
		_timer = null;
	}

	private void OnTick(object? sender, EventArgs e)
	{
		double elapsed = (DateTime.UtcNow - _startTime).TotalSeconds;
		// Sine wave from 0→1→0, period = CycleDurationSeconds
		double t = (Math.Sin(elapsed * 2 * Math.PI / CycleDurationSeconds) + 1.0) / 2.0;
		_drawable.Scale = MinScale + t * (MaxScale - MinScale);
		Invalidate();
	}

	// ── Drawable ─────────────────────────────────────────────────────────────

	private class EmojiDrawable : IDrawable
	{
		public double Scale { get; set; } = 1.0;

		public void Draw(ICanvas canvas, RectF rect)
		{
			float cx = rect.Width / 2f;
			float cy = rect.Height / 2f;

			// Font size scaled by the animation scale
			float fontSize = (float)(rect.Height * 0.55 * Scale);

			canvas.FontSize = fontSize;
			canvas.FontColor = Colors.White; // fallback for non-emoji renderers
			canvas.DrawString(
				Emoji,
				cx - fontSize / 2f,
				cy - fontSize / 2f,
				fontSize,
				fontSize,
				HorizontalAlignment.Center,
				VerticalAlignment.Center);
		}
	}
}
