using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    [Header("Maximum width & depth of the dungeon grid.")]
    private Vector2Int dungeonSize = new Vector2Int(30,30);

    [SerializeField]
    [Header("Size of a single room on the dungeon grid in units.")]
    private int roomSize = 10;

    [SerializeField]
    [Header("Increment of perlin noise.")]
    private float perlinIncrement = .25f;
    [SerializeField]
    [Header("Perlin noise x & y offset multiplier")]
    private float perlinOffsetMultiplier = 1024;

    [SerializeField]
    [Header("Threshold for dungeon room creation")]
    private float roomThreshold = .75f;

    [SerializeField]
    [Header("Floor & Floor parent")]
    private GameObject basicFloor;
    [SerializeField]
    private GameObject floorParent;

    // Perlin values
    private float[,] perlinValues;

    private Seeding seeding;

    void Start () {
        seeding = GameObject.FindObjectOfType<Seeding>();

        perlinValues = new float[dungeonSize.x, dungeonSize.y];

        GenerateDungeon();
	}

    // Generaets dungeon based with perlin noise based on seed value.
    public void GenerateDungeon()
    {
        GeneratePerlinNoise();
        PlaceRooms();
    }

    private void PlaceRooms()
    {
        for (int i = 0; i < perlinValues.GetLength(0); i++)
        {
            for (int j = 0; j < perlinValues.GetLength(1); j++)
            {
                GameObject floor = Instantiate( basicFloor, new Vector3( i * roomSize, perlinValues[i, j] * 100, j * roomSize ), Quaternion.identity );
                floor.transform.parent = floorParent.transform;
            }
        }
    }

    private void GeneratePerlinNoise()
    {
        // perlin noise x & y offset
        float offsetX, offsetY;
        offsetX = seeding.NextValue() * perlinOffsetMultiplier;
        offsetY = seeding.NextValue() * perlinOffsetMultiplier;

        // Go through 2Dimensional array
        for (int i = 0; i < perlinValues.GetLength(0); i++)
        {
            for (int j = 0; j < perlinValues.GetLength(1); j++)
            {
                perlinValues[i, j] = Mathf.PerlinNoise(offsetX + (i * perlinIncrement), offsetY + (j * perlinIncrement));
            }
        }
    }

}
