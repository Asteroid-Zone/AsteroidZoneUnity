using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace PlayGame
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
        public static readonly List<GameObject> Players = new List<GameObject>();

        private Role _role;
        
        public int AutoProp { get; set; }
    
        private int _maxHealth;
        private int _health;
    
        private float _maxSpeed;
        private float _speed;

        private int _resources;

        private void Start() {
            Players.AddRange(GameObject.FindGameObjectsWithTag(PlayerTag));
            _role = Role.StationCommander; // TODO assign roles in the menu
        
            _maxHealth = 100; // TODO different stats for different roles
            _maxSpeed = 2.5f;
            GetComponent<NavMeshAgent>().speed = _maxSpeed;
        
            _health = _maxHealth;
            _speed = _maxSpeed;

            _resources = 0;
        }

        public int GetResources() {
            return _resources;
        }

        public void AddResources(int resources) {
            _resources += resources;
        }

        public Role GetRole() {
            return _role;
        }
    
        public float GetMaxSpeed() {
            return _maxSpeed;
        }

        public float GetSpeed() {
            return _speed;
        }

        public void SetSpeed(float fraction) {
            _speed = fraction * _maxSpeed;
        }

        public int GetMaxHealth() {
            return _maxHealth;
        }

        public int GetHealth() {
            return _health;
        }
    }
}