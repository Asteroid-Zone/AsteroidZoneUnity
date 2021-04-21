using Photon.Pun;
using Photon.Realtime;
using PlayGame.Stats;
using Statics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Photon {
    public class PhotonRoom : MonoBehaviourPunCallbacks {
        [Tooltip("The UI Panel with Room Options")]
        [SerializeField]
        private GameObject roomControlPanel;
        [Tooltip("The UI Panel with lobby Options")]
        [SerializeField]
        private GameObject lobbyControlPanel;
        private GameObject _progressLabel;
        public GameObject startButton;

        public GameObject playerListPrefab;
        public GameObject voiceChatPrefab;
        public Transform playerPanel;

        private static PhotonRoom _instance;

        public int multiplayerScene;
        private int _currentScene;

        private void Awake()
        {
            //Set up Singleton
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                if (_instance != this)
                {
                    Destroy(_instance.gameObject);
                    _instance = this;
                }
            }
            DontDestroyOnLoad(gameObject);
        }

        public static void ResetStaticVariables() {
            _instance = null;
        }

        public override void OnEnable()
        {
            //subscribe to functions
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
            SceneManager.sceneLoaded += OnSceneFinishedLoading;
        }

        public override void OnDisable()
        {
            //unsubscribe to functions
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
            SceneManager.sceneLoaded -= OnSceneFinishedLoading;
        }

        public override void OnJoinedRoom()
        {
            //sets player data when we join the room
            base.OnJoinedRoom();
            lobbyControlPanel.SetActive(false);
            roomControlPanel.SetActive(true);
            Debug.Log("Asteroid Zone/MainMenuFunction: OnJoinedRoom() called by PUN. Now this client is in a room.");
            if(PhotonNetwork.IsMasterClient)
            {
              startButton.SetActive(true);
            }
            else
            {
              startButton.SetActive(false);
            }
            Debug.Log("creating voice chat");
            Instantiate(voiceChatPrefab, gameObject.transform);
            ClearList();
            ListLobby();
        }

        public override void OnPlayerEnteredRoom(Player player)
        {
            base.OnPlayerEnteredRoom(player);
            Debug.Log("happening>?");
            ClearList();
            ListLobby();
        }

        public override void OnPlayerLeftRoom(Player player)
        {
          base.OnPlayerLeftRoom(player);
          Debug.Log("Player left room");
          ClearList();
          ListLobby();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
          PhotonNetwork.LeaveRoom();
          roomControlPanel.SetActive(false);
          lobbyControlPanel.SetActive(true);
          PhotonNetwork.JoinLobby();
        }

        public void StartGame() {
            if (PhotonNetwork.IsMasterClient) SyncGameConstants();
            Debug.Log("Loading Level");
            PhotonNetwork.LoadLevel(multiplayerScene);
            StatsManager.GameStats.startTime = Time.time;
        }

        private void SyncGameConstants() {
            photonView.RPC(nameof(RPC_SyncGameConstants), RpcTarget.AllBuffered, GameConstants.TimeLimit, GameConstants.GridHeight, GameConstants.GridWidth, GameConstants.GridCellSize);
            photonView.RPC(nameof(RPC_SyncPirateConstants), RpcTarget.AllBuffered, GameConstants.PirateLaserMiningRate, GameConstants.MaxPiratesMultiplier, GameConstants.PirateProbability, GameConstants.PirateEveryXSeconds, GameConstants.PirateMinReinforcements, GameConstants.PirateMaxReinforcements);
            photonView.RPC(nameof(RPC_SyncPirateScoutConstants), RpcTarget.AllBuffered, GameConstants.PirateScoutMaxHealth, GameConstants.PirateScoutSpeed, GameConstants.PirateScoutLookRadius, GameConstants.PirateScoutLaserSpeed, GameConstants.PirateScoutLaserRange, GameConstants.PirateScoutLaserDamageRange, GameConstants.PirateScoutLaserDamage, GameConstants.PirateScoutShotDelay);
            photonView.RPC(nameof(RPC_SyncPirateEliteConstants), RpcTarget.AllBuffered, GameConstants.PirateEliteMaxHealth, GameConstants.PirateEliteSpeed, GameConstants.PirateEliteLookRadius, GameConstants.PirateEliteLaserSpeed, GameConstants.PirateEliteLaserRange, GameConstants.PirateEliteLaserDamageRange, GameConstants.PirateEliteLaserDamage, GameConstants.PirateEliteShotDelay);
            photonView.RPC(nameof(RPC_SyncPlayerConstants), RpcTarget.AllBuffered, GameConstants.PlayerMaxHealth, GameConstants.PlayerMaxSpeed, GameConstants.PlayerRotateSpeed, GameConstants.PlayerLookRadius, GameConstants.PlayerMiningRange, GameConstants.PlayerMiningRate, GameConstants.PlayerMiningDelay, GameConstants.PlayerShotDelay, GameConstants.PlayerLaserSpeed, GameConstants.PlayerLaserDamage, GameConstants.PlayerLaserDamageRange, GameConstants.PlayerLaserMiningRate, GameConstants.PlayerLaserRange);
            photonView.RPC(nameof(RPC_SyncStationConstants), RpcTarget.AllBuffered, GameConstants.EnginesMaxHealth, GameConstants.HyperdriveMaxHealth, GameConstants.StationHullMaxHealth, GameConstants.SolarPanelsMaxHealth, GameConstants.ShieldGeneratorMaxHealth, GameConstants.StationMaxShields, GameConstants.StationShieldsRechargeRate);
            photonView.RPC(nameof(RPC_SyncAsteroidConstants), RpcTarget.AllBuffered, GameConstants.AsteroidMinResources, GameConstants.AsteroidMaxResources, GameConstants.AsteroidProbability, GameConstants.AsteroidEveryXSeconds, GameConstants.MaxAsteroidsMultiplier);
        }
        
        [PunRPC]
        public void RPC_SyncGameConstants(float timeLimit, int height, int width, int cell) {
            GameConstants.TimeLimit = timeLimit;
            GameConstants.GridHeight = height;
            GameConstants.GridWidth = width;
            GameConstants.GridCellSize = cell;
        }
        
        [PunRPC]
        public void RPC_SyncPirateConstants(int miningRate, float maxMultiplier, float probability, float everyX, int minReinforcements, int maxReinforcements) {
            GameConstants.PirateLaserMiningRate = miningRate;
            GameConstants.MaxPiratesMultiplier = maxMultiplier;
            GameConstants.PirateProbability = probability;
            GameConstants.PirateEveryXSeconds = everyX;
            GameConstants.PirateMinReinforcements = minReinforcements;
            GameConstants.PirateMaxReinforcements = maxReinforcements;
        }
        
        [PunRPC]
        public void RPC_SyncPirateScoutConstants(float maxHealth, float speed, float lookRadius, float laserSpeed, int laserRange, int damageRange, int damage, int delay) {
            GameConstants.PirateScoutMaxHealth = maxHealth;
            GameConstants.PirateScoutSpeed = speed;
            GameConstants.PirateScoutLookRadius = lookRadius;
            GameConstants.PirateScoutLaserSpeed = laserSpeed;
            GameConstants.PirateScoutLaserRange = laserRange;
            GameConstants.PirateScoutLaserDamageRange = damageRange;
            GameConstants.PirateScoutLaserDamage = damage;
            GameConstants.PirateScoutShotDelay = delay;
        }
        
        [PunRPC]
        public void RPC_SyncPirateEliteConstants(float maxHealth, float speed, float lookRadius, float laserSpeed, int laserRange, int damageRange, int damage, int delay) {
            GameConstants.PirateEliteMaxHealth = maxHealth;
            GameConstants.PirateEliteSpeed = speed;
            GameConstants.PirateEliteLookRadius = lookRadius;
            GameConstants.PirateEliteLaserSpeed = laserSpeed;
            GameConstants.PirateEliteLaserRange = laserRange;
            GameConstants.PirateEliteLaserDamageRange = damageRange;
            GameConstants.PirateEliteLaserDamage = damage;
            GameConstants.PirateEliteShotDelay = delay;
        }
        
        [PunRPC]
        public void RPC_SyncPlayerConstants(int maxHealth, float speed, float rotateSpeed, float lookRadius, int miningRange, int miningRate, int miningDelay, int shotDelay, int laserSpeed, int laserDamage, int damageRange, int laserMiningRate, int laserRange) {
            GameConstants.PlayerMaxHealth = maxHealth;
            GameConstants.PlayerMaxSpeed = speed;
            GameConstants.PlayerRotateSpeed = rotateSpeed;
            GameConstants.PlayerLookRadius = lookRadius;
            GameConstants.PlayerMiningRange = miningRange;
            GameConstants.PlayerMiningRate = miningRate;
            GameConstants.PlayerMiningDelay = miningDelay;
            GameConstants.PlayerShotDelay = shotDelay;
            GameConstants.PlayerLaserSpeed = laserSpeed;
            GameConstants.PlayerLaserDamage = laserDamage;
            GameConstants.PlayerLaserDamageRange = damageRange;
            GameConstants.PlayerLaserMiningRate = laserMiningRate;
            GameConstants.PlayerLaserRange = laserRange;
        }
        
        [PunRPC]
        public void RPC_SyncStationConstants(int engines, int hyperdrive, int hull, int solarPanels, int shieldGen, int shields, int rechargeRate) {
            GameConstants.EnginesMaxHealth = engines;
            GameConstants.HyperdriveMaxHealth = hyperdrive;
            GameConstants.StationHullMaxHealth = hull;
            GameConstants.SolarPanelsMaxHealth = solarPanels;
            GameConstants.ShieldGeneratorMaxHealth = shieldGen;
            GameConstants.StationMaxShields = shields;
            GameConstants.StationShieldsRechargeRate = rechargeRate;
        }
        
        [PunRPC]
        public void RPC_SyncAsteroidConstants(int minRes, int maxRes, float probability, float everyX, float maxMultiplier) {
            GameConstants.AsteroidMinResources = minRes;
            GameConstants.AsteroidMaxResources = maxRes;
            GameConstants.AsteroidProbability = probability;
            GameConstants.AsteroidEveryXSeconds = everyX;
            GameConstants.MaxAsteroidsMultiplier = maxMultiplier;
        }

        private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.buildIndex;
            if (_currentScene == multiplayerScene)
            {
                CreatePlayer();
            }
        }

        private static void CreatePlayer() {
            PhotonNetwork.Instantiate("PhotonNetworkPlayer", new Vector3(5f,0f,10f) , Quaternion.identity);
        }

        public void ClearList() {
            if (playerPanel == null) return;

            for (int i = playerPanel.childCount - 1; i >=0; i--) {
                Destroy(playerPanel.GetChild(i).gameObject);
            }
        }
        
        public void ListLobby() {
          if(PhotonNetwork.InRoom) {
            foreach(Player player in PhotonNetwork.PlayerList) {
                GameObject tempListing = Instantiate(playerListPrefab, playerPanel);
                Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                tempText.text = player.NickName;
            }
          }
        }
    }
}
