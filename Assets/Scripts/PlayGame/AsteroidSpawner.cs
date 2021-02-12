using System.Collections;
using System.Collections.Generic;
using PlayGame;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    // TODO: Prevent asteroids from instantiating on top of objects (ships, other asteroids)
    
    public GameObject GridManager;
    public GameObject Asteroid;
    private GridManager gridManager;
    
    // Every X seconds, there is a chance for an asteroid to spawn on a random grid coordinate 
    public float probability;
    public float everyXSeconds;
    
    // Start is called before the first frame update
    
    void Start()
    {
        gridManager = GridManager.GetComponent<GridManager>();
        InvokeRepeating(nameof(AsteroidRNG), 0, everyXSeconds);
    }

    void AsteroidRNG()
    {
        var generatedProb = Random.Range(0, 1.0f);
        if (generatedProb < probability)
        {
            SpawnAsteroid();
        }
    }

    void SpawnAsteroid()
    {
        // Might want to turn into helper function if we need random coordinates
        var height = gridManager.height;
        var width = gridManager.width;
        Vector2 randomGridCoord;
        randomGridCoord.x = Random.Range(0, width);
        randomGridCoord.y = Random.Range(0, height);
        var randomGlobalCoord = gridManager.GridToGlobalCoord(randomGridCoord);
        print("Spawning asteroid at X=" + randomGridCoord.x + ", Y=" + randomGridCoord.y);
        var newAsteroid = Instantiate(Asteroid, randomGlobalCoord, Quaternion.identity);
        newAsteroid.transform.parent = gameObject.transform;
    }
}
