using System.Collections.Generic;
using Photon.Pun;
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

    public class PlayerData : MonoBehaviourPun {
        public static List<GameObject> Players;

        private const int LaserDamageRange = 10; // Makes the amount of damage the laser does vary a bit

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

        private PlayerStats _playerStats;

        private void Start() {
            _playerAgent = GetComponent<NavMeshAgent>();
            _playerStats = new PlayerStats();
            StatsManager.playerStatsList.Add(_playerStats);
            _playerStats.playerName = PhotonNetwork.NickName;

            // Initialise the players list
            Players = new List<GameObject>();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
            if (!DebugSettings.Debug) this.photonView.RPC("RPC_UpdatePlayerLists", RpcTarget.Others);
            // TODO add other players to list
            
            _role = Role.StationCommander; // TODO assign roles in the menu
        
            _maxHealth = 100; // TODO different stats for different roles
            _maxSpeed = 2.5f;

            _rotateSpeed = 0.005f;

            _laserSpeed = 1000;
            
            _health = _maxHealth;
            
            // The current speed of the player is will be stored in the speed of its NavMeshAgent
            _playerAgent.speed = 0;

            _resources = 0;

            _laserDamage = 20;
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

        private void Update()
        {
            if (!_youDiedWrittenOnScreen &&_health <= 0)
            {
                EventsManager.AddMessage("YOU DIED");
                _youDiedWrittenOnScreen = true;
            }
        }

        public List<GameObject> GetList()
        {
            return Players;
        }
        public int GetResources() {
            return _resources;
        }

        public void AddResources(int resources) {
            _resources += resources;
            _playerStats.resourcesHarvested += resources;
        }

        public void RemoveResources()
        {
            _resources = 0;
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

        public int GetLaserDamage()
        {
            // Make the amount of damage vary a bit.
            return _laserDamage + Random.Range(-LaserDamageRange, LaserDamageRange + 1);
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