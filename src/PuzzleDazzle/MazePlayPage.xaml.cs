using PuzzleDazzle.Controls;
using PuzzleDazzle.Core.Models;
using MazeCell = PuzzleDazzle.Core.Models.Cell;

namespace PuzzleDazzle;

public partial class MazePlayPage : ContentPage
{
	private readonly Maze _maze;

	// ── Player state ──────────────────────────────────────────────────────────

	private MazeCell _playerCell;

	/// <summary>
	/// The active path from start to current position.
	/// Dead-end branches are trimmed on backtrack: if the player steps onto a
	/// cell that is already in the path, everything after that cell is removed.
	/// </summary>
	private readonly List<MazeCell> _path = new();

	// ── Timer ─────────────────────────────────────────────────────────────────

	private readonly IDispatcherTimer _timer;
	private TimeSpan _elapsed = TimeSpan.Zero;
	private bool _completed = false;

	public MazePlayPage(Maze maze)
	{
		_maze = maze;
		_playerCell = maze.StartCell;
		InitializeComponent();

		PlayMazeView.SetMaze(_maze);

		_timer = Dispatcher.CreateTimer();
		_timer.Interval = TimeSpan.FromSeconds(1);
		_timer.Tick += OnTimerTick;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		LockLandscape();
		ApplyBottomInset();
		ResetPlayState();
		_timer.Start();
	}

	/// <summary>
	/// Reads the system bottom inset (gesture nav bar height) and adds it as
	/// extra bottom padding on the completion content so the button is never hidden.
	/// </summary>
	private void ApplyBottomInset()
	{
#if ANDROID
		var activity = Platform.CurrentActivity;
		if (activity?.Window?.DecorView.RootWindowInsets is { } insets)
		{
			// API 30+: use WindowInsets.Type.systemBars()
			// API 28-29 fallback: use StableInsetBottom
			int bottomPx;
			if (OperatingSystem.IsAndroidVersionAtLeast(30))
			{
				var bars = insets.GetInsets(Android.Views.WindowInsets.Type.SystemBars());
				bottomPx = bars.Bottom;
			}
			else
			{
#pragma warning disable CA1416
				bottomPx = insets.StableInsetBottom;
#pragma warning restore CA1416
			}

			// Convert px → dp
			float density = activity.Resources?.DisplayMetrics?.Density ?? 1f;
			double bottomDp = bottomPx / density;

			// Apply as extra bottom padding on the completion content stack
			var existing = CompletionContent.Padding;
			CompletionContent.Padding = new Thickness(
				existing.Left, existing.Top, existing.Right,
				existing.Bottom + bottomDp + 16); // +16dp extra breathing room
		}
#endif
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_timer.Stop();
		RestoreOrientation();
	}

	// ── Play state ────────────────────────────────────────────────────────────

	private void ResetPlayState()
	{
		_completed = false;
		_elapsed = TimeSpan.Zero;
		TimerLabel.Text = "00:00";
		_playerCell = _maze.StartCell;
		_path.Clear();
		_path.Add(_playerCell);
		PlayMazeView.UpdatePlayerState(_playerCell, _path);

		// Ensure completion overlay is hidden
		EmojiAnimation.StopAnimation();
		CompletionOverlay.IsVisible = false;
	}

	// ── Timer ─────────────────────────────────────────────────────────────────

	private void OnTimerTick(object? sender, EventArgs e)
	{
		if (_completed) return;
		_elapsed = _elapsed.Add(TimeSpan.FromSeconds(1));
		TimerLabel.Text = _elapsed.ToString(@"mm\:ss");
	}

	// ── Navigation ────────────────────────────────────────────────────────────

	private async void OnExitClicked(object? sender, EventArgs e)
	{
		_timer.Stop();
		await Navigation.PopAsync();
	}

	// ── Joystick ──────────────────────────────────────────────────────────────

	private void OnJoystickDirectionTapped(object? sender, JoystickDirection direction)
	{
		if (_completed) return;
		TryMove(direction);
	}

	// ── Movement logic ────────────────────────────────────────────────────────

	/// <summary>
	/// Attempts to move the player one cell in <paramref name="direction"/>.
	/// Does nothing if the current cell has a wall on that side.
	/// </summary>
	private void TryMove(JoystickDirection direction)
	{
		// Check wall on current cell
		if (IsWalled(direction)) return;

		// Compute target cell coordinates
		int targetRow = _playerCell.Row;
		int targetCol = _playerCell.Column;

		switch (direction)
		{
			case JoystickDirection.Up:    targetRow--; break;
			case JoystickDirection.Down:  targetRow++; break;
			case JoystickDirection.Left:  targetCol--; break;
			case JoystickDirection.Right: targetCol++; break;
		}

		var targetCell = _maze.GetCell(targetRow, targetCol);
		if (targetCell == null || !targetCell.IsActive) return;

		// Move player
		_playerCell = targetCell;

		// Update path with dead-end trimming
		UpdatePath(_playerCell);

		// Refresh view
		PlayMazeView.UpdatePlayerState(_playerCell, _path);

		// Check completion
		if (_playerCell == _maze.EndCell)
			OnMazeCompleted();
	}

	/// <summary>Returns true if the current cell has a wall in the given direction.</summary>
	private bool IsWalled(JoystickDirection direction) => direction switch
	{
		JoystickDirection.Up    => _playerCell.TopWall,
		JoystickDirection.Down  => _playerCell.BottomWall,
		JoystickDirection.Left  => _playerCell.LeftWall,
		JoystickDirection.Right => _playerCell.RightWall,
		_                       => true
	};

	/// <summary>
	/// Appends <paramref name="cell"/> to the path, or trims the path back to
	/// that cell if it was already visited (backtrack / dead-end trimming).
	/// </summary>
	private void UpdatePath(MazeCell cell)
	{
		int existingIndex = _path.IndexOf(cell);
		if (existingIndex >= 0)
		{
			// Player stepped back onto a previously visited cell — trim the dead end
			_path.RemoveRange(existingIndex + 1, _path.Count - existingIndex - 1);
		}
		else
		{
			_path.Add(cell);
		}
	}

	// ── Completion ────────────────────────────────────────────────────────────

	private void OnMazeCompleted()
	{
		_completed = true;
		_timer.Stop();

		string timeText = _elapsed.ToString(@"mm\:ss");
		CompletionTimeLabel.Text = $"Your time: {timeText}";

		CompletionOverlay.IsVisible = true;
		EmojiAnimation.StartAnimation();
	}

	private async void OnContinueClicked(object? sender, EventArgs e)
	{
		EmojiAnimation.StopAnimation();
		CompletionOverlay.IsVisible = false;
		await Navigation.PopAsync();
	}

	// ── Orientation helpers ───────────────────────────────────────────────────

	private static void LockLandscape()
	{
#if ANDROID
		var activity = Platform.CurrentActivity;
		if (activity != null)
			activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape;
#endif
	}

	private static void RestoreOrientation()
	{
#if ANDROID
		var activity = Platform.CurrentActivity;
		if (activity != null)
			activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Unspecified;
#endif
	}
}
