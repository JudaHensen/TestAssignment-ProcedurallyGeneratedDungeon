using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    [Header("Maximum width & depth of the dungeon grid.")]
    private Vector2Int dungeonSize = new Vector2Int(30,30);

    [SerializeField]
    [Header("Size of a single tile on the dungeon grid in units.")]
    private int tileSize = 10;

    [SerializeField]
    [Header("Increment of perlin noise.")]
    private float perlinIncrement = .25f;
    [SerializeField]
    [Header("Perlin noise x & y offset multiplier")]
    private float perlinOffsetMultiplier = 1024;

    [SerializeField]
    [Header("Threshold for dungeon room creation")]
    private float roomThresholdMinimum = 0;
    [SerializeField]
    private float roomThreshholdMaximum = .3f;

    [SerializeField]
    [Header("Minimum room size tiles.")]
    private int minimumRoomSize = 9;

    [SerializeField]
    [Header("Floor & Floor parent")]
    private GameObject basicFloor;
    [SerializeField]
    private GameObject floorParent;

    // Perlin values
    private float[,] perlinValues;

    // Map as string
    private string[,] map;

    private Seeding seeding;


    void Start () {
        seeding = GameObject.FindObjectOfType<Seeding>();

        perlinValues = new float[dungeonSize.x, dungeonSize.y];
        map = new string[dungeonSize.x, dungeonSize.y];

        GenerateDungeon();
	}

    // Generates dungeon based with perlin noise based on seed value.
    public void GenerateDungeon()
    {
        GeneratePerlinNoise();
        FilterThreshold();
        FilterRooms();
        //GeneratePaths();
        PlaceRooms();
    }

    // Removes everything outside of the room threshold.
    private void FilterThreshold()
    {
        for (int i = 0; i < dungeonSize.x; i++)
        {
            for (int j = 0; j < dungeonSize.y; j++)
            {
                if (perlinValues[i, j] >= roomThresholdMinimum && perlinValues[i, j] <= roomThreshholdMaximum)
                {
                    map[i, j] = "floor";
                }
                //else map[i, j] = "void";
            }
        }
    }

    // Filter out too small rooms
    private void FilterRooms()
    {
        List<TileGroup>[] mapGroups = new List<TileGroup>[dungeonSize.y];
        for (int i = 0; i < dungeonSize.y; i++) mapGroups[i] = new List<TileGroup>();

        // Create mapGroups
        for (int y = 0; y < dungeonSize.y; y++)
        {
            for (int x = 0; x < dungeonSize.x; x++)
            {
                if(map[x, y] == "floor")
                {
                    if(mapGroups[y].Count < 1) // create empty group
                    {
                        mapGroups[y].Add(new TileGroup());
                    }

                    // if lastgroup empty and this tile is floor
                    if (mapGroups[y][mapGroups[y].Count - 1].Count() < 1)
                    {
                        mapGroups[y][mapGroups[y].Count - 1].AddTile(new Vector2Int(x, y));
                    }
                    // if previous tile is floor
                    else if (map[x - 1, y] == "floor" && x - 1 == mapGroups[y][mapGroups[y].Count - 1].GetLastTile().x)
                    {
                       mapGroups[y][mapGroups[y].Count - 1].AddTile(new Vector2Int(x, y));
                    }
                    // make new group
                    else if(map[x - 1, y] != "floor")
                    {
                        mapGroups[y].Add(new TileGroup());
                        mapGroups[y][mapGroups[y].Count - 1].AddTile(new Vector2Int(x, y));
                    }
                }
            }
        }
        // Combine mapGroups
        for (int y = 0; y < dungeonSize.y - 1; y++)
        {
            // scroll through upper row
            for (int groupUp = 0; groupUp < mapGroups[y].Count; groupUp++)
            {
                bool stop = false;
                //if(stop) break;

                List<Vector2Int> _groupUp = mapGroups[y][groupUp].GetTiles();

                // scroll through upper row tiles
                for(int tileUp = mapGroups[y][groupUp].GetBreakpoint(); tileUp < _groupUp.Count; tileUp++)
                {
                    if (stop) break;

                    // scroll through lower row
                    for (int groupDown = 0; groupDown < mapGroups[y + 1].Count; groupDown++)
                    {
                        if (stop) break;

                        List<Vector2Int> _groupDown = mapGroups[y + 1][groupDown].GetTiles(); ;

                        // scroll through lower row tiles
                        for (int tileDown = mapGroups[y + 1][groupDown].GetBreakpoint(); tileDown < _groupDown.Count; tileDown++)
                        {
                            if (stop) break;

                            // check if both x coord are equal
                            if( _groupUp[tileUp].x == _groupDown[tileDown].x)
                            {
                                // add to second group
                                mapGroups[y + 1][groupDown].AddTilesFront(_groupUp);

                                // clear group
                                mapGroups[y][groupUp].RenderUseless();

                                stop = true;
                            }
                        }
                    }
                }
            }
        }

        // remove 'useless' groups
        for (int i = 0; i < mapGroups.Length; i++)
        {
            for (int groups = mapGroups[i].Count-1; groups >= 0; groups--)
            {
                if (mapGroups[i][groups].IsUseless()) mapGroups[i].Remove(mapGroups[i][groups]);
            }
        }

        // parse grouns into 1 list
        List<TileGroup> rooms = new List<TileGroup>();

        for(int i = 0; i < dungeonSize.y; i++)
        {
            for(int j = 0; j < mapGroups[i].Count; j++)
            {
                rooms.Add(mapGroups[i][j]);
            }
        }

        // renew map
        map = new string[dungeonSize.x, dungeonSize.y];

        for(int i = 0; i < rooms.Count; i++)
        {
            List<Vector2Int> room = rooms[i].GetTiles();
            if(room.Count >= minimumRoomSize)
            {
                for (int j = 0; j < room.Count; j++)
                {
                    map[room[j].x, room[j].y] = "floor";
                }
            }
        }


    }

    // Generate paths
    private void GeneratePaths()
    {

    }

    // Generates a perlin noise map
    private void GeneratePerlinNoise()
    {
        // perlin noise x & y offset
        float offsetX, offsetY;
        offsetX = seeding.NextValue() * perlinOffsetMultiplier;
        offsetY = seeding.NextValue() * perlinOffsetMultiplier;

        // Go through 2Dimensional array
        for (int i = 0; i < dungeonSize.x; i++)
        {
            for (int j = 0; j < dungeonSize.y; j++)
            {
                perlinValues[i, j] = Mathf.PerlinNoise(offsetX + (i * perlinIncrement), offsetY + (j * perlinIncrement));
            }
        }
    }

    // Place rooms in scene
    private void PlaceRooms()
    {
        for (int i = 0; i < dungeonSize.x; i++)
        {
            for (int j = 0; j < dungeonSize.y; j++)
            {
                if(map[i, j] == "floor")
                {
                    GameObject floor = Instantiate(basicFloor, new Vector3(i * tileSize, 0, j * -tileSize), Quaternion.identity);
                    floor.transform.parent = floorParent.transform;
                }  
                else if(map[i, j] == "path")
                {

                }
            }
        }
    }

    

}
