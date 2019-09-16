using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeding : MonoBehaviour {

    [SerializeField]
    private uint mainSeed = 1;
    [SerializeField]
    // use creatorSeed for seed generation
    private uint creatorSeed = 2;

    private string currentSeed = "main";
    private uint maximumLength = 99999999;

    private Dictionary<string, Seed> seeds = new Dictionary<string, Seed>();

	void Start () {
        seeds.Add("creator", new Seed(creatorSeed));

        seeds.Add("main", new Seed(mainSeed));
        Random.InitState( (int) seeds["main"].GetSeed() );
    }

    // Retrieves next seed value and updates that seeds state.
    public float NextValue()
    {
        return seeds[currentSeed].NextValue();
    }

    // Changes seed
    public void ChangeSeed(string seedName)
    {
        Random.state = seeds[seedName].GetState();
        currentSeed = seedName;
    }

    // Create seed based on "creator" seed
    public void CreateSeed(string seedName)
    {
        string previousSeed = currentSeed;
        ChangeSeed("creator");
        seeds.Add(seedName, new Seed( (uint) Mathf.Floor(seeds[currentSeed].NextValue() * maximumLength) ));
        ChangeSeed(previousSeed);
    }

    // Create seed based in input value 
    public void CreateSeed(string seedName, uint value)
    {
        if (value > maximumLength) Debug.Log("Seed not created, inpur value is to long!");
        else seeds.Add(seedName, new Seed(value));
    }

    // Removes seed, cannot remove main & creator seed.
    public void RemoveSeed(string seedName)
    {
        if(seedName != "main" && seedName != "creator") seeds.Remove(seedName);
    }
}