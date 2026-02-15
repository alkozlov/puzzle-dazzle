## Application Screens

1. Main screen
2. Settings screen
3. Maze generation screen
4. Maze play screen

### Main screen

On the main screen the user can go to the app settings or start maze generation. Most likely this screen will have a "Start" button that takes the user to the maze generation screen, and a "Settings" button that opens the settings screen. When the user taps "Start", they are taken to the generation screen and the maze generation process begins immediately.

### Settings screen

On the settings screen the user can configure generation parameters such as the maze size and difficulty. The user can also choose the maze visualization style. At the moment we need 2–3 visual styles (no more, to avoid complicating the UI).

1. Classic visualization — a black-and-white style where maze walls are thin black lines and the paths are white cells. This is the classic maze look many people expect.

2. Soft visualization — a gentler, more pleasing style where walls can be light gray or pastel colors and paths can be white or light shades. This style may appeal to users who prefer a modern, aesthetic design.

Choosing a visualization is best implemented as a tile/grid showing an image of each style so users can quickly preview how the maze will look.

### Maze generation screen

When the user arrives on this screen, the maze generation starts immediately. The user sees a progress bar that displays while the maze is being generated. This progress bar does not need to be tied to the actual generation algorithm — it simply indicates that work is in progress. After the maze is generated, it is displayed on the screen.

This screen also includes a "Back" button to return to the main screen. There should be a "Save" button allowing the user to save the generated maze as an image file so they can share or reuse it later. When the user taps "Save", they choose where to store the image on their device (other formats are not required yet to keep the UI simple).

Optionally, include a "Retry" button to regenerate a new maze with the same parameters. This is useful for users who want to explore different maze variants without changing the settings.

After a maze is generated, a floating play button (door icon) appears in the lower-right corner of the maze view. Tapping it enters the maze play screen.

### Maze play screen

The maze play screen allows the user to navigate through the generated maze interactively. It is displayed in landscape orientation (forced) and is accessed from the maze generation screen after a maze has been generated.

**Layout:**
- The maze fills the majority of the screen.
- A timer is displayed in the top-left corner, showing elapsed time in `mm:ss` format. It starts counting when the screen opens.
- An **Exit** button is displayed in the top-right corner. Tapping it discards the current play session and returns the user to the maze generation screen.
- A circular joystick control is displayed in the lower-right corner.

**Joystick:**
The joystick is a circle divided into four directional tap zones: up, down, left, and right. Each tap moves the player one cell in the chosen direction if no wall blocks the path. Taps outside the circle or at the dead center are ignored.

**Player and markers:**
- The player is represented by a dark blue dot with a white border, starting at the maze entry point (top-left active cell).
- The entry point is marked with a green dot.
- The exit point is marked with a red dot (bottom-right active cell).

**Path tracing:**
As the player moves, a light-blue semi-transparent line is drawn connecting all visited cells from the entry to the current position. If the player backtracks, dead-end branches are trimmed so only the active route from start to current position is shown.

**Completion:**
When the player reaches the exit (red dot), a notification is shown displaying the time taken to solve the maze. Dismissing the notification returns the user to the maze generation screen.

**Leaving play mode:**
Tapping the Exit button at any time stops the timer, discards all progress, and returns to the maze generation screen. The next time play mode is entered, a fresh session starts.
