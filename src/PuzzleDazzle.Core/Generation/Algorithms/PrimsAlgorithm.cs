using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Generation.Algorithms;

/// <summary>
/// Implements maze generation using Randomized Prim's Algorithm.
/// Creates perfect mazes with more branching and shorter passages compared to Recursive Backtracking.
/// Grows the maze from a single cell by randomly adding frontier cells.
/// </summary>
public class PrimsAlgorithm : IMazeGenerationAlgorithm
{
    public string Name => "Prim's Algorithm";

    public string Description => "Creates perfect mazes with more branching and shorter passages. Grows the maze from a single cell by adding random frontier cells.";

    public Maze Generate(int rows, int columns, MazeDifficulty difficulty, IProgress<double>? progress, Random random)
    {
        var maze = new Maze(rows, columns, difficulty);
        
        // Total cells to visit for progress tracking
        int totalCells = rows * columns;
        int visitedCells = 0;

        // List of frontier cells (cells adjacent to the maze but not yet part of it)
        var frontierCells = new List<(Cell cell, Cell neighbor)>();

        // Start from the start cell
        var start = maze.StartCell;
        start.Visited = true;
        visitedCells++;
        
        // Add all unvisited neighbors to frontier
        AddFrontierCells(maze, start, frontierCells);
        
        progress?.Report((double)visitedCells / totalCells);

        // Keep adding cells until no frontier cells remain
        while (frontierCells.Count > 0)
        {
            // Pick a random frontier cell
            int index = random.Next(frontierCells.Count);
            var (frontierCell, neighborCell) = frontierCells[index];
            frontierCells.RemoveAt(index);

            // If the frontier cell hasn't been visited yet
            if (!frontierCell.Visited)
            {
                // Mark it as part of the maze
                frontierCell.Visited = true;
                visitedCells++;

                // Remove wall between frontier cell and its neighbor (that's already in the maze)
                frontierCell.RemoveWallBetween(neighborCell);

                // Add this cell's unvisited neighbors to the frontier
                AddFrontierCells(maze, frontierCell, frontierCells);

                // Report progress
                if (visitedCells % 10 == 0)
                {
                    progress?.Report((double)visitedCells / totalCells);
                }
            }
        }

        // Apply difficulty-based modifications
        ApplyDifficultyModifications(maze, difficulty, random);

        // Reset visited flags
        maze.ResetVisited();

        // Final progress report
        progress?.Report(1.0);

        return maze;
    }

    /// <summary>
    /// Adds all unvisited neighbors of a cell to the frontier list.
    /// </summary>
    private void AddFrontierCells(Maze maze, Cell cell, List<(Cell, Cell)> frontierCells)
    {
        var unvisitedNeighbors = maze.GetUnvisitedNeighbors(cell);
        
        foreach (var neighbor in unvisitedNeighbors)
        {
            // Add as (frontier cell, neighbor in maze)
            if (!frontierCells.Any(f => f.Item1 == neighbor))
            {
                frontierCells.Add((neighbor, cell));
            }
        }
    }

    /// <summary>
    /// Applies modifications based on difficulty level.
    /// </summary>
    private void ApplyDifficultyModifications(Maze maze, MazeDifficulty difficulty, Random random)
    {
        switch (difficulty)
        {
            case MazeDifficulty.Easy:
                // For easy difficulty, remove some additional walls to create shortcuts
                RemoveRandomWalls(maze, wallsToRemove: (int)(maze.Rows * maze.Columns * 0.15), random);
                break;

            case MazeDifficulty.Medium:
                // Medium keeps the generated maze as-is
                break;

            case MazeDifficulty.Hard:
                // For hard difficulty, the generated maze is already complex enough
                break;
        }
    }

    /// <summary>
    /// Removes random walls to create shortcuts and make the maze easier.
    /// </summary>
    private void RemoveRandomWalls(Maze maze, int wallsToRemove, Random random)
    {
        int removed = 0;
        int maxAttempts = wallsToRemove * 3;
        int attempts = 0;

        while (removed < wallsToRemove && attempts < maxAttempts)
        {
            attempts++;

            int row = random.Next(maze.Rows);
            int col = random.Next(maze.Columns);
            var cell = maze.Grid[row, col];

            if (cell.IsStart || cell.IsEnd)
                continue;

            int wallDirection = random.Next(4);

            switch (wallDirection)
            {
                case 0: // Top
                    if (cell.TopWall && row > 0)
                    {
                        var neighbor = maze.Grid[row - 1, col];
                        cell.RemoveWallBetween(neighbor);
                        removed++;
                    }
                    break;

                case 1: // Right
                    if (cell.RightWall && col < maze.Columns - 1)
                    {
                        var neighbor = maze.Grid[row, col + 1];
                        cell.RemoveWallBetween(neighbor);
                        removed++;
                    }
                    break;

                case 2: // Bottom
                    if (cell.BottomWall && row < maze.Rows - 1)
                    {
                        var neighbor = maze.Grid[row + 1, col];
                        cell.RemoveWallBetween(neighbor);
                        removed++;
                    }
                    break;

                case 3: // Left
                    if (cell.LeftWall && col > 0)
                    {
                        var neighbor = maze.Grid[row, col - 1];
                        cell.RemoveWallBetween(neighbor);
                        removed++;
                    }
                    break;
            }
        }
    }
}
