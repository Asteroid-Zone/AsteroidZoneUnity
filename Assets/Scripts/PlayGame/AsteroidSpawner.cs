using Photon.Pun;
using System;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace PlayGame {
    
    /// <summary>
    /// This class controls the asteroid spawning.
    /// </summary>
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
        
        private void Start() {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            // Calls AsteroidRNG() every GameConstants.AsteroidEveryXSeconds seconds
            if (SceneManager.GetActiveScene().name != Scenes.TutorialScene) InvokeRepeating(nameof(AsteroidRNG), 0, GameConstants.AsteroidEveryXSeconds);
            
            // Checked space is the half size in OverlapBoxNonAlloc
            _spawnRangeCheck = GridManager.GetCellSize() / 2f;
            _maxAsteroids = (int) (GameConstants.MaxAsteroidsMultiplier * Math.Floor(2 * Math.Sqrt(GridManager.GetTotalCells())));
        }

        /// <summary>
        /// Has a chance to spawn an asteroid.
        /// This method is called every GameConstants.AsteroidEveryXSeconds seconds.
        /// <remarks>This method can only be called by the host or in debug mode.</remarks>
        /// </summary>
        private void AsteroidRNG() {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            
            // Don't spawn asteroids if the maximum count is reached.
            if (transform.childCount >= _maxAsteroids) return;
            
            float generatedProb = Random.Range(0, 1.0f);
            if (generatedProb < GameConstants.AsteroidProbability) {
                SpawnAsteroid();
            }
        }

        /// <summary>
        /// This method spawns a new asteroid in a random location.
        /// <remarks>This method can only be called by the host or in debug mode.</remarks>
        /// </summary>
        private void SpawnAsteroid() {
            if (!PhotonNetwork.IsMasterClient && !DebugSettings.Debug) return;
            
            // Initialise some random grid coordinates on the map
            Vector2 randomGridCoord = new Vector2(Random.Range(0, GameConstants.GridWidth), Random.Range(0, GameConstants.GridHeight));
            Vector3 randomGlobalCoord = GridManager.GridToGlobalCoord(randomGridCoord);

            // Half of the dimensions of the checked space
            Vector3 checkedSpaceHalfDims = new Vector3(_spawnRangeCheck, _spawnRangeCheck, _spawnRangeCheck);
            
            // If collisions were found for the current spawn, stop spawning
            if (Physics.OverlapBoxNonAlloc(randomGlobalCoord, checkedSpaceHalfDims, new Collider[16]) > 0) return;
            
            // Spawn the new asteroid
            GameObject newAsteroid;
            if (!DebugSettings.Debug) newAsteroid = PhotonNetwork.InstantiateRoomObject(Prefabs.Asteroid, randomGlobalCoord, Quaternion.identity);
            else newAsteroid = Instantiate(asteroid, randomGlobalCoord, Quaternion.identity);

            if (DebugSettings.Debug) newAsteroid.transform.parent = gameObject.transform;
            else photonView.RPC(nameof(SetParent), RpcTarget.AllBuffered, newAsteroid.GetPhotonView().ViewID);
        }

        /// <summary>
        /// This method spawns an asteroid at the given location.
        /// </summary>
        /// <param name="gridCoord">GridCoord to spawn the asteroid at.</param>
        public void SpawnAsteroid(GridCoord gridCoord) {
            GameObject newAsteroid = Instantiate(asteroid, gridCoord.GetWorldVector(), Quaternion.identity);
            newAsteroid.transform.parent = gameObject.transform;
        }

        /// <summary>
        /// This method sets the parent of an asteroid to this.
        /// </summary>
        /// <param name="photonViewID">The photonID of the asteroid.</param>
        [PunRPC]
        public void SetParent(int photonViewID) {
            PhotonView.Find(photonViewID).transform.parent = gameObject.transform;
        }
    }
}
