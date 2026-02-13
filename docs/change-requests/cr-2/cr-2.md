# Improving the Maze's Graphic Visualization

The main goal of this change is to improve the maze's graphical visualization to make it clearer and more attractive to users. The current maze design is shown in the screenshot `./current_maze_design.jpg`. That design is very simple and may not engage the target audience (children and teenagers).

The proposed design is shown in the screenshot `./proposed_maze_design.png`. The new design uses color for the maze lines, slightly increases line weight, and makes junctions smoother. This makes the maze more attractive and easier to understand, especially for children.

## Key changes

1. **Line color**: In the new design, maze lines are colored. At this stage this parameter should be exposed somewhere in the code. The user will not be able to change the color. Ideally, use the line color from the example image shown in `./proposed_maze_design.png`.

2. **Line thickness**: Maze lines are slightly thicker, making them more noticeable and improving visual clarity. This parameter should also be exposed in code so it can be easily adjusted in the future if needed.

3. **Smooth junctions**: Connections between maze lines are made smoother in the new design, giving the maze a more modern and attractive appearance.

4. **Use of more screen space**: The new maze design should occupy more space on the screen. This will require changes or a redesign of the current UI element that renders the maze. The goal is to make the maze more prominent and appealing to users.

## Expected results
- Improved visual attractiveness of the maze, which may increase user engagement.
- Better usability, especially for children who are the primary target audience of the application.
