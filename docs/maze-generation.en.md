# Maze Generation Algorithm Documentation

## Overview

Mazele Dazzle uses the **Recursive Backtracking** algorithm to generate perfect mazes. A perfect maze is defined as a maze where there is exactly one path between any two points in the maze, with no loops or isolated sections.

## Algorithm: Recursive Backtracking

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

## Alternative Algorithms (Future Enhancements)

While Mazele Dazzle currently uses Recursive Backtracking, other algorithms could be considered for future versions:

### Prim's Algorithm
- **Characteristics**: Creates mazes with more branching, shorter passages
- **Advantage**: More uniform difficulty, less bias
- **Trade-off**: Requires more complex implementation

### Kruskal's Algorithm
- **Characteristics**: Treats maze generation as minimum spanning tree
- **Advantage**: Can create different maze "styles"
- **Trade-off**: More complex, requires union-find data structure

### Eller's Algorithm
- **Characteristics**: Generates maze row by row
- **Advantage**: Memory efficient, can generate infinite mazes
- **Trade-off**: More complex logic, harder to understand

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

The Recursive Backtracking algorithm provides an excellent balance of simplicity, performance, and maze quality for Mazele Dazzle. It generates perfect mazes quickly, with natural-looking passages and consistent difficulty. The implementation supports asynchronous generation, progress tracking, and difficulty-based modifications, making it suitable for a wide range of use cases and user skill levels.
