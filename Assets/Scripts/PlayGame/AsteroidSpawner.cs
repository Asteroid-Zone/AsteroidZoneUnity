using Photon.Pun;
using System;
using Statics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayGame
{
    public class AsteroidSpawner : MonoBehaviour
    {
        public GameObject gridManager;
        public GameObject asteroid;
        
        private GridManager _gridManager;
        // Will be used as the size of the checked space when spawning (checked space should be empty)
        private float _spawnRangeCheck; 
        private int _maxAsteroids;

        // Every X seconds, there is a chance for an asteroid to spawn on a random grid coordinate 
        public float probability;
        public float everyXSeconds;

    
        // Start is called before the first frame update
    
        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient && !Variables.Debug) return;
            _gridManager = gridManager.GetComponent<GridManager>();
            InvokeRepeating(nameof(AsteroidRNG), 0, everyXSeconds);
            
            // Checked space is the half size in OverlapBoxNonAlloc
            _spawnRangeCheck = _gridManager.GetCellSize() / 2f;
            _maxAsteroids = (int) Math.Floor(2 * Math.Sqrt(_gridManager.GetTotalCells()));
        }

        private void AsteroidRNG()
        {
            if (!PhotonNetwork.IsMasterClient && !Variables.Debug) return;
            // Don't spawn asteroids if the maximum count is reached.
            if (transform.childCount >= _maxAsteroids)
            {
                return;
            }
            
            var generatedProb = Random.Range(0, 1.0f);
            if (generatedProb < probability)
            {
                SpawnAsteroid();
            }
        }

        private void SpawnAsteroid()
        {
            if (!PhotonNetwork.IsMasterClient && !Variables.Debug) return;
            // Initialise some random grid coordinates on the map
            var randomGridCoord = new Vector2(Random.Range(0, _gridManager.width), Random.Range(0, _gridManager.height));
            
            // Transform the grid coordinates to global coordinates
            var randomGlobalCoord = _gridManager.GridToGlobalCoord(randomGridCoord);

            // Half of the dimensions of the checked space
            var checkedSpaceHalfDims = new Vector3(_spawnRangeCheck, _spawnRangeCheck, _spawnRangeCheck);
            if (Physics.OverlapBoxNonAlloc(randomGlobalCoord, checkedSpaceHalfDims, new Collider[16]) > 0)
            {
                // Collisions were found for the current spawn, therefore, stop spawning
                return;
            }
            
            // Spawn the new asteroid
            GameObject newAsteroid;
            if (!Variables.Debug) newAsteroid = PhotonNetwork.Instantiate("Asteroid", randomGlobalCoord, Quaternion.identity);
            else newAsteroid = Instantiate(asteroid, randomGlobalCoord, Quaternion.identity);
            newAsteroid.transform.parent = gameObject.transform;
        
        }
    }
}
