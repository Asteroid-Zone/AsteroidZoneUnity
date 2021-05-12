using Photon.Pun;
using Photon.Realtime;
using PlayGame.Stats;
using Statics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Photon {
    
    /// <summary>
    /// This class provides the RoomControlPanel functions.
    /// </summary>
    public class PhotonRoom : MonoBehaviourPunCallbacks {
        
        [Tooltip("The UI Panel with Room Options")]
        [SerializeField]
        private GameObject roomControlPanel;
        [Tooltip("The UI Panel with lobby Options")]
        [SerializeField]
        private GameObject lobbyControlPanel;
        [SerializeField]
        private GameObject _progressLabel;
        public GameObject startButton;
        public GameObject settingsButton;

        public GameObject playerListPrefab;
        public GameObject voiceChatPrefab;
        public Transform playerPanel;

        private static PhotonRoom _instance;

        public int multiplayerScene;
        private int _currentScene;

        private void Awake() {
            //Set up Singleton
            if (_instance == null) {
                _instance = this;
            } else {
                if (_instance != this) {
                    Destroy(_instance.gameObject);
                    _instance = this;
                }
            }
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary> Reset the static instance. </summary>
        public static void ResetStaticVariables() {
            _instance = null;
        }

        public override void OnEnable() {
            base.OnEnable();
            // Subscribe to functions
            PhotonNetwork.AddCallbackTarget(this);
            SceneManager.sceneLoaded += OnSceneFinishedLoading;
        }

        public override void OnDisable() {
            base.OnDisable();
            // Unsubscribe to functions
            PhotonNetwork.RemoveCallbackTarget(this);
            SceneManager.sceneLoaded -= OnSceneFinishedLoading;
        }

        /// <summary>
        /// <para>Method overrides MonoBehaviourPunCallbacks.</para>
        /// Shows the room panel, instantiates the voice chat and lists the players.
        /// </summary>
        public override void OnJoinedRoom() {
            base.OnJoinedRoom(); // Sets player data when we join the room
            Debug.Log("Asteroid Zone/MainMenuFunction: OnJoinedRoom() called by PUN. Now this client is in a room.");
            
            // Show the room panel
            _progressLabel.SetActive(false);
            lobbyControlPanel.SetActive(false);
            roomControlPanel.SetActive(true);

            roomControlPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = $"Room: {PhotonNetwork.CurrentRoom.Name}";
            
            startButton.SetActive(PhotonNetwork.IsMasterClient); // Only the host can start the game
            settingsButton.SetActive(PhotonNetwork.IsMasterClient);
            
            Instantiate(voiceChatPrefab, gameObject.transform); // Create the voice chat
            
            ClearList();
            ListPlayersInRoom();
        }

        /// <summary>
        /// <para>Method overrides MonoBehaviourPunCallbacks.</para>
        /// <para>Method is called when a player joins the room.</para>
        /// Refreshes the list of players.
        /// </summary>
        /// <param name="player"></param>
        public override void OnPlayerEnteredRoom(Player player) {
            base.OnPlayerEnteredRoom(player);
            ClearList();
            ListPlayersInRoom();
        }

        /// <summary>
        /// <para>Method overrides MonoBehaviourPunCallbacks.</para>
        /// <para>Method is called when a player leaves the room.</para>
        /// Refreshes the list of players.
        /// </summary>
        /// <param name="player"></param>
        public override void OnPlayerLeftRoom(Player player) {
          base.OnPlayerLeftRoom(player);
          Debug.Log("Player left room (" + player.NickName + ")");
          ClearList();
          ListPlayersInRoom();
        }

        /// <summary>
        /// <para>Method overrides MonoBehaviourPunCallbacks.</para>
        /// <para>Method is called when the host leaves the room.</para>
        /// Leaves the room and loads the lobby panel.
        /// </summary>
        /// <param name="newMasterClient"></param>
        public override void OnMasterClientSwitched(Player newMasterClient) {
          PhotonNetwork.LeaveRoom();
          roomControlPanel.SetActive(false);
          lobbyControlPanel.SetActive(true);
          PhotonNetwork.JoinLobby();
        }

        /// <summary>
        /// <para>Method is called when the host presses the 'Start Game' button.</para>
        /// Syncs the game settings and loads the game.
        /// <remarks>This method can only be called by the host.</remarks>
        /// </summary>
        public void StartGame() {
            if (PhotonNetwork.IsMasterClient) SyncGameConstants();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Debug.Log("Loading Level");
            PhotonNetwork.LoadLevel(multiplayerScene);
            StatsManager.GameStats.startTime = Time.time;
        }
        
        /// <summary>
        /// <para>Method is called when the player presses the 'Leave Room' button.</para>
        /// Leaves the room and loads the lobby panel.
        /// </summary>
        public void LeaveRoom() {
            PhotonNetwork.LeaveRoom();
            roomControlPanel.SetActive(false);
            lobbyControlPanel.SetActive(true);
            PhotonNetwork.JoinLobby();
        }

        /// <summary>
        /// Performs RPC calls to sync all the game settings. Syncing is split into multiple calls to reduce the amount of parameters in each call.
        /// </summary>
        private void SyncGameConstants() {
            photonView.RPC(nameof(RPC_SyncGameConstants), RpcTarget.AllBuffered, GameConstants.TimeLimit, GameConstants.GridHeight, GameConstants.GridWidth, GameConstants.GridCellSize);
            photonView.RPC(nameof(RPC_SyncPirateConstants), RpcTarget.AllBuffered, GameConstants.PirateLaserMiningRate, GameConstants.MaxPiratesMultiplier, GameConstants.PirateProbability, GameConstants.PirateEveryXSeconds, GameConstants.PirateMinReinforcements, GameConstants.PirateMaxReinforcements);
            photonView.RPC(nameof(RPC_SyncPirateScoutConstants), RpcTarget.AllBuffered, GameConstants.PirateScoutMaxHealth, GameConstants.PirateScoutSpeed, GameConstants.PirateScoutLookRadius, GameConstants.PirateScoutLaserSpeed, GameConstants.PirateScoutLaserRange, GameConstants.PirateScoutLaserDamageRange, GameConstants.PirateScoutLaserDamage, GameConstants.PirateScoutShotDelay);
            photonView.RPC(nameof(RPC_SyncPirateEliteConstants), RpcTarget.AllBuffered, GameConstants.PirateEliteMaxHealth, GameConstants.PirateEliteSpeed, GameConstants.PirateEliteLookRadius, GameConstants.PirateEliteLaserSpeed, GameConstants.PirateEliteLaserRange, GameConstants.PirateEliteLaserDamageRange, GameConstants.PirateEliteLaserDamage, GameConstants.PirateEliteShotDelay);
            photonView.RPC(nameof(RPC_SyncPlayerConstants), RpcTarget.AllBuffered, GameConstants.PlayerMaxHealth, GameConstants.PlayerMaxSpeed, GameConstants.PlayerRotateSpeed, GameConstants.PlayerLookRadius, GameConstants.PlayerMiningRange, GameConstants.PlayerMiningRate, GameConstants.PlayerMiningDelay, GameConstants.PlayerShotDelay, GameConstants.PlayerLaserSpeed, GameConstants.PlayerLaserDamage, GameConstants.PlayerLaserDamageRange, GameConstants.PlayerLaserMiningRate, GameConstants.PlayerLaserRange);
            photonView.RPC(nameof(RPC_SyncStationConstants), RpcTarget.AllBuffered, GameConstants.EnginesMaxHealth, GameConstants.HyperdriveMaxHealth, GameConstants.StationHullMaxHealth, GameConstants.SolarPanelsMaxHealth, GameConstants.ShieldGeneratorMaxHealth, GameConstants.StationMaxShields, GameConstants.StationShieldsRechargeRate);
            photonView.RPC(nameof(RPC_SyncAsteroidConstants), RpcTarget.AllBuffered, GameConstants.AsteroidMinResources, GameConstants.AsteroidMaxResources, GameConstants.AsteroidProbability, GameConstants.AsteroidEveryXSeconds, GameConstants.MaxAsteroidsMultiplier);
        }
        
        /// <summary>
        /// Syncs the time limit and grid constants.
        /// </summary>
        /// <param name="timeLimit"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="cell"></param>
        [PunRPC]
        public void RPC_SyncGameConstants(float timeLimit, int height, int width, int cell) {
            GameConstants.TimeLimit = timeLimit;
            GameConstants.GridHeight = height;
            GameConstants.GridWidth = width;
            GameConstants.GridCellSize = cell;
        }
        
        /// <summary>
        /// Syncs the pirate spawning constants.
        /// </summary>
        /// <param name="miningRate"></param>
        /// <param name="maxMultiplier"></param>
        /// <param name="probability"></param>
        /// <param name="everyX"></param>
        /// <param name="minReinforcements"></param>
        /// <param name="maxReinforcements"></param>
        [PunRPC]
        public void RPC_SyncPirateConstants(int miningRate, float maxMultiplier, float probability, float everyX, int minReinforcements, int maxReinforcements) {
            GameConstants.PirateLaserMiningRate = miningRate;
            GameConstants.MaxPiratesMultiplier = maxMultiplier;
            GameConstants.PirateProbability = probability;
            GameConstants.PirateEveryXSeconds = everyX;
            GameConstants.PirateMinReinforcements = minReinforcements;
            GameConstants.PirateMaxReinforcements = maxReinforcements;
        }
        
        /// <summary>
        /// Syncs the pirate scout constants.
        /// </summary>
        /// <param name="maxHealth"></param>
        /// <param name="speed"></param>
        /// <param name="lookRadius"></param>
        /// <param name="laserSpeed"></param>
        /// <param name="laserRange"></param>
        /// <param name="damageRange"></param>
        /// <param name="damage"></param>
        /// <param name="delay"></param>
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
        
        /// <summary>
        /// Syncs the elite pirate constants.
        /// </summary>
        /// <param name="maxHealth"></param>
        /// <param name="speed"></param>
        /// <param name="lookRadius"></param>
        /// <param name="laserSpeed"></param>
        /// <param name="laserRange"></param>
        /// <param name="damageRange"></param>
        /// <param name="damage"></param>
        /// <param name="delay"></param>
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
        
        /// <summary>
        /// Syncs the player constants.
        /// </summary>
        /// <param name="maxHealth"></param>
        /// <param name="speed"></param>
        /// <param name="rotateSpeed"></param>
        /// <param name="lookRadius"></param>
        /// <param name="miningRange"></param>
        /// <param name="miningRate"></param>
        /// <param name="miningDelay"></param>
        /// <param name="shotDelay"></param>
        /// <param name="laserSpeed"></param>
        /// <param name="laserDamage"></param>
        /// <param name="damageRange"></param>
        /// <param name="laserMiningRate"></param>
        /// <param name="laserRange"></param>
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
        
        /// <summary>
        /// Syncs the space station constants.
        /// </summary>
        /// <param name="engines"></param>
        /// <param name="hyperdrive"></param>
        /// <param name="hull"></param>
        /// <param name="solarPanels"></param>
        /// <param name="shieldGen"></param>
        /// <param name="shields"></param>
        /// <param name="rechargeRate"></param>
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
        
        /// <summary>
        /// Syncs the asteroid constants.
        /// </summary>
        /// <param name="minRes"></param>
        /// <param name="maxRes"></param>
        /// <param name="probability"></param>
        /// <param name="everyX"></param>
        /// <param name="maxMultiplier"></param>
        [PunRPC]
        public void RPC_SyncAsteroidConstants(int minRes, int maxRes, float probability, float everyX, float maxMultiplier) {
            GameConstants.AsteroidMinResources = minRes;
            GameConstants.AsteroidMaxResources = maxRes;
            GameConstants.AsteroidProbability = probability;
            GameConstants.AsteroidEveryXSeconds = everyX;
            GameConstants.MaxAsteroidsMultiplier = maxMultiplier;
        }

        /// <summary>
        /// <para>Method is called when a new scene is loaded.</para>
        /// If the PlayGame Scene has been loaded, create a new PhotonNetworkPlayer. 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
            _currentScene = scene.buildIndex;
            if (_currentScene == multiplayerScene) {
                CreatePlayer();
            }
        }

        /// <summary>
        /// Instantiates a new PhotonNetworkPlayer.
        /// </summary>
        private static void CreatePlayer() {
            PhotonNetwork.Instantiate("PhotonNetworkPlayer", new Vector3(5f,0f,10f) , Quaternion.identity);
        }

        /// <summary>
        /// Clears the list of players displayed in the room panel.
        /// </summary>
        public void ClearList() {
            if (playerPanel == null) return;

            for (int i = playerPanel.childCount - 1; i >=0; i--) {
                Destroy(playerPanel.GetChild(i).gameObject);
            }
        }
        
        /// <summary>
        /// Creates a new PlayerListing for each player in the room.
        /// </summary>
        public void ListPlayersInRoom() {
          if(PhotonNetwork.InRoom) {
            foreach(Player player in PhotonNetwork.PlayerList) {
                GameObject tempListing = Instantiate(playerListPrefab, playerPanel);
                tempListing.transform.GetChild(0).GetComponent<Text>().text = player.NickName;
                tempListing.transform.GetChild(1).GetComponent<Text>().text = player.IsMasterClient ? "Commander" : "Miner";
            }
          }
        }
    }
}
