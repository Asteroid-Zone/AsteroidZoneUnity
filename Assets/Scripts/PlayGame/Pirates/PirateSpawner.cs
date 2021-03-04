using Photon.Pun;
using System;
using Statics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayGame.Pirates
{
    public class PirateSpawner : MonoBehaviour
    {
        public GameObject gridManager;
        public GameObject pirate;
        
        private GridManager _gridManager;
        // Will be used as the size of the checked space when spawning (checked space should be empty)
        private float _spawnRangeCheck; 
        private int _maxPirates;

        // Every X seconds, there is a chance for an pirate to spawn on a random grid coordinate 
        public float probability;
        public float everyXSeconds;

    
        // Start is called before the first frame update
    
        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            _gridManager = gridManager.GetComponent<GridManager>();
            InvokeRepeating(nameof(PirateRNG), 0, everyXSeconds);
            
            // Checked space is the half size in OverlapBoxNonAlloc
            _spawnRangeCheck = _gridManager.GetCellSize() / 2f;
            _maxPirates = (int) Math.Floor(2 * Math.Sqrt(_gridManager.GetTotalCells()));
        }

        private void PirateRNG()
        {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            // Don't spawn pirates if the maximum count is reached.
            if (transform.childCount >= _maxPirates)
            {
                return;
            }
            
            var generatedProb = Random.Range(0, 1.0f);
            if (generatedProb < probability)
            {
                if (DebugSettings.SpawnPirates) SpawnPirate();
            }
        }

        private void SpawnPirate()
        {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
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
            
            // Spawn the new pirate
            GameObject newPirate;
            if (!DebugSettings.Debug) newPirate = PhotonNetwork.Instantiate("PirateShip", randomGlobalCoord, Quaternion.identity);
            else newPirate = Instantiate(pirate, randomGlobalCoord, Quaternion.identity);
            newPirate.transform.parent = gameObject.transform;
        }
    }
}
