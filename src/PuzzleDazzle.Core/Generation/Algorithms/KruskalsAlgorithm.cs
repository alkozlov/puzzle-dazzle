using PuzzleDazzle.Core.Models;

namespace PuzzleDazzle.Core.Generation.Algorithms;

/// <summary>
/// Implements maze generation using Randomized Kruskal's Algorithm (Minimum Spanning Tree).
/// Creates perfect mazes by treating it as a graph problem.
/// Uses Union-Find data structure for efficient set operations.
/// </summary>
public class KruskalsAlgorithm : IMazeGenerationAlgorithm
{
    public string Name => "Kruskal's Algorithm";

    public string Description => "Creates perfect mazes by treating it as a minimum spanning tree problem. Good balance of characteristics with moderate branching.";

    public Maze Generate(int rows, int columns, MazeDifficulty difficulty, IProgress<double>? progress, Random random)
    {
        var maze = new Maze(rows, columns, difficulty);
        
        int totalCells = rows * columns;
        int totalWalls = (rows - 1) * columns + rows * (columns - 1); // Horizontal + vertical walls
        int processedWalls = 0;

        // Create Union-Find structure - each cell starts in its own set
        var unionFind = new UnionFind(totalCells);

        // Create list of all possible walls (edges)
        var walls = new List<Wall>();
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                var cell = maze.Grid[row, col];
                int cellIndex = row * columns + col;

                // Add wall to the right (if not on right edge)
                if (col < columns - 1)
                {
                    var rightCell = maze.Grid[row, col + 1];
                    int rightIndex = row * columns + (col + 1);
                    walls.Add(new Wall(cell, rightCell, cellIndex, rightIndex));
                }

                // Add wall below (if not on bottom edge)
                if (row < rows - 1)
                {
                    var bottomCell = maze.Grid[row + 1, col];
                    int bottomIndex = (row + 1) * columns + col;
                    walls.Add(new Wall(cell, bottomCell, cellIndex, bottomIndex));
                }
            }
        }

        // Shuffle the walls randomly
        ShuffleWalls(walls, random);

        progress?.Report(0.0);

        // Process walls in random order
        foreach (var wall in walls)
        {
            processedWalls++;

            // Check if the two cells are in different sets
            if (!unionFind.Connected(wall.Cell1Index, wall.Cell2Index))
            {
                // Remove the wall between them
                wall.Cell1.RemoveWallBetween(wall.Cell2);
                
                // Union the two sets
                unionFind.Union(wall.Cell1Index, wall.Cell2Index);
            }

            // Report progress
            if (processedWalls % 20 == 0)
            {
                progress?.Report((double)processedWalls / totalWalls);
            }
        }

        // Apply difficulty-based modifications
        ApplyDifficultyModifications(maze, difficulty, random);

        // Final progress report
        progress?.Report(1.0);

        return maze;
    }

    /// <summary>
    /// Shuffles the wall list using Fisher-Yates algorithm.
    /// </summary>
    private void ShuffleWalls(List<Wall> walls, Random random)
    {
        for (int i = walls.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            var temp = walls[i];
            walls[i] = walls[j];
            walls[j] = temp;
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

    /// <summary>
    /// Represents a wall between two cells.
    /// </summary>
    private class Wall
    {
        public Cell Cell1 { get; }
        public Cell Cell2 { get; }
        public int Cell1Index { get; }
        public int Cell2Index { get; }

        public Wall(Cell cell1, Cell cell2, int cell1Index, int cell2Index)
        {
            Cell1 = cell1;
            Cell2 = cell2;
            Cell1Index = cell1Index;
            Cell2Index = cell2Index;
        }
    }

    /// <summary>
    /// Union-Find (Disjoint Set Union) data structure for efficient set operations.
    /// </summary>
    private class UnionFind
    {
        private readonly int[] _parent;
        private readonly int[] _rank;

        public UnionFind(int size)
        {
            _parent = new int[size];
            _rank = new int[size];

            // Initially, each element is its own parent (separate set)
            for (int i = 0; i < size; i++)
            {
                _parent[i] = i;
                _rank[i] = 0;
            }
        }

        /// <summary>
        /// Finds the root of the set containing element x (with path compression).
        /// </summary>
        public int Find(int x)
        {
            if (_parent[x] != x)
            {
                _parent[x] = Find(_parent[x]); // Path compression
            }
            return _parent[x];
        }

        /// <summary>
        /// Unites the sets containing elements x and y (with union by rank).
        /// </summary>
        public void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);

            if (rootX == rootY)
                return;

            // Union by rank: attach smaller tree under larger tree
            if (_rank[rootX] < _rank[rootY])
            {
                _parent[rootX] = rootY;
            }
            else if (_rank[rootX] > _rank[rootY])
            {
                _parent[rootY] = rootX;
            }
            else
            {
                _parent[rootY] = rootX;
                _rank[rootX]++;
            }
        }

        /// <summary>
        /// Checks if elements x and y are in the same set.
        /// </summary>
        public bool Connected(int x, int y)
        {
            return Find(x) == Find(y);
        }
    }
}
