## Application Screens

1. Main screen
2. Settings screen
3. Maze generation screen

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
