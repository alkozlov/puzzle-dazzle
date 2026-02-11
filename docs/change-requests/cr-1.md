# Simplifying the Interface

This change request aims to simplify the app interface by reducing the number of pages to two.

## Current structure

- Main screen
- Settings screen
- Maze generation screen

## Proposed changes

- The **Maze generation screen** will become the main page. On first launch the user will land on this page; however, a maze will not be generated automatically. The user must press the dedicated button to start maze generation. This ensures the user immediately sees the result when they choose to generate a maze and avoids distracting them with unnecessary automatic actions.

- The **Settings screen** remains unchanged.

This preserves the app's functionality while simplifying navigation and reducing the number of steps required from the user. The change should make the app more intuitive and accessible to a broader audience.

## Toolbar

The new main page will include a toolbar (or similar control) at the bottom of the screen containing the following buttons:

1. **Start** — begins maze generation.
2. **Settings** — navigates to the Settings page.
3. **Save** — saves the generated maze (enabled only after generation).
4. **Print** — prints the generated maze (enabled only after generation).

All buttons should use icons for quick recognition and include text labels for clarity. The **Start** button will be centered in the toolbar as the primary action; **Settings**, **Save**, and **Print** will be placed to the sides for easy access. Each press of **Start** creates a new maze. **Save** and **Print** remain inactive until a maze has been generated to avoid confusion and to maintain a logical flow of actions.

The **Settings** page contains only a **Back** button to return to the maze generation main page.
