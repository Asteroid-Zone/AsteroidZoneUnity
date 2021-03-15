using UnityEngine;

namespace PlayGame.SpaceStation {
    public class ShieldGenerator : StationModule {
        
        private const int MaxHealth = 100;

        private int _maxShields = 100;
        private int _shields = 0;
        private int _rechargeRate = 1; // Amount the shields recharge per second
        private float _timeSinceLastCharge = 1; // Seconds since last charge

        public ShieldGenerator(SpaceStation station) : base("Shield Generator", MaxHealth, station) {
            
        }
        
        public void Update() {
            if (!IsFunctional()) return;
            
            // Only charge the shields every second
            if (_timeSinceLastCharge >= 1) {
                _timeSinceLastCharge = 0;
                RechargeShields();
            } else {
                _timeSinceLastCharge += Time.deltaTime;
            }
        }

        private void RechargeShields() {
            _shields += _rechargeRate; // Charge the shields
            if (_shields > _maxShields) _shields = _maxShields;
        }

        // Shields take as much of the damage as they can, return the rest of the damage
        public int AbsorbDamage(int damage) {
            _shields -= damage;
            if (_shields < 0) return _shields * -1;
            return 0;
        }

        public override string ToString() {
            return base.ToString() + "\nShields: " + _shields + "/" + _maxShields;
        }
    }
}