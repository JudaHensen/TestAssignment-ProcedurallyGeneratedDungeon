using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    private Transform parent;
    private GameObject floor;
    private TileGroup tiles;

    private int id;
    private int scale;

    private Vector2Int positionCenter = new Vector2Int();

    public Room(Transform _parent, GameObject _floor, TileGroup _tiles, int _scale, int _id)    
    {
        parent = _parent;
        floor = _floor;
        tiles = _tiles;

        scale = _scale;
        id = _id;

        CalculateCenter();
    }

    // Calculate center position of room
    private void CalculateCenter()
    {
        List<Vector2Int> floorTiles = tiles.GetTiles();
        // search for lowest x / y & highest x / y
        int lx = -1, ly = -1, hx = -1, hy = -1;

        for (int i = 0; i < floorTiles.Count; i++)
        {
            if (i == 0) {
                lx = floorTiles[i].x;
                hx = floorTiles[i].x;
                ly = floorTiles[i].y;
                hy = floorTiles[i].y;
            }
            else
            {
                if (floorTiles[i].x < lx) lx = floorTiles[i].x;
                if (floorTiles[i].x > hx) hx = floorTiles[i].x;
                if (floorTiles[i].y < ly) ly = floorTiles[i].y;
                if (floorTiles[i].y > hy) hy = floorTiles[i].y;
            }
        }

        // define center position
        positionCenter.x = lx + ((hx - lx)/2);
        positionCenter.y = ly + ((hy - ly)/2);
    }

    public Vector2Int GetCenter() { return positionCenter; }

    public bool CheckPosition(Vector2Int item)
    {
        List<Vector2Int> floorTiles = tiles.GetTiles();
        for(int i = 0; i < floorTiles.Count; i++)
        {
            if (item.x == floorTiles[i].x && item.y == floorTiles[i].y) return true;
        }
        return false;
    }

    public void Place()
    {
        GameObject room = new GameObject();
        room.transform.parent = parent;
        room.name = "room_" + id;

        List<Vector2Int> floorTiles = tiles.GetTiles();
        for (int i = 0; i < floorTiles.Count; i++)
        {
            GameObject floorTile = Instantiate(floor, new Vector3(floorTiles[i].x * scale, 0, -floorTiles[i].y * scale), Quaternion.identity);
            floorTile.transform.parent = room.transform;
        }
    }
    
}
