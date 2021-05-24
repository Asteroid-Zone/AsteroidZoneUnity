using Photon.Pun;
using System;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace PlayGame.Pirates {
    
    /// <summary>
    /// This class controls the pirate spawning.
    /// </summary>
    public class PirateSpawner : MonoBehaviourPun {
        
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

        public static void ResetStaticVariables() {
            _instance = null;
        }

        public static PirateSpawner GetInstance()
        {
            return _instance;
        }
        #endregion
        
        public GameObject scout;
        public GameObject elite;
        public InGameMusicSwitcher musicSwitcher;
        
        // Will be used as the size of the checked space when spawning (checked space should be empty)
        private float _spawnRangeCheck; 
        private int _maxPirates;

        /// <summary>
        /// Calls InvokeRepeating for the PirateRNG method and sets the maximum number of pirates that can be alive.
        /// <remarks>This method can only be called by the host or in debug mode.</remarks>
        /// </summary>
        private void Start() {
            musicSwitcher = GameObject.Find("BackgroundMusic").GetComponent<InGameMusicSwitcher>();
            if (!DebugSettings.Debug && !PhotonNetwork.IsMasterClient) return; // Only the host spawns pirates
            if (SceneManager.GetActiveScene().name != Scenes.TutorialScene) InvokeRepeating(nameof(PirateRNG), 0, GameConstants.PirateEveryXSeconds);
            
            // Checked space is the half size in OverlapBoxNonAlloc
            _spawnRangeCheck = GridManager.GetCellSize() / 2f;
            _maxPirates = (int) (GameConstants.MaxPiratesMultiplier * Math.Floor(2 * Math.Sqrt(GridManager.GetTotalCells())));
        }

        /// <summary>
        /// <para>Method is called every <c>GameConstants.PirateEveryXSeconds</c> seconds.</para>
        /// Has a random chance to spawn a scout pirate.
        /// <remarks>This method can only be called by the host or in debug mode.</remarks>
        /// </summary>
        private void PirateRNG() {
            if (!DebugSettings.Debug && !PhotonNetwork.IsMasterClient) return;
            
            // Don't spawn pirates if the maximum count is reached.
            if (transform.childCount >= _maxPirates) {
                return;
            }
            
            // Random chance to spawn a scout pirate
            float generatedProb = Random.Range(0, 1.0f);
            if (generatedProb < GameConstants.PirateProbability) {
                if (DebugSettings.SpawnPirates) SpawnPirate(PirateData.PirateType.Scout);
            }
        }

        /// <summary>
        /// <para>Method is called when pirates discover the space station.</para>
        /// Spawns a random number of elite pirates between the min and max values.
        /// <remarks>This method can only be called by the host or in debug mode.</remarks>
        /// </summary>
        public void SpawnReinforcements() {
            if (!DebugSettings.Debug && !PhotonNetwork.IsMasterClient) return;
            if (SceneManager.GetActiveScene().name == Scenes.TutorialScene) return;
            int reinforcements = Random.Range(GameConstants.PirateMinReinforcements, GameConstants.PirateMaxReinforcements);

            for (int i = 0; i < reinforcements; i++) {
                SpawnPirate(PirateData.PirateType.Elite);
            }
        }

        /// <summary>
        /// Spawns a pirate for tutorial.
        /// </summary>
        /// <param name="type">The type of pirate to spawn.</param>
        /// <param name="gridCoord">Used to dictate spawn position of pirate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if type is an invalid PirateType.</exception>
        public GameObject SpawnPirate(PirateData.PirateType type, GridCoord gridCoord) {
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
            
            GameObject newPirate = Instantiate(pirate, gridCoord.GetWorldVector(), Quaternion.identity);
            newPirate.transform.parent = gameObject.transform;
            newPirate.GetComponent<PirateController>().pirateSpawner = this;

            return newPirate;
        }

        /// <summary>
        /// Spawns a pirate in a random location.
        /// </summary>
        /// <remarks>This method can only be called by the host or in debug mode.</remarks>
        /// <param name="type">The type of pirate to spawn.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if type is an invalid PirateType.</exception>
        private void SpawnPirate(PirateData.PirateType type) {
            if (!DebugSettings.Debug && !PhotonNetwork.IsMasterClient) return;

            // Initialise some random grid coordinates on the map
            Vector2 randomGridCoord = new Vector2(Random.Range(0, GameConstants.GridWidth), Random.Range(0, GameConstants.GridHeight));
            var center = new Vector2((int)(GameConstants.GridWidth / 2), (int)(GameConstants.GridHeight / 2));
            if (randomGridCoord.x >= center.x - 2 && randomGridCoord.x <= center.x + 2 &&
                randomGridCoord.y >= center.y - 2 && randomGridCoord.y <= center.y + 2) return;

            // Transform the grid coordinates to global coordinates
            Vector3 randomGlobalCoord = GridManager.GridToGlobalCoord(randomGridCoord);

            // Half of the dimensions of the checked space
            Vector3 checkedSpaceHalfDims = new Vector3(_spawnRangeCheck, _spawnRangeCheck, _spawnRangeCheck);
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
            
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_SetPirateParam), RpcTarget.AllBuffered, newPirate.GetComponent<PhotonView>().ViewID);
            else {
                newPirate.transform.parent = gameObject.transform;
                newPirate.GetComponent<PirateController>().pirateSpawner = this;
            }
        }

        /// <summary>
        /// Sets the pirates parent to this spawner.
        /// </summary>
        /// <param name="viewID">The photonID of the pirate.</param>
        [PunRPC]
        public void RPC_SetPirateParam(int viewID) {
            GameObject newPirate = PhotonView.Find(viewID).gameObject;
            newPirate.transform.parent = gameObject.transform;
            newPirate.GetComponent<PirateController>().pirateSpawner = this;
        }

        /// <summary>
        /// Returns a list of all the pirates.
        /// </summary>
        public static PirateController[] GetAllPirateControllers()
        {
            return _instance.gameObject.GetComponentsInChildren<PirateController>();
        }
    }
}
