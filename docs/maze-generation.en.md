# Maze Generation Algorithm Documentation

## Overview

Mazele Dazzle supports **multiple maze generation algorithms**, allowing users to compare different maze styles and characteristics. All algorithms generate perfect mazes - mazes where there is exactly one path between any two points, with no loops or isolated sections.

### Available Algorithms

1. **Recursive Backtracking (Default)** - Long winding corridors, fast generation
2. **Prim's Algorithm** - More branching, shorter passages
3. **Kruskal's Algorithm** - Balanced characteristics, treats maze as graph
4. **Wilson's Algorithm** - Perfectly unbiased, uniform distribution

## Algorithm 1: Recursive Backtracking (Default)

### Description

Recursive backtracking is a depth-first search algorithm that carves passages through a grid of cells by removing walls between adjacent cells. It guarantees the creation of a perfect maze with the following properties:

- **Connectivity**: Every cell is reachable from every other cell
- **No Loops**: There is exactly one path between any two cells
- **No Isolated Areas**: All cells are part of the same connected maze structure

### How It Works

1. **Initialization**
   - Create a grid of cells (rows × columns)
   - Each cell initially has all four walls intact (top, right, bottom, left)
   - Mark all cells as unvisited
   - Choose a starting cell (typically top-left corner)

2. **Main Algorithm Loop**
   ```
   1. Mark current cell as visited
   2. Get list of unvisited neighbors (adjacent cells)
   3. If unvisited neighbors exist:
      a. Choose one neighbor randomly
      b. Remove wall between current cell and chosen neighbor
      c. Push current cell onto stack
      d. Move to chosen neighbor (make it current)
      e. Repeat from step 1
   4. If no unvisited neighbors:
      a. Pop cell from stack
      b. Make it current cell
      c. Repeat from step 1
   5. If stack is empty:
      a. Algorithm complete
   ```

3. **Result**
   - A perfect maze with passages carved through the grid
   - Start point: top-left corner (0, 0)
   - End point: bottom-right corner (rows-1, columns-1)

### Visual Example

**Step-by-step generation of a 5×5 maze:**

```
Initial Grid (all walls intact):
┌─┬─┬─┬─┬─┐
├─┼─┼─┼─┼─┤
├─┼─┼─┼─┼─┤
├─┼─┼─┼─┼─┤
└─┴─┴─┴─┴─┘

After Generation (walls removed to create paths):
┌─────┬───┐
│ ┌─┐ │ ┌─┤
│ │ └─┘ │ │
│ └─┬───┘ │
└───┴─────┘
```

### Why Recursive Backtracking?

**Advantages:**
- **Simple to implement**: Straightforward stack-based algorithm
- **Guaranteed perfect maze**: Always produces valid, solvable mazes
- **Good complexity**: Creates interesting paths with natural branching
- **Long passages**: Tends to create long, winding corridors
- **Uniform difficulty**: Consistent maze complexity

**Characteristics:**
- Creates mazes with high "river" factor (long corridors)
- Low branching factor in most areas
- Natural flow and organic feel
- Predictable performance: O(n) where n is number of cells

**Trade-offs:**
- Bias toward long passages (less branching than some algorithms)
- Can create easy-to-solve mazes if passages are too linear
- Not optimal for all maze styles (e.g., highly branching mazes)

## Implementation Details

### Data Structures

#### Cell Class
```csharp
public class Cell
{
    public int Row { get; set; }
    public int Column { get; set; }
    
    // Wall states
    public bool TopWall { get; set; }
    public bool RightWall { get; set; }
    public bool BottomWall { get; set; }
    public bool LeftWall { get; set; }
    
    // Algorithm state
    public bool Visited { get; set; }
    
    // Maze endpoints
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }
}
```

#### Maze Class
```csharp
public class Maze
{
    public int Rows { get; }
    public int Columns { get; }
    public Cell[,] Grid { get; }
    public Cell StartCell { get; }
    public Cell EndCell { get; }
    public MazeDifficulty Difficulty { get; }
}
```

### Difficulty Levels

The implementation includes three difficulty levels that modify the generated maze:

#### Easy
- **Modification**: Removes approximately 15% of additional walls after generation
- **Effect**: Creates shortcuts and multiple paths between points
- **Result**: Easier to solve, less chance of getting lost
- **Use case**: Beginners, children, quick puzzles

#### Medium
- **Modification**: No additional changes to generated maze
- **Effect**: Standard perfect maze with one path between any two points
- **Result**: Balanced difficulty, requires some thinking
- **Use case**: General audience, standard puzzles

#### Hard
- **Modification**: Keeps the maze as-is (most complex)
- **Effect**: Perfect maze with maximum path length
- **Result**: Challenging, requires strategic thinking
- **Use case**: Experienced users, challenging puzzles

### Asynchronous Generation

The maze generator supports asynchronous operation to maintain UI responsiveness:

```csharp
public async Task<Maze> GenerateAsync(
    int rows, 
    int columns, 
    MazeDifficulty difficulty = MazeDifficulty.Medium,
    IProgress<double>? progress = null)
```

**Benefits:**
- **Non-blocking**: UI remains responsive during generation
- **Progress tracking**: Reports completion percentage (0.0 to 1.0)
- **Cancellable**: Can be cancelled if needed (future enhancement)

**Progress Updates:**
- Updates every 10 cells to balance responsiveness and performance
- Reports: `(visitedCells / totalCells)` as a value between 0.0 and 1.0
- Final report: 1.0 when generation is complete

## Performance Characteristics

### Time Complexity
- **O(n)** where n = rows × columns (number of cells)
- Each cell is visited exactly once
- Backtracking ensures complete coverage

### Space Complexity
- **O(n)** for the maze grid storage
- **O(n)** worst-case for the stack (in case of a long passage)
- Total: **O(n)** space required

### Generation Speed
- Small maze (10×10): <1ms
- Medium maze (20×20): ~2-5ms
- Large maze (30×30): ~5-10ms
- Very large maze (50×50): ~20-30ms

*Note: Times are approximate and depend on hardware.*

## Maze Size Recommendations

### Small (10×10)
- **Best for**: Quick puzzles, beginners, mobile screens
- **Generation time**: Instant
- **Difficulty**: Easy to solve even on Hard setting

### Medium (20×20)
- **Best for**: Standard puzzles, balanced difficulty
- **Generation time**: Nearly instant
- **Difficulty**: Good challenge on Medium/Hard

### Large (30×30)
- **Best for**: Long puzzles, experienced users
- **Generation time**: Very fast (<100ms)
- **Difficulty**: Challenging even for experienced solvers

## Algorithm 2: Prim's Algorithm

### Description

Prim's Algorithm is a randomized version of the classical minimum spanning tree algorithm. Instead of selecting the shortest edge, it randomly selects from all possible edges connecting the maze to unvisited cells. This creates mazes with more branching and shorter passages compared to Recursive Backtracking.

### Characteristics

- **Generation Style**: Grows maze from a single cell, adding random frontier cells
- **Passage Style**: More branching, shorter dead-ends
- **Visual Appearance**: More "balanced" looking, less bias toward long corridors
- **Performance**: Medium speed, O(n log n) complexity
- **Memory**: Requires storing frontier cell list

### How It Works

1. Start with one cell in the maze
2. Add all unvisited neighbors to a frontier list
3. Randomly select a frontier cell and its neighbor in the maze
4. Remove the wall between them, add frontier cell to maze
5. Add new frontier cells from the newly added cell
6. Repeat until no frontier cells remain

## Algorithm 3: Kruskal's Algorithm

### Description

Kruskal's Algorithm treats maze generation as a minimum spanning tree problem using a Union-Find data structure. It randomly removes walls between disconnected regions until all cells are connected. This creates mazes with a good balance of characteristics.

### Characteristics

- **Generation Style**: Processes edges (walls) in random order
- **Passage Style**: Balanced branching, moderate passage length
- **Visual Appearance**: Natural looking, no strong directional bias
- **Performance**: Fast, O(n α(n)) where α is inverse Ackermann function (effectively O(n))
- **Memory**: Requires Union-Find structure for all cells

### How It Works

1. Create list of all walls (edges) between cells
2. Shuffle the wall list randomly
3. For each wall in random order:
   - Check if removing it would connect two separate regions (using Union-Find)
   - If yes, remove the wall and unite the regions
4. Continue until all cells are in one connected component

### Union-Find Data Structure

Uses path compression and union by rank for efficient set operations:
- **Find**: Determines which set a cell belongs to
- **Union**: Merges two sets together
- **Connected**: Checks if two cells are in the same set

## Algorithm 4: Wilson's Algorithm

### Description

Wilson's Algorithm uses loop-erased random walks to generate perfectly unbiased mazes. It's slower than other algorithms but produces mazes with the most uniform distribution - every possible perfect maze has equal probability of being generated.

### Characteristics

- **Generation Style**: Random walks from unvisited cells until hitting the maze
- **Passage Style**: Perfectly uniform, no algorithmic bias
- **Visual Appearance**: Most "random" looking mazes
- **Performance**: Slower, especially for large mazes (can approach O(n²))
- **Memory**: Requires tracking path during random walks

### How It Works

1. Start with one random cell in the maze
2. Pick a random unvisited cell
3. Perform random walk from that cell until hitting the maze
4. If the walk creates a loop, erase the loop (loop-erased random walk)
5. Add the entire path to the maze
6. Repeat for remaining unvisited cells

### Loop-Erased Random Walk

The key innovation: when the random walk intersects itself, erase the loop. This ensures the final path is loop-free and creates the unbiased property.

## Algorithm Comparison

| Algorithm | Speed | Branching | Bias | Complexity |
|-----------|-------|-----------|------|------------|
| Recursive Backtracking | Fast | Low | Long corridors | O(n) |
| Prim's | Medium | High | Moderate | O(n log n) |
| Kruskal's | Fast | Medium | Low | O(n α(n)) |
| Wilson's | Slow | Medium | None | O(n²) worst case |

### Which Algorithm to Choose?

- **Recursive Backtracking**: Best for performance, classic maze feel
- **Prim's**: Want more branching, balanced difficulty
- **Kruskal's**: Good all-around choice, natural appearance
- **Wilson's**: Want mathematically perfect randomness, willing to wait

## Implementation Architecture

### Interface: `IMazeGenerationAlgorithm`

All algorithms implement a common interface:

```csharp
public interface IMazeGenerationAlgorithm
{
    string Name { get; }
    string Description { get; }
    Maze Generate(int rows, int columns, MazeDifficulty difficulty, 
                  IProgress<double>? progress, Random random);
}
```

### Algorithm Selection

The `MazeGenerator` class supports algorithm selection via the `MazeAlgorithm` enum:

```csharp
public enum MazeAlgorithm
{
    RecursiveBacktracking = 0,  // Default
    Prims = 1,
    Kruskals = 2,
    Wilsons = 3
}
```

### Usage Example

```csharp
var generator = new MazeGenerator();
var algorithm = MazeAlgorithm.Prims;
var maze = await generator.GenerateAsync(
    rows: 20, 
    columns: 20, 
    difficulty: MazeDifficulty.Medium, 
    algorithm: algorithm,
    progress: myProgressHandler
);
```

## Alternative Algorithms (Not Yet Implemented)
## Alternative Algorithms (Not Yet Implemented)

Other algorithms that could be considered for future versions:

### Eller's Algorithm
- **Characteristics**: Generates maze row by row
- **Advantage**: Memory efficient, can generate infinite mazes
- **Trade-off**: More complex logic, harder to understand

### Aldous-Broder Algorithm
- **Characteristics**: Uses random walks
- **Advantage**: Unbiased like Wilson's but simpler
- **Trade-off**: Slower, especially near completion

### Hunt and Kill
- **Characteristics**: Similar to Recursive Backtracking with scanning
- **Advantage**: More balanced passages than pure backtracking
- **Trade-off**: More complex implementation

### Wilson's Algorithm
- **Characteristics**: Uses random walks, truly unbiased
- **Advantage**: Perfectly uniform maze distribution
- **Trade-off**: Slower generation time

## References and Further Reading

1. **"Mazes for Programmers"** by Jamis Buck
   - Comprehensive guide to maze generation algorithms
   - Detailed explanations and implementations

2. **"Think Labyrinth!"** by Walter D. Pullen
   - Mathematical analysis of maze algorithms
   - http://www.astrolog.org/labyrnth.htm

3. **Recursive Backtracking Depth-First Search**
   - Classic algorithm used in many maze generators
   - Foundation for path-finding and graph traversal

4. **Maze Classification Project** by Jamis Buck
   - Visual comparison of different maze algorithms
   - http://weblog.jamisbuck.org/2011/2/7/maze-generation-algorithm-recap

## Conclusion

Mazele Dazzle supports four different maze generation algorithms, each with unique characteristics and trade-offs. The Recursive Backtracking algorithm remains the default for its excellent balance of simplicity, performance, and maze quality. However, users can now experiment with Prim's, Kruskal's, and Wilson's algorithms to experience different maze styles and compare their characteristics.

All algorithms:
- Generate perfect mazes (exactly one path between any two cells)
- Support three difficulty levels (Easy, Medium, Hard)
- Provide progress tracking during generation
- Use configurable random seeds for reproducibility

This variety allows users to explore how different algorithms create different maze "personalities" - from the long winding corridors of Recursive Backtracking to the perfectly unbiased randomness of Wilson's Algorithm.
