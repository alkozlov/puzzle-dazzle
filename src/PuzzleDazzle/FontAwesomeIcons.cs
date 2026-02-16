namespace PuzzleDazzle;

/// <summary>
/// Font Awesome Solid icons constants for use in XAML and code.
/// Icons from Font Awesome 6.x Free Solid
/// Usage: Text="{x:Static local:FontAwesomeIcons.Gear}" FontFamily="FontAwesome"
/// </summary>
public static class FontAwesomeIcons
{
	// Settings & Configuration
	public const string Gear = "\uf013";           // Settings icon (⚙️ replacement)
	public const string Cog = "\uf013";            // Same as Gear
	public const string Sliders = "\uf1de";        // Alternative settings
	
	// Save & Storage
	public const string FloppyDisk = "\uf0c7";     // Save icon (💾 replacement)
	public const string Download = "\uf019";       // Download
	
	// Share & Social
	public const string ShareNodes = "\uf1e0";     // Share icon (📤 replacement)
	public const string ArrowUpFromBracket = "\ue09a"; // Share/Upload alternative
	
	// Play & Generate
	public const string Play = "\uf04e";           // Play icon (▶️ replacement)
	public const string CirclePlay = "\uf144";     // Play in circle
	public const string Rotate = "\uf2f1";         // Refresh/regenerate
	public const string Wand = "\uf0d0";           // Magic wand/generate
	
	// Navigation
	public const string House = "\uf015";          // Home
	public const string ArrowLeft = "\uf060";      // Back arrow
	public const string Bars = "\uf0c9";           // Menu
	public const string Xmark = "\uf00d";          // Close
	
	// Common Actions
	public const string Check = "\uf00c";          // Checkmark
	public const string Plus = "\u002b";           // Add/Plus
	public const string Trash = "\uf1f8";          // Delete
	public const string Pen = "\uf304";            // Edit
	
	// Status & Feedback
	public const string CircleExclamation = "\uf06a"; // Alert/Warning
	public const string CircleCheck = "\uf058";    // Success
	public const string CircleInfo = "\uf05a";     // Info
	
	// Maze & Game Related
	public const string PuzzlePiece = "\uf12e";    // Puzzle piece
	public const string TableCells = "\uf00a";     // Grid
	public const string Star = "\uf005";           // Star (for favorites)
	public const string DoorOpen = "\uf52b";       // Door open (enter play mode)

	// Maze Size Toggle Buttons
	public const string SizeSmall = "\uf78c";      // Compress arrows (small)
	public const string SizeMedium = "\uf0c8";     // Square (medium)
	public const string SizeLarge = "\uf065";      // Expand arrows (large)

	// Maze Difficulty Toggle Buttons (face icons, kept for potential reuse)
	public const string FaceSmile = "\uf118";      // Smiling face
	public const string FaceMeh = "\uf11a";        // Neutral face
	public const string FaceFrown = "\uf119";      // Frowning face

	// Maze Difficulty Toggle Buttons (star rating)
	public const string DifficultyEasy = "\uf005";               // 1 star
	public const string DifficultyMedium = "\uf005 \uf005";      // 2 stars
	public const string DifficultyHard = "\uf005 \uf005 \uf005"; // 3 stars

	// Maze Shape Toggle Buttons
	public const string ShapeSquare = "\uf0c8";    // Square (rectangle)
	public const string ShapeCircle = "\uf111";    // Circle
	public const string ShapeDiamond = "\uf219";   // Diamond
	public const string ShapeHeart = "\uf004";     // Heart
}
