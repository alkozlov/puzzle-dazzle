# CR-4 Concept

At the moment maze generation settings are configured on the `Settings` page. The user can change three categories: maze size, difficulty, and shape. All three settings are currently presented as dropdown menus. Under CR-4 we propose to change the `Settings` page UI and replace the dropdowns with sets of toggle-buttons. This will simplify the configuration process (one tap instead of two) and make it more intuitive for users.

## Maze size

The size dropdown will be replaced by three toggle-buttons, one for each available size: Small, Medium, and Large. Only one button can be selected at a time; the selected button should be visually highlighted. Prefer using icons instead of text on these buttons so the meaning is immediately clear to children. For example: a small square icon for Small, a medium square for Medium, and a large square for Large. Choose child-friendly icons that clearly communicate each option.

## Maze difficulty

The difficulty dropdown will be replaced by three toggle-buttons for Easy, Medium, and Hard. Only one may be selected; the chosen button is highlighted. Again, prefer icons instead of text—for example, a smiling face for Easy, a neutral face for Medium, and a frowning face for Hard. Pick icons that are easy for children to understand.

## Maze shape

The shape dropdown will be replaced by four toggle-buttons representing the available shapes: Rectangle, Circle, Diamond, and Heart. More shapes may be added later, so design the layout to accommodate additional items. Only one shape button can be selected at a time and should be highlighted. Use intuitive icons (square for Rectangle, circle for Circle, etc.) rather than text so children immediately understand each option.

## Persisting settings

The app already persists selected settings and reapplies them on next launch. After replacing the dropdowns with toggle-buttons, ensure the selected options continue to be saved and restored on subsequent app starts. This preserves user convenience and prevents the need to reconfigure settings every time the app opens.
