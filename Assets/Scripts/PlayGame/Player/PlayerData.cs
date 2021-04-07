using System.Collections.Generic;
using Photon.Pun;
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
        MilitaryTactician,
        MiningCaptain,
        Researcher
    }

    public enum QuestType {
        MineAsteroids,
        ResourcesToStation,
        PirateWarning,
        DefendStation,
        TransferResources
        // Escape to hyperspace, not sure because they get more points for staying longer
    }

    public class PlayerData : MonoBehaviourPun {
        public static List<GameObject> Players;

        private bool _youDiedWrittenOnScreen; // TODO remove this and make something else when player dies

        private Role _role;

        private int _maxHealth;
        private int _health;
    
        private float _maxSpeed;

        private float _rotateSpeed;

        private int _laserSpeed;

        private int _resources;

        private int _laserDamage;

        private NavMeshAgent _playerAgent;

        private Transform _lockTarget;

        public PlayerStats PlayerStats;

        public QuestType currentQuest;

        private void Start() {
            _playerAgent = GetComponent<NavMeshAgent>();
            DontDestroyOnLoad(gameObject);

            PlayerStats = new PlayerStats();
            if (!DebugSettings.Debug) PlayerStats.photonID = photonView.ViewID;
            PlayerStats.playerName = PhotonNetwork.NickName;
            StatsManager.PlayerStatsList.Add(PlayerStats);

            // Initialise the players list
            Players = new List<GameObject>();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
            if (!DebugSettings.Debug) this.photonView.RPC(nameof(RPC_UpdatePlayerLists), RpcTarget.Others);
            
            _role = Role.StationCommander; // TODO assign roles in the menu

            _maxHealth = GameConstants.PlayerMaxHealth;
            _maxSpeed = GameConstants.PlayerMaxSpeed;
            _rotateSpeed = GameConstants.PlayerRotateSpeed;

            _laserSpeed = GameConstants.PlayerLaserSpeed;
            _laserDamage = GameConstants.PlayerLaserDamage;
            
            // The current speed of the player is will be stored in the speed of its NavMeshAgent
            _playerAgent.speed = 0;
            _resources = 0;
            _health = _maxHealth;

            currentQuest = QuestType.MineAsteroids;
        }

        [PunRPC]
        public void RPC_UpdatePlayerLists()
        {
            Players.Clear();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
        }

        public static void UpdatePlayerLists()
        {
            Players.Clear();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
        }

        private void Update() {
            if (!_youDiedWrittenOnScreen &&_health <= 0)
            {
                EventsManager.AddMessage("YOU DIED");
                _youDiedWrittenOnScreen = true;
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
            return _role;
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

        public void SetLockTarget(Transform lockTarget)
        {
            _lockTarget = lockTarget;
        }

    }
}