using UnityEngine;

public class PresetBoard : MonoBehaviour
{
    //called by GameManager.InitialiseGrid() to write the maze into the grid array
    public void Apply(CellType[,] grid, int gridSize)
    {
        if (gridSize != 6)
        {
            Debug.LogWarning("PresetBoard: designed for 6x6, gridSize mismatch");
            return;
        }

        //
        // ROW 0 - top border, full east-west corridor with two south exits
        //
        grid[0, 0] = CellType.CornerSouthEast;    // ┌ top-left corner
        grid[0, 1] = CellType.TJunctionSouth;     // ┬ opens down into [1,1]
        grid[0, 2] = CellType.CorridorEastWest;   // ─ pure horizontal
        grid[0, 3] = CellType.TJunctionSouth;     // ┬ opens down into [1,3]
        grid[0, 4] = CellType.CorridorEastWest;   // ─ pure horizontal
        grid[0, 5] = CellType.CornerSouthWest;    // ┐ top-right corner

        //
        // ROW 1 - upper zone, two open crossroads, right side vertical begins
        //
        grid[1, 0] = CellType.TJunctionEast;      // ├ left corridor, opens right
        grid[1, 1] = CellType.Open;               // + crossroads, all four exits
        grid[1, 2] = CellType.CorridorEastWest;   // ─ connects [1,1] to [1,3]
        grid[1, 3] = CellType.Open;               // + crossroads, all four exits
        grid[1, 4] = CellType.CornerSouthWest;    // ┐ dead end, goes down only
        grid[1, 5] = CellType.CorridorNorthSouth; // │ right-side vertical

        //
        // ROW 2 - mid-upper zone, wall cell creates chokepoint
        //
        grid[2, 0] = CellType.TJunctionEast;      // ├ left corridor continues
        grid[2, 1] = CellType.TJunctionNorth;             // █ WALL - blocks movement, chokepoint
        grid[2, 2] = CellType.CornerSouthEast;    // ┌ opens south and east
        grid[2, 3] = CellType.CornerNorthWest;    // ┘ connects from east, opens north
        grid[2, 4] = CellType.CorridorNorthSouth; // │ vertical connector
        grid[2, 5] = CellType.TJunctionWest;      // ┤ right side, three-way

        //
        // ROW 3 - mid-lower zone, L-shapes and T-junctions create winding paths
        //
        grid[3, 0] = CellType.CorridorNorthSouth; // │ left-side vertical
        grid[3, 1] = CellType.CornerSouthEast;    // ┌ opens south and east
        grid[3, 2] = CellType.TJunctionWest;      // ┤ three-way, no east exit
        grid[3, 3] = CellType.CornerSouthEast;    // ┌ opens south and east
        grid[3, 4] = CellType.CornerNorthWest;    // ┘ connects from east, opens north
        grid[3, 5] = CellType.TJunctionWest;      // ┤ right side continues

        //
        // ROW 4 - lower zone, central open cell, wide horizontal passage
        //
        grid[4, 0] = CellType.TJunctionEast;      // ├ left corridor, three-way
        grid[4, 1] = CellType.TJunctionNorth;     // ┴ T-junction, no south exit
        grid[4, 2] = CellType.Open;               // + central crossroads
        grid[4, 3] = CellType.TJunctionNorth;     // ┴ T-junction, no south exit
        grid[4, 4] = CellType.CorridorEastWest;   // ─ horizontal connector
        grid[4, 5] = CellType.TJunctionWest;      // ┤ right side, three-way

        //
        // ROW 5 - bottom border, full east-west corridor with three north exits
        //
        grid[5, 0] = CellType.CornerNorthEast;    // └ bottom-left corner
        grid[5, 1] = CellType.CorridorEastWest;   // ─ pure horizontal
        grid[5, 2] = CellType.TJunctionNorth;     // ┴ opens up into [4,2]
        grid[5, 3] = CellType.CorridorEastWest;   // ─ pure horizontal
        grid[5, 4] = CellType.CorridorEastWest;   // ─ pure horizontal
        grid[5, 5] = CellType.CornerNorthWest;    // ┘ bottom-right corner
    }
}
