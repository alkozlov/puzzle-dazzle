# Main Changes

The goal of this change is to add the ability to play through a created maze. This will allow users not only to generate mazes but also to experience them interactively, increasing engagement and satisfaction with the app.

## Functional changes

1. **Add a maze play mode**: The app UI will include a new option that lets users switch into maze play mode. In this mode users can control the maze entry point and try to find the exit using control keys or touch gestures. This option is available after a maze is generated and will be shown in the lower-right corner of the UI element that renders the maze. The button will show a door icon to indicate entry into the maze. The button is rendered as a floating overlay on top of the maze graphic.

2. **Maze play screen**: When the entry button is pressed, users will be taken to a new screen (`MazePlayPage`) that displays the maze at a larger scale. This screen is locked to landscape orientation. The maze should occupy the majority of the screen to provide comfortable navigation. A circular joystick control for movement will be displayed in the lower-right corner; the circle is divided into four directional tap zones (up, down, left, right). An **Exit** button is shown in the top-right corner. A running timer is displayed on screen, starting when the page opens.

3. **Movement through the maze**: Users move using the joystick. Movement is step-by-step — each tap on a directional zone moves the player exactly one cell in that direction if no wall blocks the path; if a wall is present, the tap has no effect. Movement starts at the entry point (`maze.StartCell`), represented by a green dot. The objective is to reach the exit point (`maze.EndCell`), represented by a red dot. Entry and exit points are determined automatically by the maze generation algorithm: the entry is the first active cell (top-left scan) and the exit is the last active cell (bottom-right scan). Upon reaching the exit the user receives a notification showing the time taken; dismissing the notification returns the user to the maze creation screen.

4. **Forced exit from play mode**: If a user wants to leave play mode, they can press the **Exit** button in the top-right corner of the screen. This returns the user to the maze creation screen so they can continue editing or generate a new maze. Leaving play mode discards all progress in the current run; the next time the user enters play mode they will start a new session.

## Control details

**Joystick**: The joystick is a circular control divided into four directional tap zones (up, down, left, right). It is placed in the lower-right corner of the screen and sized to be easy to operate on mobile devices. Each tap immediately moves the player one cell in the chosen direction if the path is clear. If there is a wall in the chosen direction, the tap has no effect and the green dot remains in place.

**Path tracing**: During play, users will see their current path drawn as a line connecting the cells they have visited. The path line is displayed as an overlay on the maze in a subtle color (light gray or light blue) so it remains visible without distracting from the main maze design. When the user backtracks (moves to a previously visited cell), dead-end branches are trimmed from the path, so only the current active route from start to player position is shown.

## Clarified decisions

- **Entry/exit points**: Determined by the existing maze generation algorithm. No user configuration needed.
- **Orientation**: The play screen forces landscape orientation; orientation is restored when leaving the screen.
- **Movement style**: Step-by-step (one cell per tap). No continuous movement when holding.
- **Joystick visual style**: Circular control with four directional tap zones.
- **Play button placement**: Floating overlay on the `MazeView` in the lower-right corner.
- **Post-completion behavior**: Alert dismissed → return to maze creation screen.
- **Backtrack behavior**: Dead-end branches are trimmed; only the active path from start to current position is shown.
