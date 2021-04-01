using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarTiles : MonoBehaviour
{
    public int FogOfWarRadius;
    private GameObject[] _players;
    private GameObject[,] _grid;
    
    // Start is called before the first frame update
    void Start()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Find each player's location, toggle visibility for "cover" on tiles in defined radius, make tiles on the border black agai
    }
}
