using Photon.Pun;
using System;
using Statics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayGame.Pirates {
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
        
        // Will be used as the size of the checked space when spawning (checked space should be empty)
        private float _spawnRangeCheck; 
        private int _maxPirates;

        // Start is called before the first frame update
        private void Start() {
            if (!DebugSettings.Debug && !PhotonNetwork.IsMasterClient) return;
            InvokeRepeating(nameof(PirateRNG), 0, GameConstants.PirateEveryXSeconds);
            
            // Checked space is the half size in OverlapBoxNonAlloc
            _spawnRangeCheck = GridManager.GetCellSize() / 2f;
            _maxPirates =  (int) (GameConstants.MaxPiratesMultiplier * Math.Floor(2 * Math.Sqrt(GridManager.GetTotalCells())));
        }

        private void PirateRNG() {
            if (!DebugSettings.Debug && !PhotonNetwork.IsMasterClient) return;
            // Don't spawn pirates if the maximum count is reached.
            if (transform.childCount >= _maxPirates) {
                return;
            }
            
            float generatedProb = Random.Range(0, 1.0f);
            if (generatedProb < GameConstants.PirateProbability) {
                if (DebugSettings.SpawnPirates) SpawnPirate(PirateData.PirateType.Scout);
            }
        }

        public void SpawnReinforcements() {
            if (!DebugSettings.Debug && !PhotonNetwork.IsMasterClient) return;
            int reinforcements = Random.Range(GameConstants.PirateMinReinforcements, GameConstants.PirateMaxReinforcements);

            for (int i = 0; i < reinforcements; i++) {
                SpawnPirate(PirateData.PirateType.Elite);
            }
        }

        private void SpawnPirate(PirateData.PirateType type) {
            if (!DebugSettings.Debug && !PhotonNetwork.IsMasterClient) return;

            // Initialise some random grid coordinates on the map
            Vector2 randomGridCoord = new Vector2(Random.Range(0, GameConstants.GridWidth), Random.Range(0, GameConstants.GridHeight));
            
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

        [PunRPC]
        public void RPC_SetPirateParam(int viewID) {
            GameObject newPirate = PhotonView.Find(viewID).gameObject;
            newPirate.transform.parent = gameObject.transform;
            newPirate.GetComponent<PirateController>().pirateSpawner = this;
        }

        public static PirateController[] GetAllPirateControllers()
        {
            return _instance.gameObject.GetComponentsInChildren<PirateController>();
        }
    }
}
