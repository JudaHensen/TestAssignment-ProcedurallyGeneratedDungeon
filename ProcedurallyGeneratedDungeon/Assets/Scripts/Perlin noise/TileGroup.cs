using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroup {

    private List<Vector2Int> tiles = new List<Vector2Int>();
    private int lastBreakpoint = 0;

    private bool useless = false;

    public void AddTile(Vector2Int tile)
    {
        tiles.Add(tile);
    }

    public void AddTiles(List<Vector2Int> newTiles)
    {
        lastBreakpoint = tiles.Count;
        for(int i = 0; i < newTiles.Count; i++)
        {
            tiles.Add(newTiles[i]);
        }
    }

    // Shift new tiles to 1st place
    public void AddTilesFront(List<Vector2Int> newTiles)
    {
        // save old values
        List<Vector2Int> oldTiles = new List<Vector2Int>();
        for (int i = 0; i < tiles.Count; i++) oldTiles.Add(tiles[i]);

        // insert new values
        tiles.Clear();
        for (int i = 0; i < newTiles.Count; i++) tiles.Add(newTiles[i]);

        // save new breakpoint
        lastBreakpoint += tiles.Count;

        // add old values
        for (int i = 0; i < oldTiles.Count; i++) tiles.Add(oldTiles[i]);

    }

    public int Count() { return tiles.Count; }

    // use breakpoint for scanning on a new y axis
    public int GetBreakpoint() { return lastBreakpoint; }

    public List<Vector2Int> GetTiles() { return tiles; }

    public Vector2Int GetLastTile() { return tiles[tiles.Count - 1]; }

    public void RenderUseless()
    {
        useless = true;
        tiles.Clear();
    }

    public bool IsUseless() { return useless; }
}
