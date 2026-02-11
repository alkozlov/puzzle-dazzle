using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Generation.Algorithms;

/// <summary>
/// Implements maze generation using Wilson's Algorithm (Loop-Erased Random Walk).
/// Creates perfectly unbiased mazes with uniform distribution.
/// Slower than other algorithms but produces the most "random" looking mazes.
/// </summary>
public class WilsonsAlgorithm : IMazeGenerationAlgorithm
{
    public string Name => "Wilson's Algorithm";

    public string Description => "Creates perfectly unbiased mazes using loop-erased random walks. Slower but produces the most random-looking mazes.";

    public Maze Generate(int rows, int columns, MazeDifficulty difficulty, IProgress<double>? progress, Random random)
    {
        var maze = new Maze(rows, columns, difficulty);
        
        int totalCells = rows * columns;
        int visitedCells = 0;

        // Track which cells are part of the maze
        var inMaze = new bool[rows, columns];
        
        // Track the path during random walk
        var path = new Dictionary<(int, int), (int, int)>();

        // Start with a random cell in the maze
        var startRow = random.Next(rows);
        var startCol = random.Next(columns);
        inMaze[startRow, startCol] = true;
        visitedCells++;

        progress?.Report((double)visitedCells / totalCells);

        // Get list of all cells not yet in maze
        var remainingCells = new List<(int row, int col)>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (!inMaze[row, col])
                {
                    remainingCells.Add((row, col));
                }
            }
        }

        // Process remaining cells
        while (remainingCells.Count > 0)
        {
            // Pick a random unvisited cell
            var startCell = remainingCells[random.Next(remainingCells.Count)];
            
            // Perform loop-erased random walk from this cell
            path.Clear();
            var current = startCell;

            // Walk until we hit a cell that's already in the maze
            while (!inMaze[current.row, current.col])
            {
                // Get valid neighbors
                var neighbors = GetNeighbors(current.row, current.col, rows, columns);
                
                // Pick random neighbor
                var next = neighbors[random.Next(neighbors.Count)];

                // Add to path (overwrites if we create a loop, thus "loop-erasing")
                path[current] = next;
                
                current = next;
            }

            // Now add the entire path to the maze
            current = startCell;
            while (!inMaze[current.row, current.col])
            {
                inMaze[current.row, current.col] = true;
                visitedCells++;
                
                var next = path[current];
                
                // Remove wall between current and next
                var currentCell = maze.Grid[current.row, current.col];
                var nextCell = maze.Grid[next.Item1, next.Item2];
                currentCell.RemoveWallBetween(nextCell);

                current = next;

                // Report progress
                if (visitedCells % 10 == 0)
                {
                    progress?.Report((double)visitedCells / totalCells);
                }
            }

            // Update remaining cells list
            remainingCells.RemoveAll(c => inMaze[c.row, c.col]);
        }

        // Apply difficulty-based modifications
        ApplyDifficultyModifications(maze, difficulty, random);

        // Final progress report
        progress?.Report(1.0);

        return maze;
    }

    /// <summary>
    /// Gets valid neighbors for a cell (all 4 directions if within bounds).
    /// </summary>
    private List<(int row, int col)> GetNeighbors(int row, int col, int rows, int columns)
    {
        var neighbors = new List<(int, int)>();

        // Top
        if (row > 0)
            neighbors.Add((row - 1, col));

        // Right
        if (col < columns - 1)
            neighbors.Add((row, col + 1));

        // Bottom
        if (row < rows - 1)
            neighbors.Add((row + 1, col));

        // Left
        if (col > 0)
            neighbors.Add((row, col - 1));

        return neighbors;
    }

    /// <summary>
    /// Applies modifications based on difficulty level.
    /// </summary>
    private void ApplyDifficultyModifications(Maze maze, MazeDifficulty difficulty, Random random)
    {
        switch (difficulty)
        {
            case MazeDifficulty.Easy:
                RemoveRandomWalls(maze, wallsToRemove: (int)(maze.Rows * maze.Columns * 0.15), random);
                break;

            case MazeDifficulty.Medium:
                break;

            case MazeDifficulty.Hard:
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
