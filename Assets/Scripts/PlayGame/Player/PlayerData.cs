using System.Collections.Generic;
using Photon.GameControllers;
using Photon.Pun;
using PlayGame.Camera;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using PlayGame.Stats;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PlayGame.Player
{
    public enum Role {
        StationCommander,
        Miner
    }

    public enum QuestType {
        MineAsteroids,
        ReturnToStationResources,
        TransferResources,
        PirateWarning,
        ReturnToStationDefend,
        DefendStation,
        HelpPlayers,
        RepairStation,
        ActivateHyperdrive,
        FindAsteroids
    }

    public class PlayerData : MonoBehaviourPun {
        public static List<GameObject> Players;
        private int _playerID;

        public bool dead;

        public Role role;

        public GameObject viewableArea;
        public GameObject viewableAreaMinimap;

        private int _maxHealth;
        private int _health;
    
        private float _maxSpeed;
        private float _rotateSpeed;

        private int _laserSpeed;
        private int _laserDamage;
        private int _laserRange;

        private float _lookRadius;

        private int _resources;

        private NavMeshAgent _playerAgent;

        private PlayerStats _playerStats;

        public QuestType currentQuest;

        public MeshRenderer shipModel;
        public MeshRenderer gunLeft;
        public MeshRenderer gunRight;

        private Transform _spaceStation;

        private LaserGun _laserGun;
        private MiningLaser _miningLaser;
        private MoveObject _moveObject;

        private GameObject _deadPanel;
        
        private void Start() {
            _spaceStation = GameObject.FindGameObjectWithTag(Tags.StationTag).transform;
            _playerAgent = GetComponent<NavMeshAgent>();
            DontDestroyOnLoad(gameObject);

            _playerStats = new PlayerStats();
            if (!DebugSettings.Debug) _playerStats.photonID = photonView.ViewID;
            Debug.Log("Photon id: " + _playerStats.photonID);
            _playerStats.playerName = PhotonNetwork.NickName;
            StatsManager.PlayerStatsList.Add(_playerStats);

            // Initialise the players list
            UpdatePlayerLists();
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_UpdatePlayerLists), RpcTarget.Others);

            // Set camera to cockpit for miners and tactical for station commander, if single player play in cockpit mode
            if (photonView.IsMine) {
                bool cockpitMode = role == Role.Miner || DebugSettings.SinglePlayer;
                GameObject.FindGameObjectWithTag(Tags.CameraManagerTag).GetComponent<CameraManager>().SetMode(cockpitMode);
            }

            if (role == Role.StationCommander && !DebugSettings.SinglePlayer) SetUpStationCommander();
            else SetUpMiner(true);
        }

        private void SetUpMiner(bool initial) {
            if (initial) {
                _laserGun = gameObject.GetComponent<LaserGun>();
                _miningLaser = gameObject.GetComponent<MiningLaser>();
                _moveObject = gameObject.GetComponent<MoveObject>();
                
                if (photonView.IsMine) _deadPanel = GameObject.FindGameObjectWithTag(Tags.DeadPanel);
                
                _playerID = (_playerStats.photonID / 1000) - 1;
                //   _playerID = _playerID > 3 ? 3 : _playerID;
                //    _playerID = _playerID < 0 ? 0 : _playerID;
                SetPlayerColour();
            }
            
            if (photonView.IsMine) _deadPanel.SetActive(false);
            
            _maxHealth = GameConstants.PlayerMaxHealth;
            _maxSpeed = GameConstants.PlayerMaxSpeed;
            _rotateSpeed = GameConstants.PlayerRotateSpeed;

            _laserSpeed = GameConstants.PlayerLaserSpeed;
            _laserDamage = GameConstants.PlayerLaserDamage;
            _laserRange = GameConstants.PlayerLaserRange;

            _lookRadius = GameConstants.PlayerLookRadius;
            
            // The current speed of the player is will be stored in the speed of its NavMeshAgent
            _playerAgent.speed = 0;
            _resources = 0;
            _health = _maxHealth;

            ResizeViewableArea();
            
            currentQuest = QuestType.MineAsteroids;

            if (photonView.IsMine) transform.position = GameSetup.Instance.spawnPoints[_playerID].position; // Set spawn point
        }

        private void SetUpStationCommander() {
            transform.position = GridManager.GetGridCentre();
            gameObject.transform.position = _spaceStation.position;
            currentQuest = QuestType.HelpPlayers;
        }

        private void SetPlayerColour() {
            Color colour = Color.cyan;
            switch (_playerID) {
                case 1:
                    colour = Color.red;
                    break;
                case 2:
                    colour = Color.green;
                    break;
                case 3:
                    colour = Color.yellow;
                    break;
            }

            shipModel.material.color = colour;
            gunLeft.material.color = colour;
            gunRight.material.color = colour;
        }

        public void RespawnPlayer(int playerID) {
            if (!PhotonNetwork.IsMasterClient) return;
            photonView.RPC(nameof(RPC_RespawnPlayer), RpcTarget.AllBuffered, playerID);
        }
        
        [PunRPC]
        public void RPC_RespawnPlayer(int playerID) {
            foreach (GameObject o in Players) {
                PlayerData p = o.GetComponent<PlayerData>();
                if (p.GetPlayerID() == playerID) p.Respawn();
            }
        }

        private void Respawn() {
            dead = false;
            SetUpMiner(false);
            SetActiveRecursively(shipModel.gameObject, true);
        }

        // Sets the size of the viewable area ring and minimap ring
        private void ResizeViewableArea() {
            if (!DebugSettings.FogOfWar || !photonView.IsMine) {
                viewableArea.SetActive(false);
                viewableAreaMinimap.SetActive(false);
            } else {
                int size = ((int) GetLookRadius() * 2) / 10;
                viewableArea.transform.localScale = new Vector3(size, size, size);
                size *= 8;
                viewableAreaMinimap.transform.localScale = new Vector3(size, size, size);
            }
        }

        [PunRPC]
        public void RPC_UpdatePlayerLists()
        {
            UpdatePlayerLists();
        }

        public static void UpdatePlayerLists()
        {
            Players = new List<GameObject>();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
            
            // Add the station commander only if there is one
            GameObject stationCommander = GameObject.FindGameObjectWithTag(Tags.StationCommanderTag);
            if (stationCommander != null)
            {
                Players.Add(stationCommander);
            }
        }

        private void Update() {
            if (role != Role.StationCommander) {
                if (!dead && _health <= 0) {
                    Die();
                }
            } else {
                if (_spaceStation != null) gameObject.transform.position = _spaceStation.position;
            }
        }

        private void Die() {
            dead = true;
            
            if (photonView.IsMine) {
                _deadPanel.SetActive(true);
                EventsManager.AddMessage("YOU DIED");
            } else EventsManager.AddMessage(_playerStats.playerName + " DIED");
            
            SetActiveRecursively(shipModel.gameObject, false); // Hide the ship model
            
            _laserGun.StopShooting();
            _miningLaser.DisableMiningLaser();
            _moveObject.SetSpeed(0);
            _moveObject.SetLockTargetType(ToggleCommand.LockTargetType.None);
        }
        
        private void SetActiveRecursively(GameObject o, bool active) {
            o.SetActive(active);

            foreach (Transform child in o.transform){
                SetActiveRecursively(child.gameObject, active);
            }
        }

        public static List<GameObject> GetList() {
            return Players;
        }
        
        public int GetResources() {
            return _resources;
        }

        public void AddResources(int resources) {
            StatsManager.GameStats.resourcesHarvested += resources;
            _resources += resources;
            _playerStats.resourcesHarvested += resources;
        }
        
        public void RemoveResources(int amount) {
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_RemoveResources), RpcTarget.AllBuffered, amount);
            else _resources -= amount;
        }

        [PunRPC]
        public void RPC_RemoveResources(int amount) {
            _resources -= amount;
        }

        public Role GetRole() {
            return role;
        }
    
        public float GetMaxSpeed() {
            return _maxSpeed;
        }

        public float GetSpeed() {
            return _playerAgent.speed;
        }

        public float GetRotateSpeed() {
            return _rotateSpeed;
        }

        public void SetSpeed(float fraction) {
            _playerAgent.speed = fraction * _maxSpeed;
            
            if (fraction == 0)
            {
                _playerAgent.enabled = false;
            }
        }

        public int GetLaserSpeed() {
            return _laserSpeed;
        }

        public int GetMaxHealth() {
            return _maxHealth;
        }

        public int GetHealth() {
            return _health;
        }

        public int GetLaserRange() {
            return _laserRange;
        }

        // todo add modifier to this if adding powerups
        public float GetLookRadius() {
            return _lookRadius;
        }

        public QuestType GetQuest()
        {
            return currentQuest;
        }

        public void SetQuest(QuestType quest)
        {
            currentQuest = quest;
        }

        public int GetLaserDamage() {
            // Make the amount of damage vary a bit.
            return _laserDamage + Random.Range(-GameConstants.PlayerLaserDamageRange, GameConstants.PlayerLaserDamageRange + 1);
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health < 0)
            {
                _health = 0;
            }
        }

        public int GetPlayerID() {
            return _playerID;
        }

        public static PlayerData GetRandomDeadPlayer() {
            List<PlayerData> deadPlayers = new List<PlayerData>();
            
            foreach (GameObject player in Players) {
                PlayerData playerData = player.GetComponent<PlayerData>();
                if (playerData.dead) deadPlayers.Add(playerData);
            }

            if (deadPlayers.Count == 0) return null;

            int randomInt = Random.Range(0, deadPlayers.Count);
            return deadPlayers[randomInt];
        }

        public static PlayerData GetMyPlayerData()
        {
            // Gets the PlayerData object of the current (mine) player
            return DebugSettings.Debug
                ? Players[0].GetComponent<PlayerData>()
                : PhotonPlayer.Instance.myAvatar.GetComponent<PlayerData>();
        }

    }
}