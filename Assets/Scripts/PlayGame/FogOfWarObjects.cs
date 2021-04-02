using System;
using System.Collections;
using System.Collections.Generic;
using PlayGame;
using UnityEngine;

public class FogOfWarObjects : MonoBehaviour
{
    private GameObject[] _asteroids;
    private GameObject[] _pirates;
    private GameObject _gridManagerObject;
    private GridManager _gridManager;
    private FogOfWarTiles _fogOfWarTiles;

    void Start()
    {
        _gridManagerObject = GameObject.Find("Grid Manager");
        _gridManager = _gridManagerObject.GetComponent<GridManager>();
        _fogOfWarTiles = _gridManagerObject.GetComponent<FogOfWarTiles>();
    }

    private void OnPreCull()
    {
        // TODO: Improve performance by having objects register themselves
        _asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        _pirates = GameObject.FindGameObjectsWithTag("Pirate");
        List <Vector2> _visibleTiles = _fogOfWarTiles.GetVisibleTiles();
        foreach (GameObject asteroid in _asteroids)
        {
            Vector2 position = _gridManager.GlobalToGridCoord(asteroid.transform.position);
            if (_visibleTiles.Contains(position))
            {
                asteroid.layer = 0;
            }
            else
            {
                asteroid.layer = 31;
            }
        }
        foreach (GameObject pirate in _pirates)
        {
            Vector2 position = _gridManager.GlobalToGridCoord(pirate.transform.position);
            if (_visibleTiles.Contains(position))
            {
                pirate.layer = 0;
                pirate.transform.GetChild(1).gameObject.layer = 0;
            }
            else
            {
                pirate.layer = 31;
                pirate.transform.GetChild(1).gameObject.layer = 31;
            }
        }
    }
}
