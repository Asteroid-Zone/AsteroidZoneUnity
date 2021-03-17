﻿using Photon.Pun;
using System;
using Statics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayGame.Pirates {
    public class PirateSpawner : MonoBehaviour {
        #region Singleton
        private static PirateSpawner _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
        }
        
        public static PirateSpawner GetInstance()
        {
            return _instance;
        }
        #endregion
        
        public GameObject gridManager;
        public GameObject scout;
        public GameObject elite;
        public GameObject spaceStation;
        
        private GridManager _gridManager;
        // Will be used as the size of the checked space when spawning (checked space should be empty)
        private float _spawnRangeCheck; 
        private int _maxPirates;

        // Every X seconds, there is a chance for an pirate to spawn on a random grid coordinate 
        public float probability;
        public float everyXSeconds;

        // Start is called before the first frame update
        private void Start() {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            _gridManager = gridManager.GetComponent<GridManager>();
            InvokeRepeating(nameof(PirateRNG), 0, everyXSeconds);
            
            // Checked space is the half size in OverlapBoxNonAlloc
            _spawnRangeCheck = _gridManager.GetCellSize() / 2f;
            _maxPirates = (int) Math.Floor(2 * Math.Sqrt(_gridManager.GetTotalCells()));
        }

        private void PirateRNG() {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            // Don't spawn pirates if the maximum count is reached.
            if (transform.childCount >= _maxPirates) {
                return;
            }
            
            var generatedProb = Random.Range(0, 1.0f);
            if (generatedProb < probability) {
                if (DebugSettings.SpawnPirates) SpawnPirate(PirateData.PirateType.Scout);
            }
        }

        public void SpawnReinforcements() {
            int reinforcements = Random.Range(2, 4);

            for (int i = 0; i < reinforcements; i++) {
                SpawnPirate(PirateData.PirateType.Elite);
            }
        }

        private void SpawnPirate(PirateData.PirateType type) {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            // Initialise some random grid coordinates on the map
            var randomGridCoord = new Vector2(Random.Range(0, GridManager.Width), Random.Range(0, GridManager.Height));
            
            // Transform the grid coordinates to global coordinates
            var randomGlobalCoord = GridManager.GridToGlobalCoord(randomGridCoord);

            // Half of the dimensions of the checked space
            var checkedSpaceHalfDims = new Vector3(_spawnRangeCheck, _spawnRangeCheck, _spawnRangeCheck);
            // If collisions were found for the current spawn, stop spawning
            if (Physics.OverlapBoxNonAlloc(randomGlobalCoord, checkedSpaceHalfDims, new Collider[16]) > 0) return;

            // Get the correct prefab
            GameObject pirate;
            string prefab;
            switch (type) {
                case PirateData.PirateType.Scout:
                    pirate = scout;
                    prefab = Prefabs.PirateScout;
                    break;
                case PirateData.PirateType.Elite:
                    pirate = elite;
                    prefab = Prefabs.PirateElite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            // Spawn the new pirate
            GameObject newPirate;
            if (!DebugSettings.Debug) newPirate = PhotonNetwork.InstantiateRoomObject(prefab, randomGlobalCoord, Quaternion.identity);
            else newPirate = Instantiate(pirate, randomGlobalCoord, Quaternion.identity);
            newPirate.transform.parent = gameObject.transform;
            newPirate.GetComponent<PirateController>().spaceStation = spaceStation.GetComponent<SpaceStation.SpaceStation>();
            newPirate.GetComponent<PirateController>().pirateSpawner = this;
        }
    }
}
