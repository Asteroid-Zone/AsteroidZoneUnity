using Photon.Pun;
using System;
using Statics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayGame {
    public class AsteroidSpawner : MonoBehaviourPun {
        #region Singleton
        private static AsteroidSpawner _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
        }

        public static void ResetStaticVariables() {
            _instance = null;
        }

        public static AsteroidSpawner GetInstance()
        {
            return _instance;
        }
        #endregion
        
        public GameObject gridManager;
        public GameObject asteroid;
        
        // Will be used as the size of the checked space when spawning (checked space should be empty)
        private float _spawnRangeCheck; 
        private int _maxAsteroids;
        
        // Start is called before the first frame update
        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            InvokeRepeating(nameof(AsteroidRNG), 0, GameConstants.AsteroidEveryXSeconds);
            
            // Checked space is the half size in OverlapBoxNonAlloc
            _spawnRangeCheck = GridManager.GetCellSize() / 2f;
            _maxAsteroids = (int) (GameConstants.MaxAsteroidsMultiplier * Math.Floor(2 * Math.Sqrt(GridManager.GetTotalCells())));
        }

        private void AsteroidRNG() {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            // Don't spawn asteroids if the maximum count is reached.
            if (transform.childCount >= _maxAsteroids)
            {
                return;
            }
            
            var generatedProb = Random.Range(0, 1.0f);
            if (generatedProb < GameConstants.AsteroidProbability)
            {
                SpawnAsteroid();
            }
        }

        private void SpawnAsteroid()
        {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            // Initialise some random grid coordinates on the map
            var randomGridCoord = new Vector2(Random.Range(0, GameConstants.GridWidth), Random.Range(0, GameConstants.GridHeight));
            
            // Transform the grid coordinates to global coordinates
            var randomGlobalCoord = GridManager.GridToGlobalCoord(randomGridCoord);

            // Half of the dimensions of the checked space
            var checkedSpaceHalfDims = new Vector3(_spawnRangeCheck, _spawnRangeCheck, _spawnRangeCheck);
            if (Physics.OverlapBoxNonAlloc(randomGlobalCoord, checkedSpaceHalfDims, new Collider[16]) > 0)
            {
                // Collisions were found for the current spawn, therefore, stop spawning
                return;
            }
            
            // Spawn the new asteroid
            GameObject newAsteroid;
            if (!DebugSettings.Debug) newAsteroid = PhotonNetwork.InstantiateRoomObject(Prefabs.Asteroid, randomGlobalCoord, Quaternion.identity);
            else newAsteroid = Instantiate(asteroid, randomGlobalCoord, Quaternion.identity);

            if (DebugSettings.Debug) newAsteroid.transform.parent = gameObject.transform;
            else photonView.RPC(nameof(SetParent), RpcTarget.AllBuffered, newAsteroid.GetPhotonView().ViewID);
        }

        [PunRPC]
        public void SetParent(int photonViewID) {
            PhotonView.Find(photonViewID).transform.parent = gameObject.transform;
        }
    }
}
