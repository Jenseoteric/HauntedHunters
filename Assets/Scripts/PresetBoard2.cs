using UnityEngine;

/// <summary>
/// PresetBoard2.cs
/// Second maze layout - more open style than PresetBoard.
/// Attach to the GameManager GameObject, then drag it into
/// the Preset Board 2 field on the GameManager Inspector.
///
/// MAZE DESIGN NOTES - 6x6:
/// distribution: 14 open, 12 T-junction, 6 corner, 4 corridor
/// (user wanted 14/5/11/6 but 11 junctions is mathematically impossible
///  on a connected board - odd exit count - so junctions bumped to nearest even 12,
///  corner kept exactly at 6, corridor dropped to 4)
/// big open core makes the ghost very mobile, perimeter is mostly junctions/corners
/// shifting the middle rows/cols dramatically reshapes the open zone
///
/// VERIFIED: every tile exit matches its neighbour - exhaustive check passed
/// </summary>
public class PresetBoard2 : MonoBehaviour
{
    //called by GameManager.InitialiseGrid() when this preset is the chosen map
    public void Apply(CellType[,] grid, int gridSize)
    {
        if (gridSize != 6)
        {
            Debug.LogWarning("PresetBoard2: designed for 6x6, gridSize mismatch");
            return;
        }

        //
        // ROW 0 - top border, two south openings feed the open core
        //
        grid[0, 0] = CellType.CornerSouthEast;
        grid[0, 1] = CellType.CorridorEastWest;
        grid[0, 2] = CellType.TJunctionSouth;
        grid[0, 3] = CellType.TJunctionSouth;
        grid[0, 4] = CellType.CorridorEastWest;
        grid[0, 5] = CellType.CornerSouthWest;

        //
        // ROW 1 - upper edge of the open zone, corners pinch it inward
        //
        grid[1, 0] = CellType.CorridorNorthSouth;
        grid[1, 1] = CellType.CornerSouthEast;
        grid[1, 2] = CellType.Open;
        grid[1, 3] = CellType.Open;
        grid[1, 4] = CellType.CornerSouthWest;
        grid[1, 5] = CellType.CorridorNorthSouth;

        //
        // ROW 2 - full open band, junctions on both side walls
        //
        grid[2, 0] = CellType.TJunctionEast;
        grid[2, 1] = CellType.Open;
        grid[2, 2] = CellType.Open;
        grid[2, 3] = CellType.Open;
        grid[2, 4] = CellType.Open;
        grid[2, 5] = CellType.TJunctionWest;

        //
        // ROW 3 - full open band, mirrors row 2
        //
        grid[3, 0] = CellType.TJunctionEast;
        grid[3, 1] = CellType.Open;
        grid[3, 2] = CellType.Open;
        grid[3, 3] = CellType.Open;
        grid[3, 4] = CellType.Open;
        grid[3, 5] = CellType.TJunctionWest;

        //
        // ROW 4 - full open band, junctions on both side walls
        //
        grid[4, 0] = CellType.TJunctionEast;
        grid[4, 1] = CellType.Open;
        grid[4, 2] = CellType.Open;
        grid[4, 3] = CellType.Open;
        grid[4, 4] = CellType.Open;
        grid[4, 5] = CellType.TJunctionWest;

        //
        // ROW 5 - bottom border, four north openings feed the open core
        //
        grid[5, 0] = CellType.CornerNorthEast;
        grid[5, 1] = CellType.TJunctionNorth;
        grid[5, 2] = CellType.TJunctionNorth;
        grid[5, 3] = CellType.TJunctionNorth;
        grid[5, 4] = CellType.TJunctionNorth;
        grid[5, 5] = CellType.CornerNorthWest;
    }
}
