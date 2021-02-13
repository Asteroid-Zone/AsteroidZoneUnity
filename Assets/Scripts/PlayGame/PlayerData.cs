using UnityEngine;

namespace PlayGame
{
    public enum Role {
        StationCommander,
        MilitaryTactician,
        MiningCaptain,
        Researcher
    }

    public class PlayerData : MonoBehaviour {

        private Role _role;
        
        public int AutoProp { get; set; }
    
        private int _maxHealth;
        private int _health;
    
        private float _maxSpeed;
        private float _speed;

        private void Start() {
            _role = Role.StationCommander; // TODO assign roles in the menu
        
            _maxHealth = 100; // TODO different stats for different roles
            _maxSpeed = 2.5f;
        
            _health = _maxHealth;
            _speed = _maxSpeed;
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