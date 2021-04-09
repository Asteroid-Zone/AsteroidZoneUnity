using System.Collections.Generic;
using System.Linq;
using Photon.GameControllers;
using Photon.Pun;
using PlayGame.Camera;
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
        ActivateHyperdrive
    }

    public class PlayerData : MonoBehaviourPun {
        public static List<GameObject> Players;
        private int _playerID;

        private bool _youDiedWrittenOnScreen; // TODO remove this and make something else when player dies

        public Role role;

        private int _maxHealth;
        private int _health;
    
        private float _maxSpeed;
        private float _rotateSpeed;

        private int _laserSpeed;
        private int _laserDamage;
        private int _laserRange;

        private int _resources;

        private NavMeshAgent _playerAgent;

        private Transform _lockTarget;

        public PlayerStats PlayerStats;

        public QuestType currentQuest;

        private Transform _spaceStation;
        
        private void Start() {
            _spaceStation = GameObject.FindGameObjectWithTag(Tags.StationTag).transform;
            _playerAgent = GetComponent<NavMeshAgent>();
            DontDestroyOnLoad(gameObject);

            PlayerStats = new PlayerStats();
            if (!DebugSettings.Debug) PlayerStats.photonID = photonView.ViewID;
            Debug.Log(PlayerStats.photonID);
            PlayerStats.playerName = PhotonNetwork.NickName;
            StatsManager.PlayerStatsList.Add(PlayerStats);

            // Initialise the players list
            Players = new List<GameObject>();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
            Players.Add(GameObject.FindGameObjectWithTag(Tags.StationCommanderTag));
            if (!DebugSettings.Debug) this.photonView.RPC(nameof(RPC_UpdatePlayerLists), RpcTarget.Others);

            // Set camera to cockpit for miners and tactical for station commander, if single player play in cockpit mode
            if (photonView.IsMine) {
                bool cockpitMode = role == Role.Miner || DebugSettings.SinglePlayer;
                GameObject.FindGameObjectWithTag(Tags.CameraManagerTag).GetComponent<CameraManager>().SetMode(cockpitMode);
            }

            if (role == Role.StationCommander && !DebugSettings.SinglePlayer) SetUpStationCommander();
            else SetUpMiner();
        }

        private void SetUpMiner() {
            _maxHealth = GameConstants.PlayerMaxHealth;
            _maxSpeed = GameConstants.PlayerMaxSpeed;
            _rotateSpeed = GameConstants.PlayerRotateSpeed;

            _laserSpeed = GameConstants.PlayerLaserSpeed;
            _laserDamage = GameConstants.PlayerLaserDamage;
            _laserRange = GameConstants.PlayerLaserRange;
            
            // The current speed of the player is will be stored in the speed of its NavMeshAgent
            _playerAgent.speed = 0;
            _resources = 0;
            _health = _maxHealth;
            
            currentQuest = QuestType.MineAsteroids;
            _playerID = (PlayerStats.photonID / 1000) - 1;
         //   _playerID = _playerID > 3 ? 3 : _playerID;
        //    _playerID = _playerID < 0 ? 0 : _playerID;
        }

        private void SetUpStationCommander() {
            transform.position = GridManager.GetGridCentre();
            gameObject.transform.position = _spaceStation.position;
            currentQuest = QuestType.HelpPlayers;
        }

        [PunRPC]
        public void RPC_UpdatePlayerLists()
        {
            Players.Clear();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
            Players.Add(GameObject.FindGameObjectWithTag(Tags.StationCommanderTag));
        }

        public static void UpdatePlayerLists()
        {
            Players.Clear();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
            Players.Add(GameObject.FindGameObjectWithTag(Tags.StationCommanderTag));
        }

        private void Update() {
            if (role != Role.StationCommander) {
                if (!_youDiedWrittenOnScreen && _health <= 0) {
                    EventsManager.AddMessage("YOU DIED");
                    _youDiedWrittenOnScreen = true;
                }
            } else {
                gameObject.transform.position = _spaceStation.position;
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
            PlayerStats.resourcesHarvested += resources;
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

        public int GetPlayerID()
        {
            return _playerID;
        }

        public void SetLockTarget(Transform lockTarget)
        {
            _lockTarget = lockTarget;
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