using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PlayGame.Player
{
    public enum Role {
        StationCommander,
        MilitaryTactician,
        MiningCaptain,
        Researcher
    }

    public class PlayerData : MonoBehaviour
    {
        private const string PlayerTag = "Player";
        public static List<GameObject> Players;

        private bool _youDiedWrittenOnScreen;

        private Role _role;

        private int _maxHealth;
        private int _health;
    
        private float _maxSpeed;

        private int _laserSpeed;

        private int _resources;

        private NavMeshAgent _playerAgent;

        private void Start() {
            _playerAgent = GetComponent<NavMeshAgent>();
            
            // Initialise the players list
            Players = new List<GameObject>();
            Players.AddRange(GameObject.FindGameObjectsWithTag(PlayerTag));
            // TODO add other players to list
            
            _role = Role.StationCommander; // TODO assign roles in the menu
        
            _maxHealth = 100; // TODO different stats for different roles
            _maxSpeed = 2.5f;

            _laserSpeed = 1000;
            
            _health = _maxHealth;
            
            // The current speed of the player is will be stored in the speed of its NavMeshAgent
            _playerAgent.speed = 0;

            _resources = 0;
        }

        private void Update()
        {
            if (!_youDiedWrittenOnScreen &&_health <= 0)
            {
                EventsManager.AddMessageToQueue("YOU DIED");
                _youDiedWrittenOnScreen = true;
            }
        }

        public int GetResources() {
            return _resources;
        }

        public void AddResources(int resources) {
            _resources += resources;
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

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health < 0)
            {
                _health = 0;
            }
        }
    }
}