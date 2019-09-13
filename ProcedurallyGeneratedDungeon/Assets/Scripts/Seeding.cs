using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeding : MonoBehaviour {

    private uint seed;
    private uint maxLength = 99999999;

	void Start () {
        RandomSeed();
	}

    // Converts string to an integer
    public void InsertSeed(uint input)
    {
        if (input > maxLength) Debug.Log("Inserted seed is either to short or to long!");
        else {
            seed = input;
            Random.seed = (int) input;
        }   
    }

    // Creates a random seed
    public void RandomSeed()
    {
        InsertSeed((uint) Random.Range(0, maxLength));
    }
}