using UnityEngine;
public class PresetBoard3 : MonoBehaviour
{
    //called by GameManager.InitialiseGrid() when this preset is the chosen map
    public void Apply(CellType[,] grid, int gridSize)
    {
        if (gridSize != 6)
        {
            Debug.LogWarning("PresetBoard3: designed for 6x6, gridSize mismatch");
            return;
        }

        //
        // ROW 0 
        //
        grid[0, 0] = CellType.Open;
        grid[0, 1] = CellType.TJunctionWest;
        grid[0, 2] = CellType.TJunctionEast;
        grid[0, 3] = CellType.TJunctionSouth;
        grid[0, 4] = CellType.CorridorEastWest;
        grid[0, 5] = CellType.TJunctionNorth;

        //
        // ROW 1 
        //
        grid[1, 0] = CellType.CorridorNorthSouth;
        grid[1, 1] = CellType.TJunctionEast;
        grid[1, 2] = CellType.Open;
        grid[1, 3] = CellType.Open;
        grid[1, 4] = CellType.CorridorEastWest;
        grid[1, 5] = CellType.CorridorEastWest;

        //
        // ROW 2 
        //
        grid[2, 0] = CellType.CornerNorthWest;
        grid[2, 1] = CellType.CorridorNorthSouth;
        grid[2, 2] = CellType.CornerNorthEast;
        grid[2, 3] = CellType.Open;
        grid[2, 4] = CellType.CorridorEastWest;
        grid[2, 5] = CellType.CornerSouthWest;

        //
        // ROW 3 
        //
        grid[3, 0] = CellType.CorridorEastWest;
        grid[3, 1] = CellType.Open;
        grid[3, 2] = CellType.CornerSouthWest;
        grid[3, 3] = CellType.CorridorNorthSouth;
        grid[3, 4] = CellType.CornerSouthEast;
        grid[3, 5] = CellType.Open;

        //
        // ROW 4 
        //
        grid[4, 0] = CellType.CorridorEastWest;
        grid[4, 1] = CellType.Open;
        grid[4, 2] = CellType.Open;
        grid[4, 3] = CellType.Open;
        grid[4, 4] = CellType.Open;
        grid[4, 5] = CellType.CornerNorthWest;

        //
        // ROW 5 
        //
        grid[5, 0] = CellType.CorridorEastWest;
        grid[5, 1] = CellType.Open;
        grid[5, 2] = CellType.Open;
        grid[5, 3] = CellType.Open;
        grid[5, 4] = CellType.Open;
        grid[5, 5] = CellType.CorridorEastWest;
    }
}
